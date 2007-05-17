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
using System.Text;
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Serialization.AMF;

namespace Castle.FlexBridge.Serialization
{
    /// <summary>
    /// Provides methods for writing binary data to an AMF stream.
    /// </summary>
    /// <remarks>
    /// The methods provided here parallel those defined on the client side
    /// in the "flash.utils.IDataOutput" class with the exception of the following:
    /// 
    /// WriteByte() and WriteBytes() write unsigned bytes instead of signed bytes.
    /// WriteUnsignedByte() has been omitted.
    /// Unsigned integer parameters to certain methods for offsets and lengths have
    /// been made signed.
    /// WriteMultiByte() is not supported.
    /// The buffer control and encoding properties have been omitted.
    /// </remarks>
    public interface IDataOutput
    {
        /// <summary>
        /// Gets the object encoding currently in use.
        /// </summary>
        AMFObjectEncoding ObjectEncoding { get; }

        /// <summary>
        /// Gets the ActionScript serializer in use.
        /// </summary>
        IActionScriptSerializer Serializer { get; }

        /// <summary>
        /// Writes a Boolean value as a 1 or 0 byte.
        /// </summary>
        /// <param name="value"></param>
        void WriteBoolean(bool value);

        /// <summary>
        /// Writes a byte.
        /// </summary>
        /// <param name="value"></param>
        void WriteByte(byte value);

        /// <summary>
        /// Writes a sequence of count bytes from the specified byte array,
        /// starting offset(zero-based index) bytes.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        void WriteBytes(byte[] value, int offset, int count);

        /// <summary>
        /// Writes an IEEE 754 double-precision (64-bit) floating point number.
        /// </summary>
        /// <param name="value"></param>
        void WriteDouble(double value);

        /// <summary>
        /// Writes an IEEE 754 single-precision (32-bit) floating point number.
        /// </summary>
        /// <param name="value"></param>
        void WriteFloat(float value);

        /// <summary>
        /// Writes a 32-bit signed integer.
        /// </summary>
        /// <param name="value"></param>
        void WriteInt(int value);

        /*
        /// <summary>
        /// Writes a multibyte string using the specified character set.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="charSet"></param>
        void WriteMultiByte(string value, string charSet);
        */

        /// <summary>
        /// Writes an object in AMF serialized format.
        /// </summary>
        /// <param name="value"></param>
        void WriteObject(IASValue value);

        /// <summary>
        /// Writes a 16-bit integer.
        /// </summary>
        /// <param name="value"></param>
        void WriteShort(short value);

        /// <summary>
        /// Writes a 32-bit unsigned integer.
        /// </summary>
        /// <param name="value"></param>
        void WriteUnsignedInt(uint value);

        /// <summary>
        /// Writes a 16-bit unsigned integer.
        /// </summary>
        /// <param name="value"></param>
        void WriteUnsignedShort(ushort value);

        /// <summary>
        /// Writes a UTF-8 string with a 16-bit length prefix.
        /// </summary>
        /// <param name="value"></param>
        void WriteUTF(string value);

        /// <summary>
        /// Writes a UTF-8 string. 
        /// </summary>
        /// <param name="value"></param>
        void WriteUTFBytes(string value);
    }
}
