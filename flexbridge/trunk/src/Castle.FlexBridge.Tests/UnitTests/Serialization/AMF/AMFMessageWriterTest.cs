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
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Serialization;
using Castle.FlexBridge.Serialization.AMF;
using Castle.FlexBridge.Tests.UnitTests;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Castle.FlexBridge.Tests.UnitTests.Serialization.AMF
{
    [TestFixture]
    [TestsOn(typeof(AMFMessageWriter))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class AMFMessageWriterTest : BaseUnitTest
    {
        private AMFDataOutput output;
        private MemoryStream stream;
        private IActionScriptSerializer serializer;

        public override void SetUp()
        {
            base.SetUp();

            stream = new MemoryStream();
            serializer = Mocks.CreateMock<IActionScriptSerializer>();
            output = new AMFDataOutput(stream, serializer);
        }

        [Test]
        public void WriteMessageWithMultipleHeadersAndBodies()
        {
            AMFMessage message = new AMFMessage();

            message.Version = 0x1234;
            message.Headers.Add(new AMFHeader("abc", true, new ASString("123")));
            message.Headers.Add(new AMFHeader("def", false, null));
            message.Bodies.Add(new AMFBody("to", "from", new ASString("123")));
            message.Bodies.Add(new AMFBody("to", "from", null));
            
            AMFMessageWriter.WriteAMFMessage(output, message);

            CollectionAssert.AreElementsEqual(new byte[] {
                0x12, 0x34, // version
                0x00, 0x02, // header count
                0x00, 0x03, 0x61, 0x62, 0x63, 0x01, 0xff, 0xff, 0xff, 0xff, 0x02, 0x00, 0x03, 0x31, 0x32, 0x33, // header abc
                0x00, 0x03, 0x64, 0x65, 0x66, 0x00, 0xff, 0xff, 0xff, 0xff, 0x05, // header def
                0x00, 0x02, // body count
                0x00, 0x02, 0x74, 0x6f, 0x00, 0x04, 0x66, 0x72, 0x6f, 0x6d, 0xff, 0xff, 0xff, 0xff, 0x02, 0x00, 0x03, 0x31, 0x32, 0x33, // first body
                0x00, 0x02, 0x74, 0x6f, 0x00, 0x04, 0x66, 0x72, 0x6f, 0x6d, 0xff, 0xff, 0xff, 0xff, 0x05 // second body
            }, stream.ToArray());
        }

        [Test]
        [ExpectedException(typeof(AMFException))]
        public void WriteMessageWrapsErrorsWithAMFException()
        {
            // Deliberately inject an exception.
            // This should be caught and wrapped by the AMFMessageWriter code.
            IASValue mockValue = Mocks.CreateMock<IASValue>();
            mockValue.AcceptVisitor(serializer, null);
            LastCall.IgnoreArguments().Throw(new Exception("Something bad happened."));

            Mocks.ReplayAll();

            AMFMessage message = new AMFMessage();

            message.Version = 0x1234;
            message.Headers.Add(new AMFHeader("abc", true, mockValue));

            AMFMessageWriter.WriteAMFMessage(output, message);
        }
    }
}