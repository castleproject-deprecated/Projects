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
using System.Xml;
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Collections;

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// The ActionScript type map maintains a mapping of native types to ActionScript
    /// class (by alias name) and a list of <see cref="IASMapperFactory" />.  Together
    /// these are used to produce <see cref="IASSourceMapper" /> and <see cref="IASTargetMapper" />
    /// instances for mapping native .Net values to their <see cref="IASValue" /> representations
    /// and vice-versa.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This object is safe for concurrent use and modification by multiple threads.
    /// </para>
    /// <para>
    /// During ActionScript serialization, the class mapping information is used
    /// to determine the layout of the serialized class.
    /// During ActionScript deserialization, the class mapping information is used
    /// to find the appropriate type to instantiate and the method used for initialization.
    /// </para>
    /// </remarks>
    public sealed class ActionScriptMappingTable
    {
        private readonly object syncRoot = new object();

        private Dictionary<string, ActionScriptClassMapping> classMappingsByAlias;
        private Dictionary<Type, ActionScriptClassMapping> classMappingsByType;
        private List<IASMapperFactory> mapperFactories;

        private Dictionary<ASTargetMappingDescriptor, IASTargetMapper> targetMapperCache;
        private Dictionary<ASSourceMappingDescriptor, IASSourceMapper> sourceMapperCache;

        /// <summary>
        /// Creates an empty mapping table.
        /// </summary>
        public ActionScriptMappingTable()
        {
            classMappingsByAlias = new Dictionary<string, ActionScriptClassMapping>();
            classMappingsByType = new Dictionary<Type, ActionScriptClassMapping>();
            mapperFactories = new List<IASMapperFactory>();

            targetMapperCache = new Dictionary<ASTargetMappingDescriptor, IASTargetMapper>();
            sourceMapperCache = new Dictionary<ASSourceMappingDescriptor, IASSourceMapper>();
        }

        /// <summary>
        /// Clears the caches maintained by the mapping table.
        /// </summary>
        /// <remarks>
        /// Call this method whenever changes to the table, its mappings or its mapper factories
        /// may produce new mappings after the mapping table has been put into service.
        /// </remarks>
        public void ClearCaches()
        {
            lock (syncRoot)
            {
                targetMapperCache.Clear();
                sourceMapperCache.Clear();
            }
        }

        /// <summary>
        /// Gets an ActionScript target mapper that satisfies the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The target mapping descriptor</param>
        /// <returns>The ActionScript target mapper, or null if no compatible mapper can be obtained by any registered factory</returns>
        public IASTargetMapper GetASTargetMapper(ASTargetMappingDescriptor descriptor)
        {
            lock (syncRoot)
            {
                IASTargetMapper mapper;
                if (!targetMapperCache.TryGetValue(descriptor, out mapper))
                {
                    foreach (IASMapperFactory factory in mapperFactories)
                    {
                        mapper = factory.GetASTargetMapper(descriptor);
                        if (mapper != null)
                            break;
                    }

                    targetMapperCache.Add(descriptor, mapper);
                }

                return mapper;
            }
        }

        /// <summary>
        /// Gets an ActionScript source mapper that satisfies the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The source mapping descriptor</param>
        /// <returns>The ActionScript target mapper, or null if no compatible mapper can be obtained by any registered factory</returns>
        public IASSourceMapper GetASSourceMapper(ASSourceMappingDescriptor descriptor)
        {
            lock (syncRoot)
            {
                IASSourceMapper mapper;
                if (!sourceMapperCache.TryGetValue(descriptor, out mapper))
                {
                    foreach (IASMapperFactory factory in mapperFactories)
                    {
                        mapper = factory.GetASSourceMapper(descriptor);
                        if (mapper != null)
                            break;
                    }

                    sourceMapperCache.Add(descriptor, mapper);
                }

                return mapper;
            }
        }

        /// <summary>
        /// Gets the class mapping for the specified class alias, if one exists.
        /// </summary>
        /// <param name="classAlias">The class alias</param>
        /// <returns>The class mapping, or null if none exists</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="classAlias"/> is null</exception>
        public ActionScriptClassMapping GetClassMappingByAlias(string classAlias)
        {
            if (classAlias == null)
                throw new ArgumentNullException("classAlias");

            lock (syncRoot)
            {
                ActionScriptClassMapping mapping;
                if (classMappingsByAlias.TryGetValue(classAlias, out mapping))
                    return mapping;
            }

            return null;
        }

        /// <summary>
        /// Gets the class mapping for the specified type, if one exists.
        /// </summary>
        /// <param name="nativeType">The native type</param>
        /// <returns>The class mapping, or null if none exists</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nativeType"/> is null</exception>
        public ActionScriptClassMapping GetClassMappingByType(Type nativeType)
        {
            if (nativeType == null)
                throw new ArgumentNullException("nativeType");

            lock (syncRoot)
            {
                ActionScriptClassMapping mapping;
                if (classMappingsByType.TryGetValue(nativeType, out mapping))
                    return mapping;
            }

            return null;
        }

        /// <summary>
        /// Registers a mapper factory.
        /// </summary>
        /// <param name="mapperFactory">The mapper factory to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapperFactory"/> is null</exception>
        public void RegisterMapperFactory(IASMapperFactory mapperFactory)
        {
            if (mapperFactory == null)
                throw new ArgumentNullException("mapperFactory");

            lock (syncRoot)
            {
                mapperFactories.Add(mapperFactory);
            }
        }

        /// <summary>
        /// Registers a class mapping.
        /// </summary>
        /// <param name="classMapping">The class mapping data</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="classMapping"/> is null</exception>
        /// <exception cref="ActionScriptException">Thrown if the mapped native type
        /// or its ActionScript alias have already been registered with different mappings</exception>
        public void RegisterClassMapping(ActionScriptClassMapping classMapping)
        {
            if (classMapping == null)
                throw new ArgumentNullException("classMapping");

            lock (syncRoot)
            {
                if (classMappingsByType.ContainsKey(classMapping.NativeType))
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "Cannot register class mapping because there is already a mapper registered (or cached) for native type '{0}'.",
                        classMapping.NativeType));

                if (classMappingsByAlias.ContainsKey(classMapping.ASClass.ClassAlias))
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "Cannot register class mapping because there is already a mapper registered (or cached) for class alias '{0}'.",
                        classMapping.ASClass.ClassAlias));

                // Now having validated the changes, apply them.
                classMappingsByType.Add(classMapping.NativeType, classMapping);

                string classAlias = classMapping.ASClass.ClassAlias;
                if (classAlias.Length != 0)
                    classMappingsByAlias.Add(classAlias, classMapping);
            }
        }

        /// <summary>
        /// Registers a type for serialization.
        /// </summary>
        /// <remarks>
        /// This method is equivalent to calling <see cref="RegisterType(Type, string)" /> with a null value
        /// for the class alias override parameter.
        /// It serves a similar purpose to the "flex.net.registerClassAlias" method on the client side.
        /// </remarks>
        /// <param name="nativeType">The class to map</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nativeType"/> is null</exception>
        /// <exception cref="ActionScriptException">Thrown if the mapped native type
        /// or the class alias have already been registered with different mappings</exception>
        /// <exception cref="ActionScriptException">Thrown when an error occurs generating the class mapping</exception>
        public void RegisterType(Type nativeType)
        {
            RegisterType(nativeType, null);
        }

        /// <summary>
        /// Registers a type for serialization with the specified class alias.
        /// </summary>
        /// <remarks>
        /// This method is equivalent to calling <see cref="ActionScriptMappingReflector.CreateClassMapping" />
        /// followed by <see cref="RegisterClassMapping" />.
        /// It serves a similar purpose to the "flex.net.registerClassAlias" method on the client side.
        /// </remarks>
        /// <param name="nativeType">The native type</param>
        /// <param name="classAliasOverride">The ActionScript class alias</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nativeType"/> is null</exception>
        /// <exception cref="ActionScriptException">Thrown if the mapped native type
        /// or the class alias have already been registered with different mappings</exception>
        /// <exception cref="ActionScriptException">Thrown when an error occurs generating the class mapping</exception>
        public void RegisterType(Type nativeType, string classAliasOverride)
        {
            if (nativeType == null)
                throw new ArgumentNullException("nativeType");

            ActionScriptClassMapping classMapping = ActionScriptMappingReflector.CreateClassMapping(nativeType, classAliasOverride);
            RegisterClassMapping(classMapping);
        }

        /// <summary>
        /// Registers all ActionScript serializable types within the specified
        /// assembly with the mapper.
        /// </summary>
        /// <remarks>
        /// This method is equivalent to calling <see cref="RegisterType(Type)" />
        /// for each public <see cref="Type" /> in the <param name="assembly" /> that is
        /// decorated with <see cref="ActionScriptClassAttribute" />.
        /// </remarks>
        /// <param name="assembly">The assembly to search for serializable types</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        /// <exception cref="ActionScriptException">Thrown if any of the mapped native types
        /// or their ActionScript aliases have already been registered with different mappings</exception>
        /// <exception cref="ActionScriptException">Thrown when an error occurs generating a class mapping</exception>
        public void RegisterTypesInAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            foreach (ActionScriptClassMapping classMapping in ActionScriptMappingReflector.GetClassMappingsForTypesInAssembly(assembly))
            {
                RegisterClassMapping(classMapping);
            }
        }

        /// <summary>
        /// Registers all built-in mapper factories.
        /// </summary>
        /// <todo>
        /// This probably isn't the best place to do this.
        /// </todo>
        public void RegisterBuiltInMapperFactories()
        {
            RegisterMapperFactory(new ArrayMapperFactory());
            RegisterMapperFactory(new GenericCollectionMapperFactory());
            RegisterMapperFactory(new GenericDictionaryMapperFactory());
            RegisterMapperFactory(new PrimitiveMapperFactory());
            RegisterMapperFactory(new ObjectMapperFactory(this));

            // Register all built-in types.
            RegisterTypesInAssembly(GetType().Assembly);
        }

        /// <summary>
        /// Gets the default native type to use for mapping ActionScript values with certain properties.
        /// </summary>
        /// <remarks>
        /// The default native type can be used when insufficient type information is available to
        /// perform a mapping unambiguously.
        /// </remarks>
        /// <param name="kind">The value's kind</param>
        /// <param name="classAlias">The value's class alias or an empty string if none</param>
        /// <param name="contentFlags">The value's content flags</param>
        /// <returns>The default native type</returns>
        public Type GetDefaultNativeType(ASTypeKind kind, string classAlias, ASValueContentFlags contentFlags)
        {
            switch (kind)
            {
                case ASTypeKind.Array:
                    if ((contentFlags & ASValueContentFlags.HasDynamicProperties) == 0)
                        return typeof(object[]);
                    if ((contentFlags & ASValueContentFlags.HasIndexedValues) == 0)
                        return typeof(Dictionary<string, object>);
                    return typeof(MixedArray<object>);

                case ASTypeKind.Boolean:
                    return typeof(bool);

                case ASTypeKind.ByteArray:
                    return typeof(byte[]);

                case ASTypeKind.Date:
                    return typeof(DateTime);

                case ASTypeKind.Int29:
                    return typeof(int);

                case ASTypeKind.Null:
                    return typeof(object);

                case ASTypeKind.Number:
                    return typeof(double);

                case ASTypeKind.Object:
                    if (classAlias.Length != 0)
                    {
                        ActionScriptClassMapping classMapping = GetClassMappingByAlias(classAlias);
                        if (classMapping != null)
                            return classMapping.NativeType;
                    }

                    // Map untyped and unknown objects to dictionaries.
                    return typeof(Dictionary<string, object>);

                case ASTypeKind.String:
                    return typeof(string);

                case ASTypeKind.Undefined:
                    return typeof(ASUndefined);

                case ASTypeKind.Unsupported:
                    return typeof(ASUnsupported);

                case ASTypeKind.Xml:
                    return typeof(XmlDocument);

                default:
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "Unsupported type kind '{0}'.", kind));
            }
        }
    }
}
