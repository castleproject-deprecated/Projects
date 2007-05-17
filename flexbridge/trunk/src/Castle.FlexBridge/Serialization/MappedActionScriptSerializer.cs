// Copyright 2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Collections;
using Castle.FlexBridge.Serialization.Mapping;

namespace Castle.FlexBridge.Serialization
{
    /// <summary>
    /// An ActionScript serializer that uses a mapping table to decide how to map
    /// types from ActionScript to native and vice-versa.
    /// </summary>
    /// <remarks>
    /// This serializer maintains some state information about the object graph
    /// so that it can preserve referential identity of objects as they are serialized
    /// or deserialized.
    /// </remarks>
    public class MappedActionScriptSerializer : IActionScriptSerializer
    {
        private ActionScriptMappingTable mappingTable;

        /// <summary>
        /// This cache is used to maintain referential identity of native objects across
        /// the mapping process.  It is also needed to break circular references.
        /// </summary>
        private Dictionary<object, IASValue> nativeReferenceCache;

        /// <summary>
        /// This cache is used to preserve referential identity of ActionScript values
        /// mapped to native ones.  It is also needed to break circular references.
        /// </summary>
        private Dictionary<ASReferenceCacheKey, object> asReferenceCache;

        /// <summary>
        /// Creates a new serializer.
        /// </summary>
        /// <param name="mappingTable">The mapping table</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="mappingTable"/> is null</exception>
        public MappedActionScriptSerializer(ActionScriptMappingTable mappingTable)
        {
            if (mappingTable == null)
                throw new ArgumentNullException("mappingTable");

            this.mappingTable = mappingTable;
            this.nativeReferenceCache = new Dictionary<object, IASValue>(ReferenceEqualityComparer<object>.Instance);
            this.asReferenceCache = new Dictionary<ASReferenceCacheKey, object>();
        }

        public IASValue ToASValue(object nativeValue)
        {
            try
            {
                return InternalToASValue(nativeValue);
            }
            catch (ActionScriptException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ActionScriptException("An error occurred while mapping a native object to an ActionScript value.", ex);
            }
        }

        private IASValue InternalToASValue(object nativeValue)
        {
            if (nativeValue == null)
                return null;

            Type type = nativeValue.GetType();
            bool cacheable = IsEligibleForNativeReferenceCache(type);

            if (cacheable)
            {
                IASValue asValue;
                if (nativeReferenceCache.TryGetValue(nativeValue, out asValue))
                {
                    if (asValue == null)
                        throw new ActionScriptException("FIXME: A circular reference was encountered that could not be resolved by the current implementation.");
                    return asValue;
                }

                nativeReferenceCache.Add(nativeValue, null); // add a sentinel to detect the cycle

                try
                {
                    asValue = UncachedToASValue(nativeValue);
                }
                catch (Exception)
                {
                    nativeReferenceCache.Remove(nativeValue);
                    throw;
                }

                nativeReferenceCache[nativeValue] = asValue;
                return asValue;
            }
            else
            {
                return UncachedToASValue(nativeValue);
            }
        }

        private IASValue UncachedToASValue(object nativeValue)
        {
            // Use mappers.
            // FIXME!  This won't work if mapping ends up being recursive!
            Type nativeType = nativeValue.GetType();
            IASTargetMapper mapper = mappingTable.GetASTargetMapper(new ASTargetMappingDescriptor(nativeType));
            if (mapper != null)
                return mapper.ToASValue(this, nativeValue);

            throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                "Cannot find a suitable mapper for mapping type '{0}' to an ActionScript value.", nativeType.FullName));
        }

        public object ToNative(IASValue asValue, Type nativeType)
        {
            try
            {
                return InternalToNative(asValue, nativeType);
            }
            catch (ActionScriptException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ActionScriptException("An error occurred while mapping an ActionScript value to a native object.", ex);
            }
        }

        private object InternalToNative(IASValue asValue, Type nativeType)
        {
            if (asValue == null)
            {
                // Note: We cannot just map all null values to null references here
                //       because doing so would prevent mappers from expressing different
                //       preferences for null object mappings.  For example, consider the
                //       case of DBNull which is a reference-type for which all null
                //       values need to be mapped to the special DBNull.Value singleton.
                //       (Admittedly this is a somewhat contrived example since it's unlikely
                //       someone would ever declare a field of type DBNull.)
                //       Moreover, returning a null reference won't work for value types
                //       so at the least we must handle those by going through the mappers.
                //       So while we could perhaps apply a dubious optimization just for reference 
                //       types here, we do not!
                return UncachedToNative(ASNull.Value, nativeType);
            }

            // Look in the reference cache.
            ASTypeKind kind = asValue.Kind;
            bool cacheable = IsEligibleForASReferenceCache(kind);

            if (cacheable)
            {
                ASReferenceCacheKey key = new ASReferenceCacheKey(asValue, nativeType);
                object nativeValue;
                if (asReferenceCache.TryGetValue(key, out nativeValue))
                {
                    if (nativeValue == null)
                        throw new ActionScriptException("FIXME: A circular reference was encountered that could not be resolved by the current implementation.");
                    return nativeValue;
                }

                asReferenceCache.Add(key, null); // add a sentinel to detect the cycle

                try
                {
                    nativeValue = UncachedToNative(asValue, nativeType);
                }
                catch (Exception)
                {
                    asReferenceCache.Remove(key);
                    throw;
                }

                asReferenceCache[key] = nativeValue;
                return nativeValue;
            }
            else
            {
                return UncachedToNative(asValue, nativeType);
            }
        }

        private object UncachedToNative(IASValue asValue, Type nativeType)
        {
            ASTypeKind kind = asValue.Kind;
            ASClass asClass = asValue.Class;
            string classAlias = asClass != null ? asClass.ClassAlias : "";
            ASValueContentFlags contentFlags = asValue.ContentFlags;

            Type defaultNativeType = mappingTable.GetDefaultNativeType(kind, classAlias, contentFlags);

            // If the requested native type is not as precise as the default native type,
            // then use the default native type instead.  This rule is intended to avoid
            // the ambiguities that occur if we try to map to type "object" or some other
            // type that is too general.
            if (nativeType == null || nativeType.IsAssignableFrom(defaultNativeType))
                nativeType = defaultNativeType;

            // Quick short circuit if the desired type is a subtype of IASValue so mapping won't help.
            if (!typeof(IASValue).IsAssignableFrom(nativeType))
            {
                // Note: This will handle uninitialized values by returning null if no trivial
                //       conversion is possible without complete initialization.
                object value = asValue.GetNativeValue(nativeType);
                if (value != null)
                    return value;

                // If the value isn't initialized then give up because we might have to do mapping.
                if (!asValue.IsInitialized)
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "The ActionScript value cannot be mapped to type '{0}' because it is not completely initialized.",
                        nativeType != null ? nativeType.FullName : "<default>"));

                // Use mappers.
                // FIXME: This won't work if mapping ends up being recursive!
                ASSourceMappingDescriptor descriptor = new ASSourceMappingDescriptor(kind, classAlias, contentFlags, nativeType);
                IASSourceMapper mapper = mappingTable.GetASSourceMapper(descriptor);
                if (mapper != null)
                    return mapper.ToNative(this, asValue, nativeType);
            }

            // Apply default handling for null references.
            if (asValue == ASNull.Value && ! nativeType.IsValueType)
                return null;

            // As a last resort, if we can assign the AS value to the original type requested then
            // do so.  We generally prefer mappings over returning IASValue instances, but if there
            // is no other choice...
            if (nativeType.IsInstanceOfType(asValue))
                return asValue;

            // Give up!
            throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                "Cannot find a suitable mapper for mapping an ActionScript value of kind '{0}' with class alias '{1}' to an instance of type '{2}'.",
                asValue.Kind, classAlias, nativeType != null ? nativeType.FullName : "<default>"));
        }

        public IExternalizable CreateExternalizableInstance(string classAlias)
        {
            if (classAlias == null)
                throw new ArgumentNullException("classAlias");

            ActionScriptClassMapping classMapping = mappingTable.GetClassMappingByAlias(classAlias);
            if (classMapping != null)
            {
                if (classMapping.ASClass.Layout != ASClassLayout.Externalizable)
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "The class mapping for class alias '{0}' does not have an Externalizable layout.",
                        classAlias));

                return (IExternalizable)Activator.CreateInstance(classMapping.NativeType);
            }

            throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                "Cannot find a class mapping for class alias '{0}' from which to reconstruct an Externalizable ActionScript object.",
                classAlias));
        }

        public string GetClassAlias(Type nativeType)
        {
            if (nativeType == null)
                throw new ArgumentNullException("nativeType");

            ActionScriptClassMapping classMapping = mappingTable.GetClassMappingByType(nativeType);
            return classMapping != null ? classMapping.ASClass.ClassAlias : "";
        }

        /// <summary>
        /// Determines if native values of the specified type should be cached to
        /// preserve referential identity.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsEligibleForNativeReferenceCache(Type type)
        {
            return type.IsValueType;
        }

        /// <summary>
        /// Determines if ActionScript values of the specified type should be cached to
        /// preserve referential identity.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        private static bool IsEligibleForASReferenceCache(ASTypeKind kind)
        {
            switch (kind)
            {
                case ASTypeKind.String:
                case ASTypeKind.Object:
                case ASTypeKind.Array:
                case ASTypeKind.Xml:
                case ASTypeKind.ByteArray:
                    return true;

                default:
                    return false;
            }
        }


        /// <summary>
        /// The key for the reference cache.
        /// Compares values by reference.
        /// </summary>
        private struct ASReferenceCacheKey : IEquatable<ASReferenceCacheKey>
        {
            private readonly IASValue asValue;
            private readonly Type nativeType;

            public ASReferenceCacheKey(IASValue asValue, Type nativeType)
            {
                this.asValue = asValue;
                this.nativeType = nativeType;
            }

            public override bool Equals(object obj)
            {
                return Equals((ASReferenceCacheKey)obj);
            }

            public bool Equals(ASReferenceCacheKey other)
            {
                return ReferenceEquals(asValue, other.asValue)
                    && nativeType == other.nativeType;
            }

            public override int GetHashCode()
            {
                return RuntimeHelpers.GetHashCode(asValue) ^ (nativeType != null ? nativeType.GetHashCode() : 0);
            }
        }
    }
}
