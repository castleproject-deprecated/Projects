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
using System.Text;
using Castle.FlexBridge.Collections;
using Castle.FlexBridge.Serialization.AMF;
using Castle.FlexBridge.ActionScript;

namespace Castle.FlexBridge.Serialization.AMF
{
    /// <summary>
    /// Formats and writes AMF0 data.
    /// The writer maintains a cache of object references so it must be reset between
    /// each <see cref="AMFBody" /> in a message.
    /// </summary>
    internal class AMF0ObjectWriter : IAMFObjectWriter, IASValueVisitor
    {
        private const string ExceptionPrefix = "AMF0 Object Serialization Error: ";
        private AMFDataOutput output;

        private Dictionary<IASValue, int> referenceCache;
        private IASValue currentValue;

        public AMF0ObjectWriter(AMFDataOutput output)
        {
            this.output = output;

            referenceCache = new Dictionary<IASValue, int>(ReferenceEqualityComparer<IASValue>.Instance);
        }

        public AMFObjectEncoding ObjectEncoding
        {
            get { return AMFObjectEncoding.AMF0; }
        }

        public void Reset()
        {
            referenceCache.Clear();
            currentValue = null;
        }

        public void WriteObjectStreamPreamble()
        {
            // Assume decoder already starts in AMF0 mode by default.
        }

        public void WriteObject(IASValue value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }

            // Check if we have seen the object reference before.
            int referenceId;
            if (referenceCache.TryGetValue(value, out referenceId))
            {
                WriteReference(referenceId);
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
            output.WriteByte((byte)AMF0ObjectTypeCode.Undefined);
        }

        void IASValueVisitor.VisitUnsupported(IActionScriptSerializer serializer)
        {
            output.WriteByte((byte)AMF0ObjectTypeCode.Unsupported);
        }

        void IASValueVisitor.VisitBoolean(IActionScriptSerializer serializer, bool value)
        {
            output.WriteByte((byte)AMF0ObjectTypeCode.Boolean);
            output.WriteBoolean(value);
        }

        void IASValueVisitor.VisitDate(IActionScriptSerializer serializer, double millisecondsSinceEpoch, int timeZoneOffsetMinutes)
        {
            output.WriteByte((byte)AMF0ObjectTypeCode.Date);
            output.WriteDouble(millisecondsSinceEpoch);
            output.WriteShort((short) timeZoneOffsetMinutes);
        }

        void IASValueVisitor.VisitNumber(IActionScriptSerializer serializer, double value)
        {
            WriteNumber(value);
        }

        void IASValueVisitor.VisitInt29(IActionScriptSerializer serializer, int value)
        {
            // Serialize the value as a plain old number for AMF0.
            WriteNumber(value);
        }

        void IASValueVisitor.VisitString(IActionScriptSerializer serializer, string value)
        {
            if (output.IsShortString(value))
            {
                output.WriteByte((byte)AMF0ObjectTypeCode.ShortString);
                output.WriteShortString(value);
            }
            else
            {
                output.WriteByte((byte)AMF0ObjectTypeCode.LongString);
                output.WriteLongString(value);
            }
        }

        void IASValueVisitor.VisitByteArray(IActionScriptSerializer serializer, int length, IEnumerable<ArraySegment<byte>> segments)
        {
            // Serialize the value as a plain old array of numbers for AMF0.
            AddCurrentValueToCache();

            output.WriteByte((byte)AMF0ObjectTypeCode.Array);
            output.WriteInt(length);

            if (length != 0)
            {
                int bytesRemaining = length;

                foreach (ArraySegment<byte> bytes in segments)
                {
                    if (bytesRemaining < bytes.Count)
                        throw new AMFException(ExceptionPrefix + "The byte array provided more bytes than were indicated by its length.");

                    bytesRemaining -= bytes.Count;

                    int offset = bytes.Offset;
                    int count = bytes.Count;
                    byte[] array = bytes.Array;

                    while (count-- > 0)
                        WriteNumber(array[offset++]);
                }

                if (bytesRemaining != 0)
                    throw new AMFException(ExceptionPrefix + "The byte array did not provide as many bytes as were indicated by its length.");
            }
        }

        void IASValueVisitor.VisitArray(IActionScriptSerializer serializer, int indexedLength, IEnumerable<IASValue> indexedValues,
            IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties)
        {
            AddCurrentValueToCache();

            IEnumerator<KeyValuePair<string, IASValue>> dynamicPropertyEnumerator = dynamicProperties.GetEnumerator();
            if (dynamicPropertyEnumerator.MoveNext())
            {
                // Generate a mixed array.  All keys are stored as strings.
                output.WriteByte((byte)AMF0ObjectTypeCode.MixedArray);
                output.WriteInt(indexedLength);

                // Write dynamic properties as key/value pairs.
                do
                {
                    KeyValuePair<string, IASValue> pair = dynamicPropertyEnumerator.Current;
                    if (String.IsNullOrEmpty(pair.Key))
                        throw new AMFException(ExceptionPrefix + "Cannot serialize an array with a null or empty string key.");

                    output.WriteShortString(pair.Key);
                    WriteObject(pair.Value);
                }
                while (dynamicPropertyEnumerator.MoveNext());

                // Write indexed values as key/value pairs.
                int index = 0;
                foreach (IASValue element in indexedValues)
                {
                    if (index == indexedLength)
                        throw new AMFException(ExceptionPrefix + "The number of indexed values provided by the array is greater than was indicated by its length.");

                    output.WriteShortString(index.ToString(CultureInfo.InvariantCulture));
                    WriteObject(element);
                    index += 1;
                }

                if (index != indexedLength)
                    throw new AMFException(ExceptionPrefix + "The number of indexed values provided by the array is less than was indicated by its length.");

                // Terminate with empty string and end of object marker.
                output.WriteShortString("");
                output.WriteByte((byte)AMF0ObjectTypeCode.EndOfObject);
            }
            else
            {
                // Generate an ordinary array.
                output.WriteByte((byte)AMF0ObjectTypeCode.Array);
                output.WriteInt(indexedLength);

                // Write indexed values as a sequence of objects.
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
        }

        void IASValueVisitor.VisitObject(IActionScriptSerializer serializer, ASClass @class, IEnumerable<IASValue> memberValues,
            IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties,
            IExternalizable externalizableValue)
        {
            AddCurrentValueToCache();

            // Externalizable not supported.
            if (@class.Layout == ASClassLayout.Externalizable)
                throw new AMFException(ExceptionPrefix + "Externalizable class layout not supported by AMF0.");

            if (@class.ClassAlias.Length != 0)
            {
                // Write class alias for typed object.
                output.WriteByte((byte)AMF0ObjectTypeCode.TypedObject);
                output.WriteShortString(@class.ClassAlias);
            }
            else
            {
                // Write header for untyped object.
                output.WriteByte((byte)AMF0ObjectTypeCode.Object);
            }

            // Write members as key/value pairs.
            IList<string> memberNames = @class.MemberNames;

            int memberCount = memberNames.Count;
            int memberIndex = 0;
            foreach (IASValue memberValue in memberValues)
            {
                if (memberIndex == memberCount)
                    throw new AMFException(ExceptionPrefix + "The number of member values provided by the object is greater than was indicated by its class.");

                string memberName = memberNames[memberIndex];
                if (String.IsNullOrEmpty(memberName))
                    throw new AMFException(ExceptionPrefix + "Cannot serialize an object with a null or empty member name.");

                output.WriteShortString(memberName);
                WriteObject(memberValue);
                memberIndex += 1;
            }

            if (memberIndex != memberCount)
                throw new AMFException(ExceptionPrefix + "The number of member values provided by the object is less than was indicated by its class.");

            // Write dynamic key/value pairs.
            foreach (KeyValuePair<string, IASValue> pair in dynamicProperties)
            {
                if (String.IsNullOrEmpty(pair.Key))
                    throw new AMFException(ExceptionPrefix + "Cannot serialize an object with a null or empty string key.");

                output.WriteShortString(pair.Key);
                WriteObject(pair.Value);
            }

            // Terminate with empty string and end of object marker.
            output.WriteShortString("");
            output.WriteByte((byte)AMF0ObjectTypeCode.EndOfObject);
        }

        void IASValueVisitor.VisitXml(IActionScriptSerializer serializer, string xmlString)
        {
            output.WriteByte((byte)AMF0ObjectTypeCode.Xml);

            output.WriteLongString(xmlString);
        }

        private void WriteNull()
        {
            output.WriteByte((byte)AMF0ObjectTypeCode.Null);
        }

        private void WriteNumber(double value)
        {
            output.WriteByte((byte)AMF0ObjectTypeCode.Number);
            output.WriteDouble(value);
        }

        private void WriteReference(int referenceId)
        {
            output.WriteByte((byte)AMF0ObjectTypeCode.Reference);
            output.WriteUnsignedShort((ushort) referenceId);
        }

        private void AddCurrentValueToCache()
        {
            int referenceId = referenceCache.Count + 1;
            referenceCache.Add(currentValue, referenceId);
        }
    }
}
