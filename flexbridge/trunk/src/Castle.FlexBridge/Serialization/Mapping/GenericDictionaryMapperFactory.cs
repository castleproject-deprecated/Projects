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
using System.Reflection;
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Collections;
using System.Collections;
using Castle.FlexBridge.Serialization.Factories;

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// Provides mappers for implementations of <see cref="IDictionary{TKey, TValue}" />.
    /// New generic collection instances are created by consulting a list of <see cref="IGenericDictionaryFactory" />.
    /// </summary>
    public sealed class GenericDictionaryMapperFactory : BaseASMapperFactory
    {
        private List<IGenericDictionaryFactory> factories;

        private delegate GenericDictionaryMapper<TKey, TValue> GetMapperInstanceDelegate<TKey, TValue>(Type dictionaryType);
        private GetMapperInstanceDelegate<object, object> getMapperInstance;

        /// <summary>
        /// Creates a generic dictionary mapper factory.
        /// </summary>
        public GenericDictionaryMapperFactory()
        {
            factories = new List<IGenericDictionaryFactory>();
            getMapperInstance = GetMapperInstance<object, object>;

            // TODO: This is not the right place for registering built-ins!
            factories.Add(GenericDictionaryFactory.Instance);
        }

        /// <summary>
        /// Adds a factory to the list consulted when deserializing ActionScript values.
        /// </summary>
        /// <param name="factory">The factory to add</param>
        public void AddFactory(IGenericDictionaryFactory factory)
        {
            lock (factories)
                factories.Add(factory);
        }

        /// <inheritdoc />
        public override IASSourceMapper GetASSourceMapper(ASSourceMappingDescriptor descriptor)
        {
            if ((descriptor.SourceKind == ASTypeKind.Array || descriptor.SourceKind == ASTypeKind.Object)
                && (descriptor.SourceContentFlags & ASValueContentFlags.HasIndexedValues) == 0)
            {
                Type[] genericTypeArgs = MappingUtils.GetGenericDictionaryTypeArgs(descriptor.TargetNativeType);
                if (genericTypeArgs != null)
                {
                    return (IASSourceMapper)MappingUtils.InvokeGenericMethod(genericTypeArgs, getMapperInstance, new object[] { descriptor.TargetNativeType });
                }
            }

            return null;
        }

        /// <inheritdoc />
        public override IASTargetMapper GetASTargetMapper(ASTargetMappingDescriptor descriptor)
        {
            Type[] genericTypeArgs = MappingUtils.GetGenericDictionaryTypeArgs(descriptor.SourceNativeType);
            if (genericTypeArgs != null)
            {
                return (IASTargetMapper)MappingUtils.InvokeGenericMethod(genericTypeArgs, getMapperInstance, new object[] { null });
            }

            return null;
        }

        private GenericDictionaryMapper<TKey, TValue> GetMapperInstance<TKey, TValue>(Type dictionaryType)
        {
            IGenericDictionaryFactory factory;
            if (dictionaryType != null)
            {
                factory = GetFactory<TKey, TValue>(dictionaryType);
                if (factory == null)
                    return null; // Cannot produce instances of this collection type
            }
            else
            {
                factory = null;
            }

            return new GenericDictionaryMapper<TKey, TValue>(dictionaryType, factory);
        }

        private IGenericDictionaryFactory GetFactory<TKey, TValue>(Type baseType)
        {
            lock (factories)
            {
                foreach (IGenericDictionaryFactory factory in factories)
                {
                    if (factory.CanCreateInstance<TKey, TValue>(baseType))
                        return factory;
                }
            }

            return null;
        }

        /// <summary>
        /// A native array mapper specialized for the dictionary type.
        /// </summary>
        /// <typeparam name="TKey">The dictionary key type</typeparam>
        /// <typeparam name="TValue">The dictionary value type</typeparam>
        private sealed class GenericDictionaryMapper<TKey, TValue> : BaseASMapper, IASNativeArrayMapper
        {
            private Type dictionaryType;
            private IGenericDictionaryFactory dictionaryFactory;

            public GenericDictionaryMapper(Type dictionaryType, IGenericDictionaryFactory dictionaryFactory)
            {
                this.dictionaryType = dictionaryType;
                this.dictionaryFactory = dictionaryFactory;
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                return new ASNativeArray(nativeValue, this, false);
            }

            protected override object MapArrayToNative(IActionScriptSerializer serializer, Type nativeType,
                int indexedLength,
                IEnumerable<IASValue> indexedValues,
                IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties)
            {
                IDictionary<TKey, TValue> dict = CreateDictionaryInstance(indexedLength);

                // Add indexed values.
                int index = 0;
                foreach (IASValue indexedValue in indexedValues)
                {
                    TKey key = (TKey)Convert.ChangeType(index, typeof(TKey));
                    TValue value = (TValue)serializer.ToNative(indexedValue, typeof(TValue));
                    dict.Add(key, value);

                    index += 1;
                }

                // Add dynamic properties.
                foreach (KeyValuePair<string, IASValue> pair in dynamicProperties)
                {
                    TKey key = (TKey)Convert.ChangeType(pair.Key, typeof(TKey));
                    TValue value = (TValue)serializer.ToNative(pair.Value, typeof(TValue));
                    dict.Add(key, value);
                }

                return dict;
            }

            protected override object MapObjectToNative(IActionScriptSerializer serializer, Type nativeType,
                ASClass @class, IEnumerable<IASValue> memberValues,
                IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties,
                IExternalizable externalizableValue)
            {
                IDictionary<TKey, TValue> dict = CreateDictionaryInstance(@class.MemberNames.Count);

                // Add members.
                int memberIndex = 0;
                foreach (IASValue memberValue in memberValues)
                {
                    TKey key = (TKey)Convert.ChangeType(@class.MemberNames[memberIndex], typeof(TKey));
                    TValue value = (TValue)serializer.ToNative(memberValue, typeof(TValue));
                    dict.Add(key, value);

                    memberIndex += 1;
                }

                // Add dynamic properties.
                foreach (KeyValuePair<string, IASValue> pair in dynamicProperties)
                {
                    TKey key = (TKey)Convert.ChangeType(pair.Key, typeof(TKey));
                    TValue value = (TValue)serializer.ToNative(pair.Value, typeof(TValue));
                    dict.Add(key, value);
                }

                return dict;
            }

            private IDictionary<TKey, TValue> CreateDictionaryInstance(int initialCapacity)
            {
                return dictionaryFactory.CreateInstance<TKey, TValue>(dictionaryType, initialCapacity);
            }

            void IASNativeArrayMapper.AcceptVisitor(IActionScriptSerializer serializer, object nativeArray, IASValueVisitor visitor)
            {
                IDictionary<TKey, TValue> dict = (IDictionary<TKey, TValue>)nativeArray;

                visitor.VisitArray(serializer, 0, EmptyArray<IASValue>.Instance, GetDynamicProperties(serializer, dict));
            }

            ASValueContentFlags IASNativeArrayMapper.GetContentFlags(object nativeArray)
            {
                IDictionary<TKey, TValue> dict = (IDictionary<TKey, TValue>)nativeArray;

                return dict.Count != 0 ? ASValueContentFlags.HasDynamicProperties : ASValueContentFlags.None;
            }

            private static IEnumerable<KeyValuePair<string, IASValue>> GetDynamicProperties(IActionScriptSerializer serializer, IDictionary<TKey, TValue> dict)
            {
                foreach (KeyValuePair<TKey, TValue> pair in dict)
                {
                    string key = pair.Key.ToString();
                    IASValue value = serializer.ToASValue(pair.Value);
                    yield return new KeyValuePair<string, IASValue>(key, value);
                }
            }
        }
    }
}