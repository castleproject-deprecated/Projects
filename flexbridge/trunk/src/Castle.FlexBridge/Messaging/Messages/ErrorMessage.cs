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
using System.Globalization;
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Collections;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.Messaging.Messages
{
    /// <summary>
    /// The ErrorMessage class is used to report errors within the messaging system.
    /// An error message only occurs in response to a message sent within the system. 
    /// </summary>
    /// <remarks>
    /// This is a built-in message type ported from Adobe Flex 2 (tm) libraries.
    /// Refer to Adobe Flex (tm) documentation for more details.
    /// </remarks>
    [ActionScriptClass("flex.messaging.messages.ErrorMessage")]
    public class ErrorMessage : AcknowledgeMessage
    {
        private ASObject extendedData;
        private string faultCode;
        private string faultDetail;
        private string faultString;
        private ASObject rootCause;

        /// <summary>
        /// If a message may not have been delivered, the faultCode will contain this constant.
        /// </summary>
        public const string MessageDeliveryInDoubt = "Client.Error.DeliveryInDoubt";

        /// <summary>
        /// Header name for the retryable hint header.
        /// This is used to indicate that the operation that generated the error may be
        /// retryable rather than fatal. 
        /// </summary>
        public const string RetryableHintHeader = "DSRetryableErrorHint";

        /// <summary>
        /// Creates an error message in response to a request.
        /// </summary>
        /// <param name="requestMessage">The original request message</param>
        /// <param name="faultString">The fault string text</param>
        /// <param name="faultCode">The fault code, or null if none</param>
        /// <param name="exception">The exception, or null if none</param>
        /// <returns>The error message</returns>
        public static ErrorMessage CreateErrorResponse(IMessage requestMessage, string faultString, string faultCode, Exception exception)
        {
            ErrorMessage message = new ErrorMessage();
            message.MessageId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
            message.Headers = EmptyDictionary<string, object>.Instance;
            message.ClientId = requestMessage.ClientId;
            message.Destination = requestMessage.Destination;
            message.CorrelationId = requestMessage.MessageId;
            message.FaultString = faultString;
            message.FaultCode = faultCode;
            if (exception != null)
                message.FaultDetail = exception.ToString();

            return message;
        }

        /// <summary>
        /// Extended data that the remote destination has chosen to associate with this error
        /// to facilitate custom error processing on the client.
        /// </summary>
        [ActionScriptProperty("extendedData")]
        public ASObject ExtendedData
        {
            get { return extendedData; }
            set { extendedData = value; }
        }

        /// <summary>
        /// The fault code for the error.
        /// This value typically follows the convention of "[outer_context].[inner_context].[issue]".
        /// For example: "Channel.Connect.Failed", "Server.Call.Failed", etc. 
        /// </summary>
        [ActionScriptProperty("faultCode")]
        public string FaultCode
        {
            get { return faultCode; }
            set { faultCode = value; }
        }

        /// <summary>
        /// Detailed description of what caused the error.
        /// This is typically a stack trace from the remote destination. 
        /// </summary>
        [ActionScriptProperty("faultDetail")]
        public string FaultDetail
        {
            get { return faultDetail; }
            set { faultDetail = value; }
        }

        /// <summary>
        /// A simple description of the error.
        /// </summary>
        [ActionScriptProperty("faultString")]
        public string FaultString
        {
            get { return faultString; }
            set { faultString = value; }
        }

        /// <summary>
        /// Should a root cause exist for the error, this property contains those details.
        /// This may be an ErrorMessage, a NetStatusEvent info Object,
        /// or an underlying Flash error event: ErrorEvent, IOErrorEvent, or SecurityErrorEvent. 
        /// </summary>
        [ActionScriptProperty("rootCause")]
        public ASObject RootCause
        {
            get { return rootCause; }
            set { rootCause = value; }
        }
    }
}