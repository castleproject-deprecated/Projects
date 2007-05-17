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
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.Messaging.Messages
{
    /// <summary>
    /// AsyncMessage is the base class for all asynchronous messages. 
    /// </summary>
    /// <remarks>
    /// This is a built-in message type ported from Adobe Flex 2 (tm) libraries.
    /// Refer to Adobe Flex (tm) documentation for more details.
    /// </remarks>
    [ActionScriptClass("flex.messaging.messages.AsyncMessage")]
    public class AsyncMessage : AbstractMessage
    {
        private string correlationId;

        /// <summary>
        /// Messages sent by a MessageAgent with a defined subtopic property indicate their
        /// target subtopic in this header.
        /// </summary>
        public const string SubtopicHeader = "DSSubtopic";

        /// <summary>
        /// Provides access to the correlation id of the message.
        /// Used for acknowledgement and for segmentation of messages.
        /// The correlationId contains the messageId of the previous message that this message refers to. 
        /// </summary>
        [ActionScriptProperty("correlationId")]
        public string CorrelationId
        {
            get { return correlationId; }
            set { correlationId = value; }
        }
    }
}
