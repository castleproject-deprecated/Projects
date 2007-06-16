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
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Collections;

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// Provides mappers for one-dimensional arrays.
    /// </summary>
    public sealed class ArrayMapperFactory : BaseASMapperFactory
    {
        private delegate ArrayMapper<T> GetMapperInstanceDelegate<T>();
        private GetMapperInstanceDelegate<object> getMapperInstance;

        /// <summary>
        /// Creates an array mapper factory.
        /// </summary>
        public ArrayMapperFactory()
        {
            getMapperInstance = GetMapperInstance<object>;
        }

        /// <inheritdoc />
        public override IASSourceMapper GetASSourceMapper(ASSourceMappingDescriptor descriptor)
        {
            if ((descriptor.SourceKind == ASTypeKind.Array || descriptor.SourceKind == ASTypeKind.ByteArray)
                && (descriptor.SourceContentFlags & ASValueContentFlags.HasDynamicProperties) == 0
                && descriptor.TargetNativeType.IsArray)
            {
                Type[] genericTypes = new Type[] { descriptor.TargetNativeType.GetElementType() };
                return (IASSourceMapper) MappingUtils.InvokeGenericMethod(genericTypes, getMapperInstance, EmptyArray<object>.Instance);
            }

            return null;
        }

        /// <inheritdoc />
        public override IASTargetMapper GetASTargetMapper(ASTargetMappingDescriptor descriptor)
        {
            if (descriptor.SourceNativeType.IsArray && descriptor.SourceNativeType.GetArrayRank() == 1)
            {
                Type[] genericTypes = new Type[] { descriptor.SourceNativeType.GetElementType() };
                return (IASTargetMapper)MappingUtils.InvokeGenericMethod(genericTypes, getMapperInstance, EmptyArray<object>.Instance);
            }

            return null;
        }

        private static ArrayMapper<T> GetMapperInstance<T>()
        {
            return ArrayMapper<T>.Instance;
        }

        /// <summary>
        /// A mapper specialized for the array type.
        /// </summary>
        /// <typeparam name="T">The array element type</typeparam>
        private sealed class ArrayMapper<T> : BaseASMapper, IASNativeArrayMapper
        {
            public static readonly ArrayMapper<T> Instance = new ArrayMapper<T>();

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                // Handle byte arrays specially.
                byte[] bytes = nativeValue as byte[];
                if (bytes != null)
                    return new ASByteArray(bytes);

                // Handle other types.
                return new ASNativeArray(nativeValue, this, false);
            }

            protected override object MapArrayToNative(IActionScriptSerializer serializer, Type nativeType,
                int indexedLength, IEnumerable<IASValue> indexedValues,
                IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties)
            {
                if (dynamicProperties.GetEnumerator().MoveNext())
                    throw new ActionScriptException("Cannot map an ActionScript array with dynamic properties to a native Array type.");

                T[] array = new T[indexedLength];

                // Add indexed values.
                int index = 0;
                foreach (IASValue indexedValue in indexedValues)
                {
                    T value = (T)serializer.ToNative(indexedValue, typeof(T));
                    array[index++] = value;
                }

                return array;
            }

            protected override object MapByteArrayToNative(IActionScriptSerializer serializer, Type nativeType,
                int length, IEnumerable<ArraySegment<byte>> segments)
            {
                // Map to a byte array if that's what we asked for or if we asked for
                // a supertype of byte[], like object[] or Array.
                if (nativeType.IsAssignableFrom(typeof(byte[])))
                {
                    byte[] bytes = new byte[length];

                    int offset = 0;
                    foreach (ArraySegment<byte> segment in segments)
                    {
                        Array.Copy(segment.Array, segment.Offset, bytes, offset, segment.Count);
                        offset += segment.Count;
                    }

                    return bytes;
                }
                else
                {
                    // Otherwise convert the elements one at a time. 
                    T[] array = new T[length];

                    int elementIndex = 0;
                    foreach (ArraySegment<byte> segment in segments)
                    {
                        for (int i = 0; i < segment.Count; i++)
                        {
                            T elementValue = (T)serializer.ToNative(new ASInt29(segment.Array[i]), typeof(T));
                            array[elementIndex++] = elementValue;
                        }
                    }

                    return array;
                }
            }

            void IASNativeArrayMapper.AcceptVisitor(IActionScriptSerializer serializer, object nativeArray, IASValueVisitor visitor)
            {
                // We can assume this isn't a byte array since we checked that earlier.
                T[] array = (T[])nativeArray;

                visitor.VisitArray(serializer, array.Length, GetIndexedValues(serializer, array), EmptyDictionary<string, IASValue>.Instance);
            }

            ASValueContentFlags IASNativeArrayMapper.GetContentFlags(object nativeArray)
            {
                T[] array = (T[])nativeArray;

                return array.Length != 0 ? ASValueContentFlags.HasIndexedValues : ASValueContentFlags.None;
            }

            private static IEnumerable<IASValue> GetIndexedValues(IActionScriptSerializer serializer, T[] array)
            {
                foreach (T element in array)
                {
                    yield return serializer.ToASValue(element);
                }
            }
        }
    }
}