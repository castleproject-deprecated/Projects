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
using Castle.FlexBridge.ActionScript;

namespace Castle.FlexBridge.Serialization.AMF
{
    /// <summary>
    /// Inputs data in AMF format from a stream.
    /// </summary>
    public class AMFDataInput : IDataInput
    {
        private Stream stream;
        private IActionScriptSerializer serializer;
        private UTF8Encoding utf8Encoding;

        private IAMFObjectReader objectReader;
        private bool inObjectStream;

        /// <summary>
        /// A small buffer to use for deserializing primitive types.
        /// </summary>
        private byte[] buffer = new byte[8];

        /// <summary>
        /// Creates an AMF data input reader.
        /// Initially uses the <see cref="AMFObjectEncoding.AMF0" /> object encoding,
        /// switches modes automatically if <see cref="AMFObjectEncoding.AMF3" /> data is encountered.
        /// </summary>
        /// <param name="stream">The input stream</param>
        /// <param name="serializer">The ActionScript serializer to use for object serialization</param>
        public AMFDataInput(Stream stream, IActionScriptSerializer serializer)
        {
            this.stream = stream;
            this.serializer = serializer;

            objectReader = new AMF0ObjectReader(this);
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
        /// Gets the object encoding currently in use.
        /// </summary>
        public AMFObjectEncoding ObjectEncoding
        {
            get { return objectReader.ObjectEncoding; }
        }

        /// <summary>
        /// Begins reading a new object stream.
        /// <see cref="EndObjectStream" /> must be called when finished reading the object stream.
        /// </summary>
        /// <remarks>
        /// An object stream is a container for a sequence of AMF-encoded objects.
        /// In particular, <see cref="ReadObject" /> may only be called when reading an object stream.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if an object stream is already in progress</exception>
        public void BeginObjectStream()
        {
            if (inObjectStream)
                throw new InvalidOperationException("An object stream is already in progress.");

            inObjectStream = true;
        }

        /// <summary>
        /// Ends reading an object stream.
        /// Releases resources associated with reading the object stream so they will be
        /// reclaimed promptly.
        /// </summary>
        /// <remarks>
        /// An object stream is a container for a sequence of AMF-encoded objects.
        /// In particular, <see cref="ReadObject" /> may only be called when reading an object stream.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if an object stream is not currently in progress</exception>
        public void EndObjectStream()
        {
            if (!inObjectStream)
                throw new InvalidOperationException("An object stream is not currently in progress.");

            objectReader.Reset();
            inObjectStream = false;
        }

        public bool ReadBoolean()
        {
            return ReadByte() != 0;
        }

        public byte ReadByte()
        {
            int value = stream.ReadByte();
            if (value < 0)
                ThrowEndOfStreamException();

            return (byte) value;
        }

        public void ReadBytes(byte[] bytes, int offset, int count)
        {
            int actualCount = stream.Read(bytes, offset, count);
            if (actualCount != count)
                ThrowEndOfStreamException();
        }

        public double ReadDouble()
        {
            ReadBytes(buffer, 0, 8);

            // Need to reverse the order and pack into an int64.
            long high = unchecked((long) ((buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3]));
            long low = unchecked((long) ((buffer[4] << 24) | (buffer[5] << 16) | (buffer[6] << 8) | buffer[7]));

            long bits = (high << 32) | low;
            return BitConverter.Int64BitsToDouble(bits);
        }

        public float ReadFloat()
        {
            ReadBytes(buffer, 0, 4);

            // Reverse the order.
            buffer[4] = buffer[3];
            buffer[5] = buffer[2];
            buffer[6] = buffer[1];
            buffer[7] = buffer[0];
            return BitConverter.ToSingle(buffer, 4);
        }

        public int ReadInt()
        {
            return unchecked((int)ReadUnsignedInt());
        }

        public IASValue ReadObject()
        {
            if (!inObjectStream)
                throw new InvalidOperationException("ReadOnly cannot be called except while reading an object stream.");

            return objectReader.ReadObject();
        }

        public short ReadShort()
        {
            return unchecked((short)ReadUnsignedShort());
        }

        public uint ReadUnsignedInt()
        {
            ReadBytes(buffer, 0, 4);

            return unchecked((uint) ((buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3]));
        }

        public ushort ReadUnsignedShort()
        {
            ReadBytes(buffer, 0, 2);

            return unchecked((ushort)((buffer[0] << 8) | buffer[1]));
        }

        public string ReadUTF()
        {
            return ReadShortString();
        }

        public string ReadUTFBytes(int length)
        {
            if (length == 0)
                return "";
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            // TODO: Optimize to avoid creating temporary byte buffer
            byte[] bytes = new byte[length];
            ReadBytes(bytes, 0, length);

            return utf8Encoding.GetString(bytes);
        }

        /// <summary>
        /// Reads a string with a 16-bit unsigned integer length prefix.
        /// </summary>
        /// <returns>The value</returns>
        public string ReadShortString()
        {
            int length = ReadUnsignedShort();
            return ReadUTFBytes(length);
        }

        /// <summary>
        /// Reads a string with a 32-bit unsigned integer length prefix.
        /// </summary>
        /// <returns>The value</returns>
        public string ReadLongString()
        {
            int length = ReadInt();
            return ReadUTFBytes(length);
        }

        /// <summary>
        /// Reads a big-endian variable-width 29-bit signed integer.
        /// </summary>
        /// <returns>The value that was read</returns>
        public int ReadVWInt29()
        {
            // at least 1 byte
            byte b = ReadByte();

            if (b < 0x80)
                return b;

            // at least 2 bytes
            int result = unchecked((b & 0x7f) << 7);
            b = ReadByte();

            if (b < 0x80)
                return result | b;

            // at least 3 bytes
            result = unchecked((result | (b & 0x7f)) << 7);
            b = ReadByte();

            if (b < 0x80)
                return result | b;

            // must be 4 bytes
            result = unchecked((result | (b & 0x7f)) << 8);
            b = ReadByte();
            result |= b;

            // check if msb of 29bit integer is set
            if ((result & 0x01000000) == 0)
                return result;

            return unchecked(result | (int) 0xe0000000); // sign-extend to 32 bits
        }

        /// <summary>
        /// Throws an end of stream exception.
        /// </summary>
        private static void ThrowEndOfStreamException()
        {
            throw new EndOfStreamException("End of stream encountered while reading AMF data.");
        }
    }
}
