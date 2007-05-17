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
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests.UnitTests.Serialization.AMF
{
    [TestFixture]
    [TestsOn(typeof(AMFBody))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class AMFBodyTest
    {
        [Test]
        public void CanGetAndSetProperties()
        {
            ASString content = new ASString("abc");

            AMFBody body = new AMFBody();

            Assert.IsNull(body.Content);
            body.Content = content;
            Assert.AreSame(content, body.Content);

            Assert.IsNull(body.RequestTarget);
            body.RequestTarget = "abc";
            Assert.AreEqual("abc", body.RequestTarget);

            Assert.IsNull(body.ResponseTarget);
            body.ResponseTarget = "def";
            Assert.AreEqual("def", body.ResponseTarget);
        }

        [Test]
        public void SpecialConstructorInitializesProperties()
        {
            ASString content = new ASString("abc");
            AMFBody body = new AMFBody("request", "response", content);

            Assert.AreEqual("request", body.RequestTarget);
            Assert.AreEqual("response", body.ResponseTarget);
            Assert.AreSame(content, body.Content);
        }
    }
}
