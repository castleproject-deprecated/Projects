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
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.Serialization.AMF
{
    /// <summary>
    /// Writes AMF messages to a stream.
    /// </summary>
    public class AMFMessageWriter
    {
        private AMFDataOutput output;

        public AMFMessageWriter(AMFDataOutput output)
        {
            this.output = output;
        }

        /// <summary>
        /// Writes an AMF message.
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <exception cref="AMFException">Thrown if an exception occurred while writing the message</exception>
        public void WriteAMFMessage(AMFMessage message)
        {
            try
            {
                UncheckedWriteAMFMessage(message);
            }
            catch (Exception ex)
            {
                throw new AMFException("An exception occurred while writing an AMF message to the stream.", ex);
            }
        }

        /// <summary>
        /// Writes an AMF message and bubbles up exceptions.
        /// </summary>
        /// <param name="message">The message to write</param>
        private void UncheckedWriteAMFMessage(AMFMessage message)
        {
            output.WriteUnsignedShort(message.Version);

            output.WriteUnsignedShort((ushort)message.Headers.Count);
            foreach (AMFHeader header in message.Headers)
                WriteAMFHeader(header);

            output.WriteUnsignedShort((ushort)message.Bodies.Count);
            foreach (AMFBody body in message.Bodies)
                WriteAMFBody(body);
        }

        /// <summary>
        /// Writes an AMF header.
        /// </summary>
        /// <param name="header">The header to write</param>
        private void WriteAMFHeader(AMFHeader header)
        {
            output.WriteShortString(header.Name);
            output.WriteBoolean(header.MustUnderstand);
            output.WriteUnsignedInt(0xffffffff); // header length ignored

            WriteAMFContent(header.Content);
        }

        /// <summary>
        /// Writes an AMF body.
        /// </summary>
        /// <param name="body">The body to write</param>
        private void WriteAMFBody(AMFBody body)
        {
            output.WriteShortString(body.RequestTarget);
            output.WriteShortString(body.ResponseTarget);
            output.WriteUnsignedInt(0xffffffff); // body length ignored

            WriteAMFContent(body.Content);
        }

        /// <summary>
        /// Writes AMF content.
        /// </summary>
        /// <param name="content">The content to write</param>
        private void WriteAMFContent(IASValue content)
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
