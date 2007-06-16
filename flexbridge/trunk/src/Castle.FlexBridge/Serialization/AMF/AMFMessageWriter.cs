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
using Castle.FlexBridge.ActionScript;

namespace Castle.FlexBridge.Serialization.AMF
{
    /// <summary>
    /// Writes AMF messages to a stream.
    /// </summary>
    public static class AMFMessageWriter
    {
        /// <summary>
        /// Writes an AMF message to an output stream.
        /// </summary>
        /// <param name="output">The output stream</param>
        /// <param name="message">The message to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="output"/> or
        /// <paramref name="message"/> is null</exception>
        /// <exception cref="AMFException">Thrown if an exception occurred while writing the message</exception>
        public static void WriteAMFMessage(AMFDataOutput output, AMFMessage message)
        {
            if (output == null)
                throw new ArgumentNullException("output");
            if (message == null)
                throw new ArgumentNullException("message");

            try
            {
                UncheckedWriteAMFMessage(output, message);
            }
            catch (Exception ex)
            {
                throw new AMFException("An exception occurred while writing an AMF message to the stream.", ex);
            }
        }

        /// <summary>
        /// Writes an AMF message to an output stream and bubbles up exceptions.
        /// </summary>
        /// <param name="output">The output stream</param>
        /// <param name="message">The message to write</param>
        private static void UncheckedWriteAMFMessage(AMFDataOutput output, AMFMessage message)
        {
            output.WriteUnsignedShort(message.Version);

            output.WriteUnsignedShort((ushort)message.Headers.Count);
            foreach (AMFHeader header in message.Headers)
                WriteAMFHeader(output, header);

            output.WriteUnsignedShort((ushort)message.Bodies.Count);
            foreach (AMFBody body in message.Bodies)
                WriteAMFBody(output, body);
        }

        /// <summary>
        /// Writes an AMF header.
        /// </summary>
        /// <param name="output">The output stream</param>
        /// <param name="header">The header to write</param>
        private static void WriteAMFHeader(AMFDataOutput output, AMFHeader header)
        {
            output.WriteShortString(header.Name);
            output.WriteBoolean(header.MustUnderstand);
            output.WriteUnsignedInt(0xffffffff); // header length ignored

            WriteAMFContent(output, header.Content);
        }

        /// <summary>
        /// Writes an AMF body.
        /// </summary>
        /// <param name="output">The output stream</param>
        /// <param name="body">The body to write</param>
        private static void WriteAMFBody(AMFDataOutput output, AMFBody body)
        {
            output.WriteShortString(body.RequestTarget);
            output.WriteShortString(body.ResponseTarget);
            output.WriteUnsignedInt(0xffffffff); // body length ignored

            WriteAMFContent(output, body.Content);
        }

        /// <summary>
        /// Writes AMF content.
        /// </summary>
        /// <param name="output">The output stream</param>
        /// <param name="content">The content to write</param>
        private static void WriteAMFContent(AMFDataOutput output, IASValue content)
        {
            output.BeginObjectStream();
            try
            {
                output.WriteObject(content);
            }
            finally
            {
                output.EndObjectStream();
            }
        }
    }
}
