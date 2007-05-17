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
using Castle.FlexBridge.Collections;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.ActionScript
{
    /// <summary>
    /// Adapts a byte array for ActionScript serialization.
    /// </summary>
    /// <remarks>
    /// If your byte array is large and not stored entirely in memory (for instance, if it is a stream),
    /// it may be worthwhile creating a custom implementation of <see cref="IASValue" />
    /// to provide a more efficient serialization by presenting the contents as an enumeration of
    /// byte segments.
    /// </remarks>
    public sealed class ASByteArray : BaseASValue
    {
        private ArraySegment<byte> bytes;

        /// <summary>
        /// Gets a singleton read-only empty byte array instance.
        /// </summary>
        public static readonly ASByteArray Empty = new ASByteArray(EmptyArray<byte>.Instance);

        /// <summary>
        /// Creates a wrapper for an existing byte array.
        /// </summary>
        /// <param name="bytes">The array of bytes</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="bytes"/> is null</exception>
        public ASByteArray(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            this.bytes = new ArraySegment<byte>(bytes);
        }

        /// <summary>
        /// Creates a wrapper for a segment of a byte array.
        /// </summary>
        /// <param name="bytes">The byte array segment</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="bytes"/> is null</exception>
        public ASByteArray(ArraySegment<byte> bytes)
        {
            this.bytes = bytes;
        }

        /// <summary>
        /// Gets the array of bytes.
        /// </summary>
        public ArraySegment<byte> Bytes
        {
            get { return bytes; }
        }

        /// <summary>
        /// Gets the number of bytes in the byte array.
        /// </summary>
        public int Count
        {
            get { return bytes.Count; }
        }

        /// <summary>
        /// Gets or sets the byte with the specified index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The value</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of bounds</exception>
        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= bytes.Count)
                    throw new ArgumentOutOfRangeException("index");

                return bytes.Array[index + bytes.Offset];
            }
            set
            {
                if (index < 0 || index >= bytes.Count)
                    throw new ArgumentOutOfRangeException("index");

                bytes.Array[index + bytes.Offset] = value;
            }
        }

        public override ASTypeKind Kind
        {
            get { return ASTypeKind.ByteArray; }
        }

        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            ArraySegment<byte>[] segments = new ArraySegment<byte>[] { bytes };
            visitor.VisitByteArray(serializer, bytes.Count, segments);
        }

        public override object GetNativeValue(Type nativeType)
        {
            if (nativeType == typeof(byte[])
                && bytes.Offset == 0 && bytes.Count == bytes.Array.Length)
                return bytes.Array;

            if (nativeType == typeof(ArraySegment<byte>))
                return bytes;

            return base.GetNativeValue(nativeType);
        }
    }
}
