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

using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Serialization.AMF;
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests.UnitTests.Serialization.AMF
{
    [TestFixture]
    [TestsOn(typeof(AMFHeader))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class AMFHeaderTest
    {
        [Test]
        public void CanGetAndSetProperties()
        {
            ASString content = new ASString("abc");
            AMFHeader header = new AMFHeader();

            Assert.IsNull(header.Content);
            header.Content = content;
            Assert.AreSame(content, header.Content);

            Assert.IsFalse(header.MustUnderstand);
            header.MustUnderstand = true;
            Assert.IsTrue(header.MustUnderstand);

            Assert.IsNull(header.Name);
            header.Name = "abc";
            Assert.AreEqual("abc", header.Name);
        }

        [Test]
        public void SpecialConstructorInitializesProperties()
        {
            ASString content = new ASString("abc");
            AMFHeader header = new AMFHeader("header", true, content);

            Assert.AreEqual("header", header.Name);
            Assert.IsTrue(header.MustUnderstand);
            Assert.AreEqual(content, header.Content);
        }
    }
}