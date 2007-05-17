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
using System.Globalization;
using System.Text;
using Castle.FlexBridge.Collections;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.Messaging.Messages
{
    /// <summary>
    /// An AcknowledgeMessage acknowledges the receipt of a message that was sent previously.
    /// Every message sent within the messaging system must receive an acknowledgement.
    /// </summary>
    /// <remarks>
    /// This is a built-in message type ported from Adobe Flex 2 (tm) libraries.
    /// Refer to Adobe Flex (tm) documentation for more details.
    /// </remarks>
    [ActionScriptClass("flex.messaging.messages.AcknowledgeMessage")]
    public class AcknowledgeMessage : AsyncMessage
    {
        /// <summary>
        /// Header name for the error hint header.
        /// Used to indicate that the acknowledgement is for a message that
        /// generated an error. 
        /// </summary>
        public const string ErrorHintHeader = "DSErrorHint";

        /// <summary>
        /// Creates an acknowledge message in response to a request.
        /// </summary>
        /// <param name="requestMessage">The original request message</param>
        /// <param name="body">The message body</param>
        /// <returns>The acknowledge message</returns>
        public static AcknowledgeMessage CreateAcknowledgeResponse(IMessage requestMessage, object body)
        {
            AcknowledgeMessage message = new AcknowledgeMessage();
            message.MessageId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
            message.Headers = EmptyDictionary<string, object>.Instance;
            message.Body = body;
            message.ClientId = requestMessage.ClientId;
            message.Destination = requestMessage.Destination;
            message.CorrelationId = requestMessage.MessageId;

            return message;
        }
    }
}
