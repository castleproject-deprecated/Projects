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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Serialization;
using Castle.FlexBridge.Serialization.AMF;
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests.IntegrationTests
{
    /// <summary>
    /// Communicates with a RemoteObjectTestProxy running inside an Adobe Flex (tm) application to
    /// perform requests on behalf of a test.
    /// </summary>
    public class RemoteObjectTestProxy
    {
        private IFlashExternalInterface externalInterface;

        private const int ResponsePollMilliseconds = 100;
        private const int ResponseTimeoutSeconds = 10;

        public const string GatewayDestination = "gateway";
        public const string SecureGatewayDestination = "secure-gateway";

        private MappedActionScriptSerializerFactory serializerFactory;

        public RemoteObjectTestProxy(IFlashExternalInterface externalInterface)
        {
            this.externalInterface = externalInterface;

            serializerFactory = new MappedActionScriptSerializerFactory(true);
            serializerFactory.MappingTable.RegisterTypesInAssembly(GetType().Assembly);
        }

        public void Initialize(string destination, AMFObjectEncoding objectEncoding)
        {
            externalInterface.InvokeMethod("remoteObjectTestProxy_Initialize", destination, objectEncoding.ToString());
        }

        public void SetCredentials(string username, string password)
        {
            externalInterface.InvokeMethod("remoteObjectTestProxy_SetCredentials", username, password);
        }

        public void SetRemoteCredentials(string username, string password)
        {
            externalInterface.InvokeMethod("remoteObjectTestProxy_SetRemoteCredentials", username, password);
        }

        public MethodResult InvokeMethod(string methodName, params object[] args)
        {
            byte[] amfArgs = ToAMF(args);
            string amfBase64Args = Convert.ToBase64String(amfArgs);
            externalInterface.InvokeMethod("remoteObjectTestProxy_InvokeMethod", methodName, amfBase64Args);

            // Note: Don't time out while a debugger is attached.  We might be trying to
            //       debug the test from another thread.
            for (int i = 0; Debugger.IsAttached || i < ResponseTimeoutSeconds * 1000 / ResponsePollMilliseconds; i++)
            {
                object resultValue = externalInterface.InvokeMethod("remoteObjectTestProxy_GetLastMethodResult");
                if (resultValue != DBNull.Value)
                {
                    string amfBase64Result = (string)resultValue;
                    byte[] amfResult = Convert.FromBase64String(amfBase64Result);

                    MethodResult result = (MethodResult) FromAMF(amfResult);
                    return result;
                }

                Thread.Sleep(ResponsePollMilliseconds);
            }

            Assert.Fail("Timeout while waiting for response from method '{0}'.", methodName);
            return null;
        }

        public void Logout()
        {
            externalInterface.InvokeMethod("remoteObjectTestProxy_Logout");
        }

        private byte[] ToAMF(object nativeValue)
        {
            IActionScriptSerializer serializer = serializerFactory.CreateSerializer();
            IASValue asValue = serializer.ToASValue(nativeValue);
            MemoryStream stream = new MemoryStream();
            AMFDataOutput dataOutput = new AMFDataOutput(stream, serializer);
            dataOutput.ObjectEncoding = AMFObjectEncoding.AMF3;
            dataOutput.BeginObjectStream();
            dataOutput.WriteObject(asValue);
            dataOutput.EndObjectStream();

            return stream.ToArray();
        }

        private object FromAMF(byte[] bytes)
        {
            IActionScriptSerializer serializer = serializerFactory.CreateSerializer();
            MemoryStream stream = new MemoryStream(bytes);
            AMFDataInput dataInput = new AMFDataInput(stream, serializer);
            dataInput.BeginObjectStream();
            IASValue asValue = dataInput.ReadObject();
            dataInput.EndObjectStream();
            object nativeValue = serializer.ToNative(asValue, null);

            return nativeValue;
        }
    }
}
