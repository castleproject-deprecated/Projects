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
using System.IO;
using System.Text;
using Castle.FlexBridge.Serialization;
using Castle.FlexBridge.Serialization.AMF;
using Castle.FlexBridge.ActionScript;

namespace Castle.FlexBridge.Serialization.AMF
{
    /// <summary>
    /// Outputs data in AMF format to a stream.
    /// </summary>
    public class AMFDataOutput : IDataOutput
    {
        private Stream stream;
        private IActionScriptSerializer serializer;
        private AMFObjectEncoding objectEncoding;

        private UTF8Encoding utf8Encoding;
        private IAMFObjectWriter objectWriter;
        private bool inObjectStream;

        /// <summary>
        /// Creates an AMF data output writer.
        /// Initially uses the <see cref="AMFObjectEncoding.AMF0" /> object encoding.
        /// </summary>
        /// <param name="stream">The output stream</param>
        /// <param name="serializer">The ActionScript serializer to use for object serialization</param>
        public AMFDataOutput(Stream stream, IActionScriptSerializer serializer)
        {
            this.stream = stream;
            this.serializer = serializer;

            objectEncoding = AMFObjectEncoding.AMF0;
            utf8Encoding = new UTF8Encoding(false, false);
        }

        /// <summary>
        /// Gets the ActionScript serializer in use.
        /// </summary>
        public IActionScriptSerializer Serializer
        {
            get { return serializer; }
        }

        /// <summary>
        /// Gets or sets the object encoding in use.
        /// Changes in the encoding only take effect when <see cref="BeginObjectStream" />
        /// is next called.
        /// </summary>
        public AMFObjectEncoding ObjectEncoding
        {
            get { return objectEncoding; }
            set { objectEncoding = value; }
        }

        /// <summary>
        /// Begins writing a new object stream using the currently specified <see cref="ObjectEncoding" />.
        /// <see cref="EndObjectStream" /> must be called when finished writing the object stream.
        /// The encoding must be set before calling this method.
        /// </summary>
        /// <remarks>
        /// An object stream is a container for a sequence of AMF-encoded objects.
        /// In particular, <see cref="WriteObject" /> may only be called when writing an object stream.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if an object stream is already in progress</exception>
        public void BeginObjectStream()
        {
            if (inObjectStream)
                throw new InvalidOperationException("An object stream is already in progress.");

            if (objectWriter == null || objectEncoding != objectWriter.ObjectEncoding)
            {
                switch (objectEncoding)
                {
                    case AMFObjectEncoding.AMF0:
                        objectWriter = new AMF0ObjectWriter(this);
                        break;

                    case AMFObjectEncoding.AMF3:
                        objectWriter = new AMF3ObjectWriter(this);
                        break;

                    default:
                        throw new NotSupportedException("Unsupported object encoding: " + objectEncoding);
                }
            }

            objectWriter.WriteObjectStreamPreamble();

            inObjectStream = true;
        }

        /// <summary>
        /// Ends writing an object stream.
        /// Releases resources associated with writing the object stream so they will be
        /// reclaimed promptly.
        /// </summary>
        /// <remarks>
        /// An object stream is a container for a sequence of AMF-encoded objects.
        /// In particular, <see cref="WriteObject" /> may only be called when writing an object stream.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if an object stream is not currently in progress</exception>
        public void EndObjectStream()
        {
            if (! inObjectStream)
                throw new InvalidOperationException("An object stream is not currently in progress.");

            // If the object encoding has changed, just dicard the old object writer.
            // Otherwise reset it in preparation for reuse on another object stream.
            if (objectEncoding != objectWriter.ObjectEncoding)
                objectWriter = null;
            else
                objectWriter.Reset();

            inObjectStream = false;
        }

        /// <inheritdoc />
        public void WriteByte(byte value)
        {
            stream.WriteByte(value);
        }

        /// <inheritdoc />
        public void WriteBytes(byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <inheritdoc />
        public void WriteBytes(byte[] bytes, int offset, int count)
        {
            stream.Write(bytes, offset, count);
        }

        /// <inheritdoc />
        public void WriteBoolean(bool value)
        {
            stream.WriteByte(value ? (byte)1 : (byte)0);
        }

        /// <inheritdoc />
        public void WriteDouble(double value)
        {
            long bits = BitConverter.DoubleToInt64Bits(value);

            WriteByte(unchecked((byte)(bits >> 56)));
            WriteByte(unchecked((byte)(bits >> 48)));
            WriteByte(unchecked((byte)(bits >> 40)));
            WriteByte(unchecked((byte)(bits >> 32)));
            WriteByte(unchecked((byte)(bits >> 24)));
            WriteByte(unchecked((byte)(bits >> 16)));
            WriteByte(unchecked((byte)(bits >> 8)));
            WriteByte(unchecked((byte)bits));
        }

        /// <inheritdoc />
        public void WriteFloat(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            WriteByte(bytes[3]);
            WriteByte(bytes[2]);
            WriteByte(bytes[1]);
            WriteByte(bytes[0]);
        }

        /// <inheritdoc />
        public void WriteInt(int value)
        {
            WriteUnsignedInt(unchecked((uint)value));
        }

        /// <inheritdoc />
        public void WriteObject(IASValue value)
        {
            if (!inObjectStream)
                throw new InvalidOperationException("WriteObject cannot be called except while writing an object stream.");

            objectWriter.WriteObject(value);
        }

        /// <inheritdoc />
        public void WriteShort(short value)
        {
            WriteUnsignedShort(unchecked((ushort)value));
        }

        /// <inheritdoc />
        public void WriteUnsignedShort(ushort value)
        {
            WriteByte(unchecked((byte)(value >> 8)));
            WriteByte(unchecked((byte)value));
        }

        /// <inheritdoc />
        public void WriteUnsignedInt(uint value)
        {
            WriteByte(unchecked((byte)(value >> 24)));
            WriteByte(unchecked((byte)(value >> 16)));
            WriteByte(unchecked((byte)(value >> 8)));
            WriteByte(unchecked((byte)value));
        }

        /// <inheritdoc />
        public void WriteUTF(string value)
        {
            WriteShortString(value);
        }

        /// <inheritdoc />
        public void WriteUTFBytes(string value)
        {
            byte[] bytes = utf8Encoding.GetBytes(value);
            WriteBytes(bytes);
        }

        /// <summary>
        /// Returns true if the specified string can certainly be represented using
        /// a 16-bit unsigned integer length prefix.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsShortString(string value)
        {
            return utf8Encoding.GetMaxByteCount(value.Length) <= UInt16.MaxValue;
        }

        /// <summary>
        /// Writes a string with a 16-bit unsigned integer length prefix.
        /// </summary>
        /// <param name="value"></param>
        public void WriteShortString(string value)
        {
            if (value.Length == 0)
            {
                WriteUnsignedShort(0);
                return;
            }

            // todo: optimize using existing buffer
            byte[] bytes = utf8Encoding.GetBytes(value);
            if (bytes.Length > UInt16.MaxValue)
                throw new AMFException("String too long to be represented with the short AMF string encoding.");

            WriteUnsignedShort((ushort) bytes.Length);
            WriteBytes(bytes);
        }

        /// <summary>
        /// Writes a string with a 32-bit unsigned integer length prefix.
        /// </summary>
        /// <param name="value"></param>
        public void WriteLongString(string value)
        {
            if (value.Length == 0)
            {
                WriteUnsignedInt(0);
                return;
            }

            // todo: optimize using existing buffer
            byte[] bytes = utf8Encoding.GetBytes(value);

            WriteUnsignedInt((uint) bytes.Length);
            WriteBytes(bytes);
        }

        /// <summary>
        /// Writes a string with a variable-width 29-bit signed integer length prefix
        /// with least-significant bit set to 1.  This is used in AMF3 encoding of strings.
        /// </summary>
        /// <remarks>
        /// Optimized for non-empty strings.  Empty strings are handled elsewhere.
        /// </remarks>
        /// <param name="value"></param>
        public void WriteVWInt29StringWithLSBSetTo1(string value)
        {
            // todo: optimize using existing buffer
            byte[] bytes = utf8Encoding.GetBytes(value);
            if (bytes.Length > ASConstants.Int29MaxValue / 2)
                throw new AMFException("String too long to be represented with the variable-width AMF string encoding.");

            WriteVWInt29(bytes.Length * 2 + 1);
            WriteBytes(bytes);
        }

        /// <summary>
        /// Writes a big-endian variable-width 29-bit signed integer.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteVWInt29(int value)
        {
            value &= 0x1fffffff;

            if (value < 0x80)
            {
                WriteByte(unchecked ((byte)value));
            }
            else if (value < 0x4000)
            {
                WriteByte(unchecked ((byte)((value >> 7) | 0x80)));
                WriteByte(unchecked ((byte)(value & 0x7f)));
            }
            else if (value < 0x200000)
            {
                WriteByte(unchecked((byte)((value >> 14) | 0x80)));
                WriteByte(unchecked((byte)((value >> 7) | 0x80)));
                WriteByte(unchecked((byte)(value & 0x7f)));
            }
            else
            {
                WriteByte(unchecked((byte)((value >> 22) | 0x80)));
                WriteByte(unchecked((byte)((value >> 15) | 0x80)));
                WriteByte(unchecked((byte)((value >> 8) | 0x80)));
                WriteByte(unchecked((byte)value));
            }
        }
    }
}
