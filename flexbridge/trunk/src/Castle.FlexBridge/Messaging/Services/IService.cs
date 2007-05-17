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
using Castle.FlexBridge.Gateway;
using Castle.FlexBridge.Messaging.Messages;

namespace Castle.FlexBridge.Messaging.Services
{
    /// <summary>
    /// A service consumes a <see cref="IMessage" /> and produces an <see cref="IMessage" />
    /// in response.  The service identities the types of messages it owns by their
    /// class alias.  In addition to processing all <see cref="IMessage" /> objects of a
    /// supported type, the service is responsible for handling <see cref="CommandMessage" />
    /// messages whose <see cref="CommandMessage.MessageRefType" /> is a supported message type.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Returns true if the service ownss messages of the specified type.
        /// </summary>
        /// <param name="messageClassAlias">The class alias of the message type</param>
        /// <returns>True if the service owns the specified message type</returns>
        bool OwnsMessageType(string messageClassAlias);

        /// <summary>
        /// Begins asynchronous processing of a request message.
        /// </summary>
        /// <param name="context">The AMF context for the request</param>
        /// <param name="request">The request message</param>
        /// <param name="callback">The asynchronous callback</param>
        /// <param name="asyncState">Async state data for the <see cref="IAsyncResult" /></param>
        /// <returns>The async result object</returns>
        IAsyncResult BeginProcessRequest(IAMFContext context, IMessage request, AsyncCallback callback, object asyncState);

        /// <summary>
        /// Ends asynchronous processing of a request message.
        /// </summary>
        /// <param name="asyncResult">The <see cref="IAsyncResult" /> returned by a corresponding call to <see cref="BeginProcessRequest" /></param>
        /// <returns>The response message</returns>
        IMessage EndProcessRequest(IAsyncResult asyncResult);
    }
}
