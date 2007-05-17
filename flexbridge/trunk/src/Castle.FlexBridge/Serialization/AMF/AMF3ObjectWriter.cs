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
using Castle.FlexBridge.Collections;
using Castle.FlexBridge.Serialization;
using Castle.FlexBridge.Serialization.AMF;
using Castle.FlexBridge.ActionScript;

namespace Castle.FlexBridge.Serialization.AMF
{
    /// <summary>
    /// Formats and writes AMF3 data.
    /// The writer maintains a cache of object references so it must be reset between
    /// each <see cref="AMFBody" /> in a message.
    /// </summary>
    internal class AMF3ObjectWriter : IAMFObjectWriter, IASValueVisitor
    {
        private const string ExceptionPrefix = "AMF3 Object Serialization Error: ";

        /// <summary>
        /// The maximum admissible length of an array.
        /// </summary>
        private const int MaxArrayLength = ASConstants.Int29MaxValue / 2;

        private AMFDataOutput output;

        private Dictionary<ASClass, int> classDefinitionCache;
        private Dictionary<IASValue, CacheItem> objectReferenceCache;
        private Dictionary<string, int> stringReferenceCache;

        private IASValue currentValue;

        public AMF3ObjectWriter(AMFDataOutput output)
        {
            this.output = output;

            classDefinitionCache = new Dictionary<ASClass, int>(ReferenceEqualityComparer<ASClass>.Instance);
            objectReferenceCache = new Dictionary<IASValue, CacheItem>(ReferenceEqualityComparer<IASValue>.Instance);
            stringReferenceCache = new Dictionary<string, int>(StringComparer.Ordinal);
        }

        public AMFObjectEncoding ObjectEncoding
        {
            get { return AMFObjectEncoding.AMF0; }
        }

        public void Reset()
        {
            classDefinitionCache.Clear();
            objectReferenceCache.Clear();
            stringReferenceCache.Clear();

            currentValue = null;
        }

        public void WriteObjectStreamPreamble()
        {
            // Assume decoder already starts in AMF0 mode by default.
            // Write the code to transition to AMF3.
            output.WriteByte((byte)AMF0ObjectTypeCode.AMF3Data);
        }

        public void WriteObject(IASValue value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }

            // Check if we have seen the object reference before.
            CacheItem cacheItem;
            if (objectReferenceCache.TryGetValue(value, out cacheItem))
            {
                WriteCachedValue(cacheItem);
                return;
            }

            // Otherwise write it out fresh.
            currentValue = value;
            value.AcceptVisitor(output.Serializer, this);
            currentValue = null;
        }

        void IASValueVisitor.VisitNull(IActionScriptSerializer serializer)
        {
            WriteNull();
        }

        void IASValueVisitor.VisitUndefined(IActionScriptSerializer serializer)
        {
            output.WriteByte((byte)AMF3ObjectTypeCode.Undefined);
        }

        void IASValueVisitor.VisitUnsupported(IActionScriptSerializer serializer)
        {
            throw new AMFException(ExceptionPrefix + "Cannot write an Unsupported value to an AMF3 stream.");
        }

        void IASValueVisitor.VisitBoolean(IActionScriptSerializer serializer, bool value)
        {
            output.WriteByte(value ? (byte)AMF3ObjectTypeCode.True : (byte)AMF3ObjectTypeCode.False);
        }

        void IASValueVisitor.VisitNumber(IActionScriptSerializer serializer, double value)
        {
            output.WriteByte((byte)AMF3ObjectTypeCode.Number);
            output.WriteDouble(value);
        }

        void IASValueVisitor.VisitInt29(IActionScriptSerializer serializer, int value)
        {
            output.WriteByte((byte)AMF3ObjectTypeCode.Integer);
            output.WriteVWInt29(value);
        }

        void IASValueVisitor.VisitDate(IActionScriptSerializer serializer, double millisecondsSinceEpoch, int timeZoneOffsetMinutes)
        {
            AddCurrentValueToCache(AMF3ObjectTypeCode.Date);

            output.WriteByte((byte)AMF3ObjectTypeCode.Date);
            output.WriteVWInt29(1);
            output.WriteDouble(millisecondsSinceEpoch - timeZoneOffsetMinutes * 60000);
        }

        void IASValueVisitor.VisitString(IActionScriptSerializer serializer, string value)
        {
            output.WriteByte((byte)AMF3ObjectTypeCode.String);
            WriteStringData(value);
        }

        void IASValueVisitor.VisitByteArray(IActionScriptSerializer serializer, int length, IEnumerable<ArraySegment<byte>> segments)
        {
            if (length > MaxArrayLength)
                throw new AMFException(ExceptionPrefix + "Cannot serialize a byte array longer than " + MaxArrayLength + " bytes.");

            AddCurrentValueToCache(AMF3ObjectTypeCode.ByteArray);
            output.WriteByte((byte)AMF3ObjectTypeCode.ByteArray);
            output.WriteVWInt29(length * 2 + 1);

            if (length != 0)
            {
                int bytesRemaining = length;

                foreach (ArraySegment<byte> bytes in segments)
                {
                    if (bytesRemaining < bytes.Count)
                        throw new AMFException(ExceptionPrefix + "The byte array provided more bytes than were indicated by its length.");

                    bytesRemaining -= bytes.Count;
                    output.WriteBytes(bytes.Array, bytes.Offset, bytes.Count);
                }

                if (bytesRemaining != 0)
                    throw new AMFException(ExceptionPrefix + "The byte array did not provide as many bytes as were indicated by its length.");
            }
        }

        void IASValueVisitor.VisitArray(IActionScriptSerializer serializer, int indexedLength, IEnumerable<IASValue> indexedValues,
            IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties)
        {
            AddCurrentValueToCache(AMF3ObjectTypeCode.Array);

            if (indexedLength > MaxArrayLength)
                throw new AMFException(ExceptionPrefix + "Cannot serialize an array longer than " + MaxArrayLength + " elements.");

            output.WriteByte((byte)AMF3ObjectTypeCode.Array);
            output.WriteVWInt29(indexedLength * 2 + 1);

            // Write dynamic properties as key/value pairs.
            foreach (KeyValuePair<string, IASValue> pair in dynamicProperties)
            {
                if (String.IsNullOrEmpty(pair.Key))
                    throw new AMFException(ExceptionPrefix + "Cannot serialize an array with a null or empty string key.");

                WriteStringData(pair.Key);
                WriteObject(pair.Value);
            }

            // Terminate with empty string.
            WriteStringData("");

            // Write indexed values in sequence.
            int index = 0;
            foreach (IASValue element in indexedValues)
            {
                if (index == indexedLength)
                    throw new AMFException(ExceptionPrefix + "The number of indexed values provided by the array is greater than was indicated by its length.");

                WriteObject(element);
                index += 1;
            }

            if (index != indexedLength)
                throw new AMFException(ExceptionPrefix + "The number of indexed values provided by the array is less than was indicated by its length.");
        }

        void IASValueVisitor.VisitObject(IActionScriptSerializer serializer, ASClass @class, IEnumerable<IASValue> memberValues,
            IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties,
            IExternalizable externalizableValue)
        {
            int objectReferenceId = AddCurrentValueToCache(AMF3ObjectTypeCode.Object);

            output.WriteByte((byte)AMF3ObjectTypeCode.Object);

            // Look for a suitable existing class definition in the cache.
            int classDefinitionId;
            if (classDefinitionCache.TryGetValue(@class, out classDefinitionId))
            {
                // Use class definition reference.
                output.WriteVWInt29(classDefinitionId * 4 + 1);
            }
            else
            {
                // Include class definition inline and add it to the cache.
                classDefinitionCache.Add(@class, objectReferenceId);

                output.WriteVWInt29(@class.MemberNames.Count * 16 + (int)@class.Layout * 4 + 3);
                WriteStringData(@class.ClassAlias);

                foreach (string memberName in @class.MemberNames)
                {
                    if (String.IsNullOrEmpty(memberName))
                        throw new AMFException(ExceptionPrefix + "Cannot serialize an object with a null or empty member name.");

                    WriteStringData(memberName);
                }
            }

            if (@class.Layout == ASClassLayout.Externalizable)
            {
                if (externalizableValue == null)
                    throw new AMFException(ExceptionPrefix + "The class layout is Externalizable but the object does not provide an ExternalizableValue.");

                externalizableValue.WriteExternal(output);
            }
            else
            {
                // Write the members first as a sequence of values.
                int memberCount = @class.MemberNames.Count;
                int memberIndex = 0;
                foreach (IASValue memberValue in memberValues)
                {
                    if (memberIndex == memberCount)
                        throw new AMFException(ExceptionPrefix + "The number of member values provided by the object is greater than was indicated by its class.");

                    WriteObject(memberValue);
                    memberIndex += 1;
                }

                if (memberIndex != memberCount)
                    throw new AMFException(ExceptionPrefix + "The number of member values provided by the object is less than was indicated by its class.");

                if (@class.Layout == ASClassLayout.Dynamic)
                {
                    // Write dynamic key/value pairs.
                    foreach (KeyValuePair<string, IASValue> pair in dynamicProperties)
                    {
                        if (String.IsNullOrEmpty(pair.Key))
                            throw new AMFException(ExceptionPrefix + "Cannot serialize an object with a null or empty string key.");

                        WriteStringData(pair.Key);
                        WriteObject(pair.Value);
                    }

                    // Terminate with empty string.
                    WriteStringData("");
                }
            }
        }

        void IASValueVisitor.VisitXml(IActionScriptSerializer serializer, string xmlString)
        {
            output.WriteByte((byte)AMF3ObjectTypeCode.Xml);
            WriteStringData(xmlString);
        }

        /// <summary>
        /// Writes a string and manages the string reference cache.
        /// </summary>
        /// <param name="value"></param>
        private void WriteStringData(string value)
        {
            if (value.Length == 0)
            {
                WriteEmptyStringData();
                return;
            }

            int stringReferenceId;
            if (stringReferenceCache.TryGetValue(value, out stringReferenceId))
            {
                output.WriteVWInt29(stringReferenceId * 2);
            }
            else
            {
                stringReferenceCache.Add(value, stringReferenceCache.Count);

                output.WriteVWInt29StringWithLSBSetTo1(value);
            }
        }

        /// <summary>
        /// Writes an empty string.
        /// </summary>
        private void WriteEmptyStringData()
        {
            output.WriteVWInt29(1);
        }

        /// <summary>
        /// Writes a null.
        /// </summary>
        private void WriteNull()
        {
            output.WriteByte((byte)AMF3ObjectTypeCode.Null);
        }

        /// <summary>
        /// Adds the current unmapped value to the object reference cache.
        /// </summary>
        /// <param name="objectType"></param>
        private int AddCurrentValueToCache(AMF3ObjectTypeCode objectType)
        {
            CacheItem cacheItem = new CacheItem(objectType, objectReferenceCache.Count);
            objectReferenceCache.Add(currentValue, cacheItem);

            return cacheItem.ReferenceId;
        }

        /// <summary>
        /// Writes a cached item to the stream.
        /// </summary>
        /// <param name="cacheItem">The cache item to write</param>
        private void WriteCachedValue(CacheItem cacheItem)
        {
            output.WriteByte((byte) cacheItem.ObjectType);
            output.WriteVWInt29(cacheItem.ReferenceId * 2);
        }

        private struct CacheItem
        {
            public CacheItem(AMF3ObjectTypeCode objectType, int referenceId)
            {
                this.ObjectType = objectType;
                this.ReferenceId = referenceId;
            }

            public readonly AMF3ObjectTypeCode ObjectType;
            public readonly int ReferenceId;
        }
    }
}