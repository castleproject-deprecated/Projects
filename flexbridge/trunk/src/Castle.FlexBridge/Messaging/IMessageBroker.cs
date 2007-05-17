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
using Castle.FlexBridge.Messaging.Services;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.Messaging
{
    /// <summary>
    /// Processes flex messages.
    /// </summary>
    public interface IMessageBroker
    {
        /// <summary>
        /// Begins asynchronous processing of a request message.
        /// </summary>
        /// <param name="context">The AMF context for the request</param>
        /// <param name="request">The request message</param>
        /// <param name="callback">The asynchronous callback</param>
        /// <param name="asyncState">Async state data for the <see cref="IAsyncResult" /></param>
        /// <returns>The async result object</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="request"/>
        /// is null</exception>
        IAsyncResult BeginProcessRequest(IAMFContext context, IMessage request, AsyncCallback callback, object asyncState);

        /// <summary>
        /// Ends asynchronous processing of a request message.
        /// </summary>
        /// <param name="asyncResult">The <see cref="IAsyncResult" /> returned by a corresponding call to <see cref="BeginProcessRequest" /></param>
        /// <returns>The response message</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncResult"/> is null</exception>
        IMessage EndProcessRequest(IAsyncResult asyncResult);

        /// <summary>
        /// Registers a service.
        /// </summary>
        /// <param name="service">The service to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> is null</exception>
        void RegisterService(IService service);

        /// <summary>
        /// Get the service that handles messages of the specified type.
        /// </summary>
        /// <param name="messageClassAlias">The class alias of the message type</param>
        /// <returns>The service, or null if none is registered that handles messages with the specified class alias</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="messageClassAlias"/> is null</exception>
        IService GetServiceForMessageClassAlias(string messageClassAlias);

        /// <summary>
        /// Gets the service that is responsible for handling the specified message.
        /// </summary>
        /// <param name="serializer">The serializer (used to discover the message type's class alias)</param>
        /// <param name="message">The message</param>
        /// <returns>The service, or null if none is registered that handles messages of the specified type</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serializer"/> or <paramref name="message"/> is null</exception>
        IService GetServiceForMessage(IActionScriptSerializer serializer, IMessage message);
    }
}
