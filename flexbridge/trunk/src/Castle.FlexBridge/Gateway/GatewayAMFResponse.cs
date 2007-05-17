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
using Castle.FlexBridge.Messaging.DebugEvents;
using Castle.FlexBridge.Messaging.Messages;
using Castle.FlexBridge.Serialization.AMF;

namespace Castle.FlexBridge.Gateway
{
    /// <summary>
    /// Contains information about an AMF response being generated.
    /// </summary>
    internal class GatewayAMFResponse : IAMFResponse
    {
        private GatewayAMFContext context;
        private AMFMessage message;

        /// <summary>
        /// The response target suffix for error messages.
        /// </summary>
        private const string ErrorTargetSuffix = "/onStatus";

        /// <summary>
        /// The response target suffix for successful completion results.
        /// </summary>
        private const string ResultTargetSuffix = "/onResult";

        /// <summary>
        /// The response target suffix for debug event notifications.
        /// </summary>
        private const string DebugEventTargetSuffix = "/onDebugEvents";

        /// <summary>
        /// Creates an AMF response object.
        /// </summary>
        public GatewayAMFResponse(GatewayAMFContext context)
        {
            this.context = context;
        }

        public AMFMessage Message
        {
            get { return message; }
        }

        /// <summary>
        /// Prepares the response message in response to some request.
        /// </summary>
        /// <param name="requestMessage">The original request message</param>
        public void PrepareResponseMessage(AMFMessage requestMessage)
        {
            message = new AMFMessage(requestMessage.Version, new List<AMFHeader>(), new List<AMFBody>());
        }

        public void AddResultResponse(string responseTarget, object content)
        {
            IASValue asContent = context.Serializer.ToASValue(content);
            AddBody(new AMFBody(responseTarget + ResultTargetSuffix, "null", asContent));
        }

        public void AddErrorResponse(string responseTarget, string errorMessage, Exception exception)
        {
            // TODO: Only send full exception details if server is configured for it.
            if (exception != null)
                errorMessage += "\n" + exception.ToString();

            AddBody(new AMFBody(responseTarget + ErrorTargetSuffix, "null", new ASString(errorMessage)));
        }

        public void AddErrorResponse(string responseTarget, ErrorMessage errorMessage)
        {
            IASValue asContent = context.Serializer.ToASValue(errorMessage);
            AddBody(new AMFBody(responseTarget + ErrorTargetSuffix, "null", asContent));
        }

        public void AddDebugEventResponse(string responseTarget, DebugEvent debugEvent)
        {
            IASValue asContent = context.Serializer.ToASValue(debugEvent);
            AddBody(new AMFBody(responseTarget + DebugEventTargetSuffix, "null", asContent));
        }

        public void AddBody(AMFBody body)
        {
            message.Bodies.Add(body);
        }
    }
}
