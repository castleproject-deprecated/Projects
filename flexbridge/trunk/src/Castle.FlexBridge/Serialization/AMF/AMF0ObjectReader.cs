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
    /// Formats and reads AMF0 data.
    /// The reader maintains a cache of object references so it must be reset between
    /// each <see cref="AMFBody" /> in a message.
    /// </summary>
    /// <remarks>
    /// The AMF0 object reader delegates to the AMF3 object reader when AMF3 data
    /// is encountered thus forming a Chain of Responsibility.
    /// </remarks>
    internal class AMF0ObjectReader : IAMFObjectReader
    {
        private const string ExceptionPrefix = "AMF0 Object Deserialization Error: ";
        private AMFDataInput input;

        private AMF3ObjectReader amf3ObjectReader;
        private bool isAMF3;

        private List<IASValue> objectReferenceCache;

        public AMF0ObjectReader(AMFDataInput input)
        {
            this.input = input;

            amf3ObjectReader = new AMF3ObjectReader(input);
            objectReferenceCache = new List<IASValue>();
        }

        public AMFObjectEncoding ObjectEncoding
        {
            get { return isAMF3 ? amf3ObjectReader.ObjectEncoding : AMFObjectEncoding.AMF0; }
        }

        public void Reset()
        {
            isAMF3 = false;
            objectReferenceCache.Clear();

            amf3ObjectReader.Reset();
        }

        public IASValue ReadObject()
        {
            if (! isAMF3)
            {
                // Decide what to do based on the type code.
                AMF0ObjectTypeCode typeCode = (AMF0ObjectTypeCode)input.ReadByte();
                switch (typeCode)
                {
                    case AMF0ObjectTypeCode.AMF3Data:
                        isAMF3 = true;
                        return amf3ObjectReader.ReadObject();

                    case AMF0ObjectTypeCode.Array:
                        return ReadArray();

                    case AMF0ObjectTypeCode.Boolean:
                        return ReadBoolean();

                    case AMF0ObjectTypeCode.Date:
                        return ReadDate();

                    case AMF0ObjectTypeCode.LongString:
                        return ReadLongString();

                    case AMF0ObjectTypeCode.MixedArray:
                        return ReadMixedArray();

                    case AMF0ObjectTypeCode.Null:
                        return null;

                    case AMF0ObjectTypeCode.Number:
                        return ReadNumber();

                    case AMF0ObjectTypeCode.Object:
                        return ReadObjectValue();

                    case AMF0ObjectTypeCode.Reference:
                        return ReadReference();

                    case AMF0ObjectTypeCode.ShortString:
                        return ReadShortString();

                    case AMF0ObjectTypeCode.TypedObject:
                        return ReadTypedObject();

                    case AMF0ObjectTypeCode.Undefined:
                        return ASUndefined.Value;

                    case AMF0ObjectTypeCode.Unsupported:
                        return ASUnsupported.Value;

                    case AMF0ObjectTypeCode.Xml:
                        return ReadXml();

                    default:
                        throw new AMFException(String.Format(CultureInfo.CurrentCulture,
                            ExceptionPrefix + "Encountered unexpected or unsupported token '{0}'.", typeCode));
                }
            }

            // Delegate to AMF3 object reader.
            return amf3ObjectReader.ReadObject();
        }

        private IASValue ReadArray()
        {
            int length = input.ReadInt();
            if (length < 0)
                throw new AMFException(ExceptionPrefix + "Encountered negative Array length.");

            // Important: Add the array to the cache before deserializing its properties!
            ASArray result = ASArray.CreateUninitializedInstance();
            AddObjectToCache(result);

            IASValue[] indexedValues;

            // Read indexed values, if any.
            if (length != 0)
            {
                indexedValues = new IASValue[length];
                for (int i = 0; i < length; i++)
                    indexedValues[i] = ReadObject();
            }
            else
            {
                indexedValues = EmptyArray<IASValue>.Instance;
            }

            result.SetProperties(indexedValues, EmptyDictionary<string, IASValue>.Instance);
            return result;
        }

        private IASValue ReadBoolean()
        {
            return ASBoolean.ToASBoolean(input.ReadBoolean());
        }

        private IASValue ReadDate()
        {
            double millisecondsSinceEpoch = input.ReadDouble();
            int timezoneOffsetMinutes = input.ReadShort();

            return new ASDate(millisecondsSinceEpoch, timezoneOffsetMinutes);
        }

        private IASValue ReadLongString()
        {
            return new ASString(input.ReadLongString());
        }

        private IASValue ReadMixedArray()
        {
            int length = input.ReadInt();
            if (length < 0)
                throw new AMFException(ExceptionPrefix + "Encountered negative MixedArray length.");

            // Important: Add the array to the cache before deserializing its properties!
            ASArray result = ASArray.CreateUninitializedInstance();
            AddObjectToCache(result);

            IASValue[] indexedValues = length == 0 ? EmptyArray<IASValue>.Instance : new IASValue[length];
            IDictionary<string, IASValue> mixedValues = null;

            for (;;)
            {
                string key = input.ReadShortString();
                if (key.Length == 0)
                    break;

                IASValue value = input.ReadObject();

                // If the string looks like a valid index, then stuff it into the array.
                int index;
                if (int.TryParse(key, NumberStyles.None, CultureInfo.InvariantCulture, out index) && index >= 0 && index < length)
                {
                    indexedValues[index] = value;
                }
                else
                {
                    // Otherwise add it as a mixed value.
                    if (mixedValues == null)
                        mixedValues = new Dictionary<string, IASValue>();

                    mixedValues.Add(key, value);
                }
            }

            if (mixedValues == null)
                mixedValues = EmptyDictionary<string, IASValue>.Instance;

            ConsumeEndOfObject();

            result.SetProperties(indexedValues, mixedValues);
            return result;
        }

        private IASValue ReadObjectValue()
        {
            return ReadObjectWithClass(ASClass.UntypedDynamicClass);
        }

        private IASValue ReadNumber()
        {
            return new ASNumber(input.ReadDouble());
        }

        private IASValue ReadReference()
        {
            // Note: reference ids start with 1 in AMF0.
            int referenceId = input.ReadUnsignedShort();
            if (referenceId < 1 || referenceId > objectReferenceCache.Count)
                throw new AMFException(String.Format(CultureInfo.CurrentCulture,
                    ExceptionPrefix + "Encountered Reference token with an object reference id '{0}' that is out of bounds.", referenceId));

            return objectReferenceCache[referenceId - 1];
        }

        private IASValue ReadShortString()
        {
            return new ASString(input.ReadShortString());
        }

        private IASValue ReadTypedObject()
        {
            string classAlias = input.ReadShortString();

            return ReadObjectWithClass(ASClassCache.GetClass(classAlias, ASClassLayout.Dynamic, EmptyArray<string>.Instance));
        }

        private IASValue ReadXml()
        {
            string xmlString = input.ReadLongString();
            return new ASXmlDocument(xmlString);
        }

        private IASValue ReadObjectWithClass(ASClass classDefinition)
        {
            // Important: Add the object to the cache before deserializing its properties!
            ASObject result = ASObject.CreateUninitializedInstance(classDefinition);
            AddObjectToCache(result);

            IDictionary<string, IASValue> dynamicProperties;

            // Read dynamic properties, if any.
            string key = input.ReadShortString();
            if (key.Length != 0)
            {
                dynamicProperties = new Dictionary<string, IASValue>();

                for (; ; )
                {
                    IASValue value = input.ReadObject();
                    dynamicProperties.Add(key, value);

                    key = input.ReadShortString();
                    if (key.Length == 0)
                        break;
                }
            }
            else
            {
                dynamicProperties = EmptyDictionary<string, IASValue>.Instance;
            }

            ConsumeEndOfObject();

            result.SetProperties(EmptyArray<IASValue>.Instance, dynamicProperties);
            return result;
        }

        /// <summary>
        /// Ensures that the next byte is an EndOfObject type code and consumes it.
        /// </summary>
        private void ConsumeEndOfObject()
        {
            AMF0ObjectTypeCode typeCode = (AMF0ObjectTypeCode) input.ReadByte();
            if (typeCode != AMF0ObjectTypeCode.EndOfObject)
                throw new AMFException(String.Format(CultureInfo.CurrentCulture,
                    ExceptionPrefix + "Expected EndOfObject token but encountered '{0}' instead.", typeCode));
        }

        /// <summary>
        /// Adds the specified object to the reference cache.
        /// </summary>
        /// <param name="obj">The object to cache</param>
        private void AddObjectToCache(IASValue obj)
        {
            objectReferenceCache.Add(obj);
        }
    }
}
