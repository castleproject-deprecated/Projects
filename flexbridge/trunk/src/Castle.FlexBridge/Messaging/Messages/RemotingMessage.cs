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
    /// RemotingMessages are used to send RPC requests to a remote endpoint.
    /// These messages use the operation property to specify which method to call on the remote object.
    /// The destination property indicates what object/service should be used. 
    /// </summary>
    /// <remarks>
    /// This is A built-in Flex Message type ported from Flex 2 libraries.
    /// Refer to Flex documentation for more details.
    /// </remarks>
    [ActionScriptClass(RemotingMessage.ClassAlias)]
    public class RemotingMessage : AbstractMessage
    {
        /// <summary>
        /// Gets the class alias for <see cref="RemotingMessage" />.
        /// </summary>
        public const string ClassAlias = "flex.messaging.messages.RemotingMessage";

        private string operation;
        private string source;

        /// <summary>
        /// Provides access to the name of the remote method/operation that should be called.
        /// </summary>
        [ActionScriptProperty("operation")]
        public string Operation
        {
            get { return operation; }
            set { operation = value; }
        }

        /// <summary>
        /// This property is provided for backwards compatibility.
        /// </summary>
        /// <remarks>
        /// The best practice, however, is to not expose the underlying source of a RemoteObject
        /// destination on the client and only one source to a destination.  Some types of Remoting
        /// Services may even ignore this property for security reasons.
        /// </remarks>
        [ActionScriptProperty("source")]
        public string Source
        {
            get { return source; }
            set { source = value; }
        }
    }
}
