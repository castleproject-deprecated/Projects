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
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Collections;
using Castle.FlexBridge.Serialization.Factories;

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// Provides mappers for implementations of <see cref="ICollection{T}" />.
    /// New generic collection instances are created by consulting a list of <see cref="IGenericCollectionFactory" />.
    /// </summary>
    public sealed class GenericCollectionMapperFactory : BaseASMapperFactory
    {
        private List<IGenericCollectionFactory> factories;

        private delegate GenericCollectionMapper<T> GetMapperInstanceDelegate<T>(Type collectionType);
        private GetMapperInstanceDelegate<object> getMapperInstance;

        /// <summary>
        /// Creates a generic collection mapper factory.
        /// </summary>
        public GenericCollectionMapperFactory()
        {
            factories = new List<IGenericCollectionFactory>();
            getMapperInstance = GetMapperInstance<object>;

            // TODO: This is not the right place for registering built-ins!
            factories.Add(GenericListFactory.Instance);
            factories.Add(GenericLinkedListFactory.Instance);
        }

        /// <summary>
        /// Adds a factory to the list consulted when deserializing ActionScript values.
        /// </summary>
        /// <param name="factory">The factory to add</param>
        public void AddFactory(IGenericCollectionFactory factory)
        {
            lock (factories)
                factories.Add(factory);
        }

        /// <inheritdoc />
        public override IASSourceMapper GetASSourceMapper(ASSourceMappingDescriptor descriptor)
        {
            if ((descriptor.SourceKind == ASTypeKind.Array || descriptor.SourceKind == ASTypeKind.ByteArray)
                && (descriptor.SourceContentFlags & ASValueContentFlags.HasDynamicProperties) == 0)
            {
                Type[] genericTypeArgs = MappingUtils.GetGenericCollectionTypeArgs(descriptor.TargetNativeType);
                if (genericTypeArgs != null)
                {
                    return (IASSourceMapper)MappingUtils.InvokeGenericMethod(genericTypeArgs,
                        getMapperInstance, new object[] { descriptor.TargetNativeType });
                }
            }

            return null;
        }

        /// <inheritdoc />
        public override IASTargetMapper GetASTargetMapper(ASTargetMappingDescriptor descriptor)
        {
            // Note: We don't want to map arrays or dictionaries here.  Leave that up to a
            //       specialized implementation instead.
            if (!descriptor.SourceNativeType.IsArray
                && !MappingUtils.IsGenericDictionary(descriptor.SourceNativeType))
            {
                Type[] genericTypeArgs = MappingUtils.GetGenericCollectionTypeArgs(descriptor.SourceNativeType);
                if (genericTypeArgs != null)
                {
                    return (IASTargetMapper)MappingUtils.InvokeGenericMethod(genericTypeArgs, getMapperInstance,
                        new object[] { null });
                }
            }

            return null;
        }

        private GenericCollectionMapper<T> GetMapperInstance<T>(Type collectionType)
        {
            IGenericCollectionFactory factory;
            if (collectionType != null)
            {
                factory = GetFactory<T>(collectionType);
                if (factory == null)
                    return null; // Cannot produce instances of this collection type
            }
            else
            {
                factory = null;
            }

            return new GenericCollectionMapper<T>(collectionType, factory);
        }

        private IGenericCollectionFactory GetFactory<T>(Type collectionType)
        {
            lock (factories)
            {
                foreach (IGenericCollectionFactory factory in factories)
                {
                    if (factory.CanCreateInstance<T>(collectionType))
                        return factory;
                }
            }

            return null;
        }

        /// <summary>
        /// A mapper mapper specialized for the collection type.
        /// </summary>
        /// <typeparam name="T">The collection value type</typeparam>
        private sealed class GenericCollectionMapper<T> : BaseASMapper, IASNativeArrayMapper
        {
            private Type collectionType;
            private IGenericCollectionFactory collectionFactory;

            public GenericCollectionMapper(Type collectionType, IGenericCollectionFactory collectionFactory)
            {
                this.collectionType = collectionType;
                this.collectionFactory = collectionFactory;
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
                if (dynamicProperties.GetEnumerator().MoveNext())
                    throw new ActionScriptException("Cannot map an ActionScript array with dynamic properties to a native Array type.");

                ICollection<T> collection = CreateCollectionInstance(indexedLength);

                // Add indexed values.
                foreach (IASValue indexedValue in indexedValues)
                {
                    T value = (T)serializer.ToNative(indexedValue, typeof(T));
                    collection.Add(value);
                }

                return collection;
            }

            protected override object MapByteArrayToNative(IActionScriptSerializer serializer, Type nativeType,
                int length, IEnumerable<ArraySegment<byte>> segments)
            {
                ICollection<T> collection = CreateCollectionInstance(length);

                if (typeof(T) == typeof(byte))
                {
                    ICollection<byte> byteCollection = (ICollection<byte>)collection;

                    foreach (ArraySegment<byte> segment in segments)
                    {
                        for (int i = 0; i < segment.Count; i++)
                        {
                            byteCollection.Add(segment.Array[i + segment.Offset]);
                        }
                    }
                }
                else
                {
                    foreach (ArraySegment<byte> segment in segments)
                    {
                        for (int i = 0; i < segment.Count; i++)
                        {
                            IASValue byteValue = new ASInt29(segment.Array[i + segment.Offset]);
                            T value = (T)serializer.ToNative(byteValue, typeof(T));
                            collection.Add(value);
                        }
                    }
                }

                return collection;
            }

            private ICollection<T> CreateCollectionInstance(int initialCapacity)
            {
                return collectionFactory.CreateInstance<T>(collectionType, initialCapacity);
            }

            void IASNativeArrayMapper.AcceptVisitor(IActionScriptSerializer serializer, object nativeArray, IASValueVisitor visitor)
            {
                ICollection<T> collection = (ICollection<T>) nativeArray;

                visitor.VisitArray(serializer, collection.Count, GetIndexedValues(serializer, collection), EmptyDictionary<string, IASValue>.Instance);
            }

            ASValueContentFlags IASNativeArrayMapper.GetContentFlags(object nativeArray)
            {
                ICollection<T> collection = (ICollection<T>)nativeArray;

                return collection.Count != 0 ? ASValueContentFlags.HasIndexedValues : ASValueContentFlags.None;
            }

            private static IEnumerable<IASValue> GetIndexedValues(IActionScriptSerializer serializer, ICollection<T> collection)
            {
                foreach (T element in collection)
                {
                    yield return serializer.ToASValue(element);
                }
            }
        }
    }
}