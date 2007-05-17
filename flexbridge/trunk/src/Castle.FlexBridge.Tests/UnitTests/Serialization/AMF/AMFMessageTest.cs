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

using Castle.FlexBridge.Serialization.AMF;
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests.UnitTests.Serialization.AMF
{
    [TestFixture]
    [TestsOn(typeof(AMFMessage))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class AMFMessageTest
    {
        [Test]
        public void CanGetAndSetProperties()
        {
            AMFMessage message = new AMFMessage();

            Assert.AreEqual(0, message.Version);
            message.Version = 123;
            Assert.AreEqual(123, message.Version);

            Assert.AreEqual(0, message.Bodies.Count);
            message.Bodies.Add(new AMFBody());
            Assert.AreEqual(1, message.Bodies.Count);

            Assert.AreEqual(0, message.Headers.Count);
            message.Headers.Add(new AMFHeader());
            Assert.AreEqual(1, message.Headers.Count);
        }

        [Test]
        public void SpecialConstructorInitializesProperties()
        {
            AMFMessage message = new AMFMessage(123, new AMFHeader[] { new AMFHeader() }, new AMFBody[] { new AMFBody() });
            Assert.AreEqual(123, message.Version);
            Assert.AreEqual(1, message.Bodies.Count);
            Assert.AreEqual(1, message.Headers.Count);
        }
    }
}