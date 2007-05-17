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
using System.Web;
using Castle.FlexBridge.Serialization.AMF;

namespace Castle.FlexBridge.Gateway
{
    /// <summary>
    /// Contains information about an AMF request in progress.
    /// </summary>
    internal class GatewayAMFRequest : IAMFRequest
    {
        private GatewayAMFContext context;
        private AMFMessage message;
        private bool isSecureConnection;

        /// <summary>
        /// Creates an AMF request object.
        /// </summary>
        public GatewayAMFRequest(GatewayAMFContext context)
        {
            this.context = context;
        }

        public AMFMessage Message
        {
            get { return message; }
        }

        public bool IsSecureConnection
        {
            get { return isSecureConnection; }
            set { isSecureConnection = value; }
        }

        /// <summary>
        /// Sets the AMF request message.
        /// </summary>
        /// <param name="message">The message</param>
        public void SetMessage(AMFMessage message)
        {
            this.message = message;
        }
    }
}
