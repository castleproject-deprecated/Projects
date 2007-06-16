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
    /// Reads AMF messages from a stream.
    /// </summary>
    public static class AMFMessageReader
    {
        /// <summary>
        /// Reads an AMF message from an input stream.
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <returns>The AMF message that was read, or null on end of stream</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="input"/> is null</exception>
        /// <exception cref="AMFException">Thrown if an exception occurred while writing the message</exception>
        public static AMFMessage ReadAMFMessage(AMFDataInput input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            try
            {
                // Check for end of stream.
                // If we catch an EndOfStreamException here, we gracefully unwind and
                // return null to the caller.  If it happens later on then it's an error
                // because it means the message is incomplete.
                byte firstByte;
                try
                {
                    firstByte = input.ReadByte();
                }
                catch (EndOfStreamException)
                {
                    // Return gracefully.
                    return null;
                }

                // Okay, there's data.  From now on, if we see EndOfStreamException it's an error!
                return UncheckedReadAMFMessage(input, firstByte);
            }
            catch (Exception ex)
            {
                throw new AMFException("An exception occurred while reading an AMF message from the stream.", ex);
            }
        }

        /// <summary>
        /// Reads an AMF message from an input stream and bubbles up exceptions.
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <param name="firstByte">The first byte of the message</param>
        /// <returns>The AMF message that was read</returns>
        private static AMFMessage UncheckedReadAMFMessage(AMFDataInput input, byte firstByte)
        {
            byte secondVersionByte = input.ReadByte();
            ushort version = (ushort) ((firstByte << 8) | secondVersionByte);

            int headerCount = input.ReadUnsignedShort();
            AMFHeader[] headers = new AMFHeader[headerCount];
            for (int i = 0; i < headerCount; i++)
                headers[i] = ReadAMFHeader(input);

            int bodyCount = input.ReadUnsignedShort();
            AMFBody[] bodies = new AMFBody[bodyCount];
            for (int i = 0; i < bodyCount; i++)
                bodies[i] = ReadAMFBody(input);

            return new AMFMessage(version, headers, bodies);
        }

        /// <summary>
        /// Reads an AMF header from an input stream.
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <returns>The AMF header that was read</returns>
        private static AMFHeader ReadAMFHeader(AMFDataInput input)
        {
            string name = input.ReadShortString();
            bool mustUnderstand = input.ReadBoolean();
            int headerLength = input.ReadInt();

            IASValue content = ReadAMFContent(input, headerLength);

            return new AMFHeader(name, mustUnderstand, content);
        }

        /// <summary>
        /// Reads an AMF body from an input stream.
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <returns>The AMF body that was read</returns>
        private static AMFBody ReadAMFBody(AMFDataInput input)
        {
            string requestTarget = input.ReadShortString();
            string responseTarget = input.ReadShortString();
            int bodyLength = input.ReadInt();

            IASValue content = ReadAMFContent(input, bodyLength);

            return new AMFBody(requestTarget, responseTarget, content);
        }

        /// <summary>
        /// Reads AMF content from an input stream.
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <param name="contentLength">The length of the content to read in bytes</param>
        /// <returns>The content object</returns>
        private static IASValue ReadAMFContent(AMFDataInput input, int contentLength)
        {
            input.BeginObjectStream();
            try
            {
                // TODO: validate for content length
                return input.ReadObject();
            }
            finally
            {
                input.EndObjectStream();
            }
        }
    }
}
