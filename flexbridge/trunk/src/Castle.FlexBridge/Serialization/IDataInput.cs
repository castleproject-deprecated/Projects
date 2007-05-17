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
    /// Provides methods for reading binary data from an AMF stream.
    /// </summary>
    /// <remarks>
    /// The methods provided here parallel those defined on the client side
    /// in the "flash.utils.IDataInput" class with the exception of the following:
    /// 
    /// ReadByte() and ReadBytes() read unsigned bytes instead of signed bytes.
    /// ReadUnsignedByte() has been omitted.
    /// Unsigned integer parameters to certain methods for offsets and lengths have
    /// been made signed.
    /// ReadMultiByte() is not supported.
    /// The buffer control and encoding properties have been omitted.
    /// </remarks>
    public interface IDataInput
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
        /// Reads a Boolean value as a 1 or 0 byte.
        /// </summary>
        /// <returns></returns>
        bool ReadBoolean();

        /// <summary>
        /// Reads a byte.
        /// </summary>
        /// <returns></returns>
        byte ReadByte();

        /// <summary>
        /// Reads the number of data bytes, specified by the count parameter.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        void ReadBytes(byte[] bytes, int offset, int count);

        /// <summary>
        /// Reads an IEEE 754 double-precision floating point number.
        /// </summary>
        /// <returns></returns>
        double ReadDouble();

        /// <summary>
        /// Reads an IEEE 754 single-precision floating point number.
        /// </summary>
        /// <returns></returns>
        float ReadFloat();

        /// <summary>
        /// Reads a signed 32-bit integer.
        /// </summary>
        /// <returns></returns>
        int ReadInt();

        /*
        /// <summary>
        /// Reads a multibyte string of specified length using the specified character set.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="charSet"></param>
        /// <returns></returns>
        string ReadMultiByte(int length, string charSet);
        */

        /// <summary>
        /// Reads an object encoded in AMF serialized format.
        /// </summary>
        /// <returns></returns>
        IASValue ReadObject();

        /// <summary>
        /// Reads a signed 16-bit integer.
        /// </summary>
        /// <returns></returns>
        short ReadShort();

        /// <summary>
        /// Reads an unsigned 32-bit integer.
        /// </summary>
        /// <returns></returns>
        uint ReadUnsignedInt();

        /// <summary>
        /// Reads an unsigned 16-bit integer.
        /// </summary>
        /// <returns></returns>
        ushort ReadUnsignedShort();

        /// <summary>
        /// Reads a UTF-8 string with a 16-bit length prefix.
        /// </summary>
        /// <returns></returns>
        string ReadUTF();

        /// <summary>
        /// Reads a UTF-8 string with the specified number of bytes in length.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        string ReadUTFBytes(int length);
    }
}
