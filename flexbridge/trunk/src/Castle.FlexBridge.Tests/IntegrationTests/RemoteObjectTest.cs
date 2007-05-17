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
using System.Threading;
using Castle.FlexBridge.Serialization.AMF;
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests.IntegrationTests
{
    /// <summary>
    /// Verifies round-trip messaging related to Adobe Flex (tm) RemoteObjects.
    /// Performs some remote method invocations via the gateway and
    /// verifies that the expected results were received.
    /// </summary>
    [DependsOn(typeof(VerifyIntegrationTestPrerequisites))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class RemoteObjectTest : BaseIntegrationTest
    {
        private RemoteObjectTestProxy remoteObjectTestProxy;

        public override void SetUp()
        {
            base.SetUp();

            remoteObjectTestProxy = new RemoteObjectTestProxy(this);
        }

        [Test]
        public void Test()
        {
            remoteObjectTestProxy.Initialize(RemoteObjectTestProxy.GatewayDestination, AMFObjectEncoding.AMF3);
            MethodResult result = remoteObjectTestProxy.InvokeMethod("test", "a", "b", "c");

            AssertSuccessfulResult(result);
        }

        private static void AssertSuccessfulResult(MethodResult result)
        {
            Assert.IsNull(result.FaultMessage);
        }
    }
}
