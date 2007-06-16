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
using System.IO;
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Serialization;
using Castle.FlexBridge.Serialization.AMF;
using Castle.FlexBridge.Tests.UnitTests;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Castle.FlexBridge.Tests.UnitTests.Serialization.AMF
{
    [TestFixture]
    [TestsOn(typeof(AMFMessageReader))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class AMFMessageReaderTest : BaseUnitTest
    {
        private AMFDataInput input;
        private MemoryStream stream;
        private IActionScriptSerializer serializer;

        public override void SetUp()
        {
            base.SetUp();

            stream = new MemoryStream();
            serializer = Mocks.CreateMock<IActionScriptSerializer>();
            input = new AMFDataInput(stream, serializer);
        }

        [Test]
        public void ReadMessageWithMultipleHeadersAndBodies()
        {
            SetStreamContents(new byte[] {
                0x12, 0x34, // version
                0x00, 0x02, // header count
                0x00, 0x03, 0x61, 0x62, 0x63, 0x01, 0xff, 0xff, 0xff, 0xff, 0x02, 0x00, 0x03, 0x31, 0x32, 0x33, // header abc
                0x00, 0x03, 0x64, 0x65, 0x66, 0x00, 0xff, 0xff, 0xff, 0xff, 0x05, // header def
                0x00, 0x02, // body count
                0x00, 0x02, 0x74, 0x6f, 0x00, 0x04, 0x66, 0x72, 0x6f, 0x6d, 0xff, 0xff, 0xff, 0xff, 0x02, 0x00, 0x03, 0x31, 0x32, 0x33, // first body
                0x00, 0x02, 0x74, 0x6f, 0x00, 0x04, 0x66, 0x72, 0x6f, 0x6d, 0xff, 0xff, 0xff, 0xff, 0x05 // second body
            });

            AMFMessage message = AMFMessageReader.ReadAMFMessage(input);

            Assert.AreEqual(0x1234, message.Version);

            Assert.AreEqual(2, message.Headers.Count);
            Assert.AreEqual("abc", message.Headers[0].Name);
            Assert.AreEqual(true, message.Headers[0].MustUnderstand);
            Assert.AreEqual(new ASString("123"), message.Headers[0].Content);
            Assert.AreEqual("def", message.Headers[1].Name);
            Assert.AreEqual(false, message.Headers[1].MustUnderstand);
            Assert.AreEqual(null, message.Headers[1].Content);

            Assert.AreEqual(2, message.Bodies.Count);
            Assert.AreEqual("to", message.Bodies[0].RequestTarget);
            Assert.AreEqual("from", message.Bodies[0].ResponseTarget);
            Assert.AreEqual(new ASString("123"), message.Bodies[0].Content);
            Assert.AreEqual("to", message.Bodies[1].RequestTarget);
            Assert.AreEqual("from", message.Bodies[1].ResponseTarget);
            Assert.AreEqual(null, message.Bodies[1].Content);
        }

        [Test]
        public void ReadMessageReturnsNullOnEndOfStream()
        {
            Assert.IsNull(AMFMessageReader.ReadAMFMessage(input));

            // Doing it a second time shouldn't hurt.
            Assert.IsNull(AMFMessageReader.ReadAMFMessage(input));
        }

        [Test]
        [ExpectedException(typeof(AMFException))]
        public void ReadMessageWrapsErrorsWithAMFException()
        {
            SetStreamContents(new byte[] { 0 }); // an incomplete message!

            // Should encounter an EndOfStreamException and wrap it with an AMFException.
            AMFMessageReader.ReadAMFMessage(input);
        }

        /// <summary>
        /// Initializes the contents of the stream to the specified bytes
        /// and resets the stream reading position to 0.
        /// </summary>
        /// <param name="bytes">The bytes to put in the stream.</param>
        private void SetStreamContents(byte[] bytes)
        {
            stream.SetLength(bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}