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
using System.Reflection;
using Castle.FlexBridge.Collections;

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// Provides methods to assist with mapping collections.
    /// </summary>
    internal static class MappingUtils
    {
        private static readonly Type genericDictionaryTypeDefinition = typeof(IDictionary<,>);
        private static Dictionary<Type, Type[]> genericDictionaryTypeCache;

        private static readonly Type genericCollectionTypeDefinition = typeof(ICollection<>);
        private static Dictionary<Type, Type[]> genericCollectionTypeCache;

        private static readonly Type genericMixedArrayTypeDefinition = typeof(IMixedArray<>);
        private static Dictionary<Type, Type[]> genericMixedArrayTypeCache;

        static MappingUtils()
        {
            genericDictionaryTypeCache = new Dictionary<Type, Type[]>();
            genericCollectionTypeCache = new Dictionary<Type, Type[]>();
            genericMixedArrayTypeCache = new Dictionary<Type, Type[]>();
        }

        /// <summary>
        /// Invokes a generic specialization of the method referred to by a delegate.
        /// </summary>
        /// <param name="genericTypeArgs">The generic type arguments of the method referred to by the delegate</param>
        /// <param name="d">A delegate referring to a particular specialization of the method</param>
        /// <param name="args">The arguments for the delegate</param>
        /// <returns>The result</returns>
        public static object InvokeGenericMethod(Type[] genericTypeArgs, Delegate d, object[] args)
        {
            MethodInfo method = d.Method.GetGenericMethodDefinition().MakeGenericMethod(genericTypeArgs);
            return method.Invoke(d.Target, args);
        }

        /// <summary>
        /// Determines if the specified type is a generic dictionary.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type implements <see cref="IDictionary{TKey, TValue}" /></returns>
        public static bool IsGenericDictionary(Type type)
        {
            return GetGenericDictionaryTypeArgs(type) != null;
        }

        /// <summary>
        /// Gets the type arguments of the generic dictionary interface implemented by the type, or null if none.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>The type arguments of the <see cref="IDictionary{TKey, TValue}" /> interface</returns>
        public static Type[] GetGenericDictionaryTypeArgs(Type type)
        {
            return GetGenericTypeArgs(type, genericDictionaryTypeDefinition, genericDictionaryTypeCache);
        }

        /// <summary>
        /// Determines if the specified type is a generic collection.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type implements <see cref="ICollection{T}" /></returns>
        public static bool IsGenericCollection(Type type)
        {
            return GetGenericCollectionTypeArgs(type) != null;
        }

        /// <summary>
        /// Gets the type arguments of the generic collection interface implemented by the type, or null if none.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>The type arguments of the <see cref="ICollection{T}" /> interface</returns>
        public static Type[] GetGenericCollectionTypeArgs(Type type)
        {
            return GetGenericTypeArgs(type, genericCollectionTypeDefinition, genericCollectionTypeCache);
        }

        /// <summary>
        /// Determines if the specified type is a generic mixed array.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type implements <see cref="IMixedArray{T}" /></returns>
        public static bool IsGenericMixedArray(Type type)
        {
            return GetGenericMixedArrayTypeArgs(type) != null;
        }

        /// <summary>
        /// Gets the type arguments of the generic mixed array interface implemented by the type, or null if none.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>The type arguments of the <see cref="IMixedArray{T}" /> interface</returns>
        public static Type[] GetGenericMixedArrayTypeArgs(Type type)
        {
            return GetGenericTypeArgs(type, genericMixedArrayTypeDefinition, genericMixedArrayTypeCache);
        }

        private static Type[] GetGenericTypeArgs(Type type, Type typeDefinition, Dictionary<Type, Type[]> cache)
        {
            lock (cache)
            {
                Type[] genericTypeArgs;
                if (!cache.TryGetValue(type, out genericTypeArgs))
                {
                    if (type.IsInterface && type.IsGenericType
                        && type.GetGenericTypeDefinition() == typeDefinition)
                    {
                        genericTypeArgs = type.GetGenericArguments();
                    }
                    else
                    {
                        foreach (Type interfaceType in type.GetInterfaces())
                        {
                            if (interfaceType.IsGenericType &&
                                interfaceType.GetGenericTypeDefinition() == typeDefinition)
                            {
                                genericTypeArgs = interfaceType.GetGenericArguments();
                                break;
                            }
                        }
                    }

                    cache.Add(type, genericTypeArgs);
                }

                return genericTypeArgs;
            }
        }
    }
}
