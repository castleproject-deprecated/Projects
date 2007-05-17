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
using Castle.FlexBridge.Collections;
using Castle.FlexBridge.Gateway;
using Castle.FlexBridge.Gateway.RequestProcessors;
using Castle.FlexBridge.Messaging.Messages;
using Castle.FlexBridge.Messaging.Services;
using Castle.FlexBridge.Serialization;
using Castle.FlexBridge.Utilities;

namespace Castle.FlexBridge.Messaging
{
    /// <summary>
    /// Default implementation of the message broker.
    /// </summary>
    public class DefaultMessageBroker : IMessageBroker
    {
        private readonly object syncRoot = new object();

        private List<IService> services;
        private Dictionary<string, IService> servicesByClassAlias;

        /// <summary>
        /// Creates a message broker.
        /// </summary>
        /// <param name="registerBuiltInServices">If true, automatically registers all built-in services</param>
        public DefaultMessageBroker(bool registerBuiltInServices)
        {
            services = new List<IService>();
            servicesByClassAlias = new Dictionary<string, IService>();

            if (registerBuiltInServices)
                RegisterBuiltInServices();
        }

        public IAsyncResult BeginProcessRequest(IAMFContext context, IMessage request, AsyncCallback callback,
            object asyncState)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (request == null)
                throw new ArgumentNullException("request");

            IService service = UncheckedGetServiceForMessage(context.Serializer, request);

            MessageProcessor processor = new MessageProcessor(context, request, service, callback, asyncState);
            processor.BeginTask();
            return processor;
        }

        public IMessage EndProcessRequest(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");

            MessageProcessor processor = (MessageProcessor) asyncResult;
            return processor.EndTask();
        }

        public void RegisterService(IService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            lock (syncRoot)
            {
                services.Add(service);
            }
        }

        public IService GetServiceForMessageClassAlias(string messageClassAlias)
        {
            if (messageClassAlias == null)
                throw new ArgumentNullException("messageClassAlias");

            return UncheckedGetServiceForMessageClassAlias(messageClassAlias);
        }

        private IService UncheckedGetServiceForMessageClassAlias(string messageClassAlias)
        {
            if (messageClassAlias.Length == 0)
                return null;

            lock (syncRoot)
            {
                IService cachedService;
                if (servicesByClassAlias.TryGetValue(messageClassAlias, out cachedService))
                    return cachedService;

                foreach (IService registeredService in services)
                {
                    if (registeredService.OwnsMessageType(messageClassAlias))
                    {
                        servicesByClassAlias.Add(messageClassAlias, registeredService);
                        return registeredService;
                    }
                }

                return null;
            }
        }

        public IService GetServiceForMessage(IActionScriptSerializer serializer, IMessage message)
        {
            if (serializer == null)
                throw new ArgumentNullException("serializer");
            if (message == null)
                throw new ArgumentNullException("message");

            return UncheckedGetServiceForMessage(serializer, message);
        }

        private IService UncheckedGetServiceForMessage(IActionScriptSerializer serializer, IMessage message)
        {
            IService service;

            CommandMessage commandMessage = message as CommandMessage;
            if (commandMessage != null && ! String.IsNullOrEmpty(commandMessage.MessageRefType))
            {
                service = UncheckedGetServiceForMessageClassAlias(commandMessage.MessageRefType);
                if (service != null)
                    return service;
            }

            string classAlias = serializer.GetClassAlias(message.GetType());
            service = UncheckedGetServiceForMessageClassAlias(classAlias);
            return service;
        }

        private void RegisterBuiltInServices()
        {
            RegisterService(new RemotingService());
            RegisterService(new AuthenticationService());
            RegisterService(new DefaultCommandService());
        }

        /// <summary>
        /// Processes messages asynchronously.
        /// </summary>
        private sealed class MessageProcessor : EventAsyncResult
        {
            private IAMFContext context;
            private IMessage requestMessage;
            private IMessage responseMessage;
            private IService service;

            public MessageProcessor(IAMFContext context, IMessage requestMessage, IService service, AsyncCallback callback, object asyncState)
                : base(callback, asyncState)
            {
                this.context = context;
                this.requestMessage = requestMessage;
                this.service = service;
            }

            public void BeginTask()
            {
                if (service == null)
                {
                    responseMessage = ErrorMessage.CreateErrorResponse(requestMessage,
                        "No service was found to process the message.",
                        "Gateway.MessageBroker.UnsupportedService", null);

                    SignalCompletion(true);
                    return;
                }

                try
                {
                    service.BeginProcessRequest(context, requestMessage, OnAsyncResult, null);
                }
                catch (Exception ex)
                {
                    responseMessage = ErrorMessage.CreateErrorResponse(requestMessage,
                        "An error occurred while a service was processing the message.",
                        "Gateway.MessageBroker.ServiceBeginProcessRequestFailed", ex);
                    SignalCompletion(true);
                }
            }

            public IMessage EndTask()
            {
                WaitForCompletion();
                return responseMessage;
            }

            private void OnAsyncResult(IAsyncResult asyncResult)
            {
                // If we already generated an error message of some sort, then ignore
                // this result.
                if (responseMessage != null)
                    return;

                try
                {
                    responseMessage = service.EndProcessRequest(asyncResult);
                }
                catch (Exception ex)
                {
                    responseMessage = ErrorMessage.CreateErrorResponse(requestMessage,
                        "An error occurred while a service was processing the message.",
                        "Gateway.MessageBroker.ServiceEndProcessRequestFailed", ex);
                }

                SignalCompletion(asyncResult.CompletedSynchronously);
            }
        }
    }
}
