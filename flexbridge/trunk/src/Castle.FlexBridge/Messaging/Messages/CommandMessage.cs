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
    /// The CommandMessage class provides a mechanism for sending commands to the
    /// server infrastructure, such as commands related to publish/subscribe messaging
    /// scenarios, ping operations, and cluster operations. 
    /// </summary>
    /// <remarks>
    /// This is a built-in message type ported from Adobe Flex 2 (tm) libraries.
    /// Refer to Adobe Flex (tm) documentation for more details.
    /// </remarks>
    [ActionScriptClass(CommandMessage.ClassAlias)]
    public class CommandMessage : AsyncMessage
    {
        private string messageRefType;
        private OperationCode operation;

        /// <summary>
        /// Gets the class alias for <see cref="CommandMessage" />.
        /// </summary>
        public const string ClassAlias = "flex.messaging.messages.CommandMessage";

        /// <summary>
        /// The server message type for authentication commands.
        /// </summary>
        public const string AuthenticationMessageRefType = "flex.messaging.messages.AuthenticationMessage";

        /// <summary>
        /// Subscribe commands issued by a Consumer pass the Consumer's selector expression in this header.
        /// </summary>
        public const string SelectorHeader = "DSSelector";

        /// <summary>
        /// CommandMessage operation types.
        /// </summary>
        public enum OperationCode : uint
        {
            /// <summary>
            /// This operation is used to test connectivity over the current channel to
            /// the remote endpoint.
            /// </summary>
            ClientPing = 5,

            /// <summary>
            /// This operation is used by a remote destination to sync missed or cached messages back
            /// to a client as a result of a client issued poll command.
            /// </summary>
            ClientSync = 4,

            /// <summary>
            /// This operation is used to request a list of failover endpoint URIs for the remote
            /// destination based on cluster membership.
            /// </summary>
            ClusterRequest = 7,

            /// <summary>
            /// This operation is used to send credentials to the endpoint so that the user can be logged
            /// in over the current channel.
            /// The credentials need to be Base64 encoded and stored in the body of the message. 
            /// </summary>
            Login = 8,

            /// <summary>
            /// This operation is used to log the user out of the current channel, and will
            /// invalidate the server session if the channel is HTTP based.
            /// </summary>
            Logout = 9,

            /// <summary>
            /// This operation is used to poll a remote destination for pending, undelivered messages.
            /// </summary>
            Poll = 2,

            /// <summary>
            /// This operation is used to indicate that the client's session with a remote
            /// destination has timed out.
            /// </summary>
            SessionInvalidate = 10,

            /// <summary>
            /// This operation is used to subscribe to a remote destination.
            /// </summary>
            Subscribe = 0,

            /// <summary>
            /// This is the default operation for new CommandMessage instances.
            /// </summary>
            Unknown = 10000,

            /// <summary>
            /// This operation is used to unsubscribe from a remote destination. 
            /// </summary>
            Unsubscribe = 1
        }

        /// <summary>
        /// Targets the CommandMessage to a remote destination belonging to a specific service,
        /// based upon whether this server message type matches the message type the service handles.
        /// </summary>
        [ActionScriptProperty("messageRefType")]
        public string MessageRefType
        {
            get { return messageRefType; }
            set { messageRefType = value; }
        }

        /// <summary>
        /// Provides access to the operation/command for the CommandMessage.
        /// Operations indicate how this message should be processed by the remote destination. 
        /// </summary>
        [ActionScriptProperty("operation")]
        public OperationCode Operation
        {
            get { return operation; }
            set { operation = value; }
        }
    }
}
