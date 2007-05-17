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

namespace Castle.FlexBridge.Serialization.AMF
{
    /// <summary>
    /// Formats and reads AMF3 data.
    /// The reader maintains a cache of object references so it must be reset between
    /// each <see cref="AMFBody" /> in a message.
    /// </summary>
    internal class AMF3ObjectReader : IAMFObjectReader
    {
        private const string ExceptionPrefix = "AMF3 Object Deserialization Error: ";
        private AMFDataInput input;

        private List<CacheItem> objectReferenceCache;
        private List<string> stringReferenceCache;

        public AMF3ObjectReader(AMFDataInput input)
        {
            this.input = input;

            objectReferenceCache = new List<CacheItem>();
            stringReferenceCache = new List<string>();
        }

        public AMFObjectEncoding ObjectEncoding
        {
            get { return AMFObjectEncoding.AMF3; }
        }

        public void Reset()
        {
            objectReferenceCache.Clear();
            stringReferenceCache.Clear();
        }

        public IASValue ReadObject()
        {
            // Decide what to do based on the type code.
            AMF3ObjectTypeCode typeCode = (AMF3ObjectTypeCode)input.ReadByte();
            switch (typeCode)
            {
                case AMF3ObjectTypeCode.Array:
                    return ReadArray();

                case AMF3ObjectTypeCode.ByteArray:
                    return ReadByteArray();

                case AMF3ObjectTypeCode.Date:
                    return ReadDate();

                case AMF3ObjectTypeCode.False:
                    return ASBoolean.False;

                case AMF3ObjectTypeCode.Integer:
                    return ReadInteger();

                case AMF3ObjectTypeCode.Null:
                    return null;

                case AMF3ObjectTypeCode.Number:
                    return ReadNumber();

                case AMF3ObjectTypeCode.Object:
                    return ReadObjectValue();

                case AMF3ObjectTypeCode.String:
                    return ReadString();

                case AMF3ObjectTypeCode.True:
                    return ASBoolean.True;

                case AMF3ObjectTypeCode.Undefined:
                    return ASUndefined.Value;

                case AMF3ObjectTypeCode.Xml:
                    return ReadXml();

                default:
                    throw new AMFException(String.Format(CultureInfo.CurrentCulture, "Encountered unexpected or unsupported AMF3 object type code '{0}'.", typeCode));
            }
        }

        private IASValue ReadArray()
        {
            int bits = input.ReadVWInt29();
            int lengthOrReferenceId = bits >> 1;

            // Handle cached objects.
            if ((bits & 1) == 0)
            {
                return GetObjectFromCache(AMF3ObjectTypeCode.Array, lengthOrReferenceId);
            }

            // Read out the whole array.
            if (lengthOrReferenceId < 0)
                throw new AMFException(String.Format(CultureInfo.CurrentCulture,
                    ExceptionPrefix + "Encountered Array token with invalid length '{0}'.", lengthOrReferenceId));

            // Important: Add the array to the cache before deserializing its properties!
            ASArray result = ASArray.CreateUninitializedInstance();
            AddObjectToCache(AMF3ObjectTypeCode.Array, result);

            // Read mixed values if any.
            IDictionary<string, IASValue> mixedValues;

            string key = ReadStringData();
            if (key.Length != 0)
            {
                mixedValues = new Dictionary<string, IASValue>();

                for (; ; )
                {
                    IASValue value = ReadObject();
                    mixedValues.Add(key, value);

                    key = ReadStringData();
                    if (key.Length == 0)
                        break;
                }
            }
            else
            {
                mixedValues = EmptyDictionary<string, IASValue>.Instance;
            }

            // Read indexed values if any.
            IASValue[] indexedValues;

            if (lengthOrReferenceId != 0)
            {
                indexedValues = new IASValue[lengthOrReferenceId];

                for (int i = 0; i < lengthOrReferenceId; i++)
                    indexedValues[i] = ReadObject();
            }
            else
            {
                indexedValues = EmptyArray<IASValue>.Instance;
            }

            result.SetProperties(indexedValues, mixedValues);
            return result;
        }

        private IASValue ReadByteArray()
        {
            int bits = input.ReadVWInt29();
            int lengthOrReferenceId = bits >> 1;

            // Handle cached objects.
            if ((bits & 1) == 0)
            {
                return GetObjectFromCache(AMF3ObjectTypeCode.ByteArray, lengthOrReferenceId);
            }

            // Read out the whole byte array.
            if (lengthOrReferenceId < 0)
                throw new AMFException(String.Format(CultureInfo.CurrentCulture,
                    ExceptionPrefix + "Encountered ByteArray token with invalid length '{0}'.", lengthOrReferenceId));

            byte[] bytes;

            if (lengthOrReferenceId != 0)
            {
                bytes = new byte[lengthOrReferenceId];
                input.ReadBytes(bytes, 0, lengthOrReferenceId);
            }
            else
            {
                bytes = EmptyArray<byte>.Instance;
            }

            ASByteArray result = new ASByteArray(bytes);
            AddObjectToCache(AMF3ObjectTypeCode.ByteArray, result);
            return result;
        }

        private IASValue ReadDate()
        {
            int bits = input.ReadVWInt29();

            // Handle cached objects.
            if ((bits & 1) == 0)
            {
                int referenceId = bits >> 1;
                return GetObjectFromCache(AMF3ObjectTypeCode.Date, referenceId);
            }

            // Read out the whole date.
            double millisecondsSinceEpoch = input.ReadDouble();

            ASDate result = new ASDate(millisecondsSinceEpoch, 0);
            AddObjectToCache(AMF3ObjectTypeCode.Date, result);
            return result;
        }

        private IASValue ReadInteger()
        {
            return new ASInt29(input.ReadVWInt29());
        }

        private IASValue ReadObjectValue()
        {
            int bits = input.ReadVWInt29();

            // Handle cached objects.
            if ((bits & 1) == 0)
            {
                int referenceId = bits >> 1;
                return GetObjectFromCache(AMF3ObjectTypeCode.Object, referenceId);
            }

            // Handle cached class definitions.
            ASClass classDefinition;

            if ((bits & 2) == 0)
            {
                int referenceId = bits >> 2;

                // Note: Object might be an ASExternalizableObject.
                IASValue obj = GetObjectFromCache(AMF3ObjectTypeCode.Object, referenceId);
                classDefinition = obj.Class;
            }
            else
            {
                // Read the class definition.
                ASClassLayout classLayout = (ASClassLayout) ((bits & 0x0c) >> 2);
                if (classLayout > ASClassLayout.Dynamic)
                    throw new AMFException(String.Format(CultureInfo.CurrentCulture,
                        ExceptionPrefix + "Encountered Object token with invalid class layout '{0}'.", classLayout));

                int memberCount = bits >> 4;
                if (memberCount < 0)
                    throw new AMFException(String.Format(CultureInfo.CurrentCulture,
                        ExceptionPrefix + "Encountered Object token with invalid member count '{0}'.", memberCount));

                if (classLayout == ASClassLayout.Externalizable && memberCount != 0)
                    throw new AMFException(String.Format(CultureInfo.CurrentCulture,
                        ExceptionPrefix + "Encountered Object token with Externalizable class layout and non-zero member count '{0}'.", memberCount));

                string classAlias = ReadStringData();

                string[] memberNames;
                if (memberCount != 0)
                {
                    memberNames = new string[memberCount];

                    for (int i = 0; i < memberCount; i++)
                        memberNames[i] = ReadStringData();
                }
                else
                {
                    memberNames = EmptyArray<string>.Instance;
                }

                // Look up the class in the cache.
                classDefinition = ASClassCache.GetClass(classAlias, classLayout, memberNames);
            }

            // Read the instance data.
            if (classDefinition.Layout == ASClassLayout.Externalizable)
            {
                // Important: Add the object to the cache before deserializing its properties!
                ASExternalizableObject result = ASExternalizableObject.CreateUninitializedInstance(classDefinition);
                AddObjectToCache(AMF3ObjectTypeCode.Object, result);

                // Use custom serialization for the externalizable object.
                IExternalizable externalizableValue = input.Serializer.CreateExternalizableInstance(classDefinition.ClassAlias);
                externalizableValue.ReadExternal(input);

                result.SetProperties(externalizableValue);
                return result;
            }
            else
            {
                // Important: Add the object to the cache before deserializing its properties!
                ASObject result = ASObject.CreateUninitializedInstance(classDefinition);
                AddObjectToCache(AMF3ObjectTypeCode.Object, result);

                // Read the member values.
                int memberCount = classDefinition.MemberNames.Count;
                IASValue[] memberValues;

                if (memberCount != 0)
                {
                    memberValues = new IASValue[memberCount];

                    for (int i = 0; i < memberCount; i++)
                        memberValues[i] = ReadObject();
                }
                else
                {
                    memberValues = EmptyArray<IASValue>.Instance;
                }

                // Read the dynamic property values.
                IDictionary<string, IASValue> dynamicProperties;

                if (classDefinition.Layout == ASClassLayout.Dynamic)
                {
                    string key = ReadStringData();
                    if (key.Length != 0)
                    {
                        dynamicProperties = new Dictionary<string, IASValue>();

                        for (; ; )
                        {
                            IASValue value = ReadObject();
                            dynamicProperties.Add(key, value);

                            key = ReadStringData();
                            if (key.Length == 0)
                                break;
                        }
                    }
                    else
                    {
                        dynamicProperties = EmptyDictionary<string, IASValue>.Instance;
                    }
                }
                else
                {
                    dynamicProperties = EmptyDictionary<string, IASValue>.Instance;
                }

                result.SetProperties(memberValues, dynamicProperties);
                return result;
            }
        }

        private IASValue ReadNumber()
        {
            return new ASNumber(input.ReadDouble());
        }

        private IASValue ReadString()
        {
            return new ASString(ReadStringData());
        }

        private IASValue ReadXml()
        {
            string xmlString = ReadStringData();
            return new ASXmlDocument(xmlString);
        }

        /// <summary>
        /// Reads AMF3 string data.
        /// </summary>
        /// <returns>The string that was read</returns>
        private string ReadStringData()
        {
            int bits = input.ReadVWInt29();
            int lengthOrReferenceId = bits >> 1;

            // Handle cached strings.
            if ((bits & 1) == 0)
            {
                if (lengthOrReferenceId < 0 || lengthOrReferenceId >= stringReferenceCache.Count)
                    throw new AMFException(String.Format(CultureInfo.CurrentCulture,
                        ExceptionPrefix + "Encountered string data with a string reference id '{0}' that is out of bounds.", lengthOrReferenceId));

                return stringReferenceCache[lengthOrReferenceId];
            }

            // Read out the whole string.
            if (lengthOrReferenceId < 0)
                throw new AMFException(String.Format(CultureInfo.CurrentCulture,
                    ExceptionPrefix + "Encountered string data with invalid length '{0}'.", lengthOrReferenceId));

            string result = input.ReadUTFBytes(lengthOrReferenceId);

            // Add non-empty strings to the cache.
            if (result.Length != 0)
                stringReferenceCache.Add(result);

            return result;
        }

        /// <summary>
        /// Gets an object from the cache.
        /// </summary>
        /// <param name="objectType">The type of object expected</param>
        /// <param name="referenceId">The object reference id</param>
        /// <returns>The cached object</returns>
        private IASValue GetObjectFromCache(AMF3ObjectTypeCode objectType, int referenceId)
        {
            if (referenceId < 0 || referenceId >= objectReferenceCache.Count)
                throw new AMFException(String.Format(CultureInfo.CurrentCulture,
                    ExceptionPrefix + "Encountered {0} token with an object reference id '{1}' that is out of bounds.", objectType, referenceId));

            CacheItem cacheItem = objectReferenceCache[referenceId];
            if (cacheItem.ObjectType != objectType)
                throw new AMFException(String.Format(CultureInfo.CurrentCulture,
                    ExceptionPrefix + "Encountered {0} token with an object reference id '{1}' that refers to an object of a different type than expected.", objectType, referenceId));

            return cacheItem.Object;
        }

        /// <summary>
        /// Adds the specified object to the reference cache.
        /// </summary>
        /// <param name="objectType">The type of object to cache</param>
        /// <param name="obj">The object to cache</param>
        private void AddObjectToCache(AMF3ObjectTypeCode objectType, IASValue obj)
        {
            objectReferenceCache.Add(new CacheItem(objectType, obj));
        }

        private struct CacheItem
        {
            public CacheItem(AMF3ObjectTypeCode objectType, IASValue obj)
            {
                this.ObjectType = objectType;
                this.Object = obj;
            }

            public readonly AMF3ObjectTypeCode ObjectType;
            public readonly IASValue Object;
        }
    }
}