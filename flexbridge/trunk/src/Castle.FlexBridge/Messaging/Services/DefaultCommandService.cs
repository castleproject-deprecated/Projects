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
using Castle.FlexBridge.Gateway;
using Castle.FlexBridge.Messaging.Messages;
using Castle.FlexBridge.Utilities;

namespace Castle.FlexBridge.Messaging.Services
{
    /// <summary>
    /// Handles <see cref="CommandMessage" /> when it is not handled by any other services.
    /// </summary>
    public sealed class DefaultCommandService : BaseService
    {
        /// <inheritdoc />
        public override bool OwnsMessageType(string messageClassAlias)
        {
            return messageClassAlias == CommandMessage.ClassAlias;
        }

        /// <inheritdoc />
        public override IAsyncResult BeginProcessRequest(IAMFContext context, IMessage request, AsyncCallback callback,
            object asyncState)
        {
            CommandMessage commandMessage = (CommandMessage)request;
            MessageProcessor processor = new MessageProcessor(context, commandMessage, this, callback, asyncState);
            processor.BeginTask();
            return processor;
        }

        /// <inheritdoc />
        public override IMessage EndProcessRequest(IAsyncResult asyncResult)
        {
            MessageProcessor processor = (MessageProcessor)asyncResult;
            return processor.EndTask();
        }

        /// <summary>
        /// Processes messages asynchronously.
        /// </summary>
        private sealed class MessageProcessor : EventAsyncResult
        {
            private IAMFContext context;
            private CommandMessage requestMessage;
            private IMessage responseMessage;
            private DefaultCommandService service;

            public MessageProcessor(IAMFContext context, CommandMessage requestMessage, DefaultCommandService service, AsyncCallback callback, object asyncState)
                : base(callback, asyncState)
            {
                this.context = context;
                this.requestMessage = requestMessage;
                this.service = service;
            }

            public void BeginTask()
            {
                switch (requestMessage.Operation)
                {
                    case CommandMessage.OperationCode.ClientPing:
                        responseMessage = AcknowledgeMessage.CreateAcknowledgeResponse(requestMessage, null);
                        SignalCompletion(true);
                        break;

                    default:
                        responseMessage = ErrorMessage.CreateErrorResponse(requestMessage,
                            String.Format(CultureInfo.CurrentCulture,
                            "Operation '{0}' is not supported by the default command service.", requestMessage.Operation),
                            "Gateway.DefaultCommandService.UnsupportedOperation", null);
                        SignalCompletion(true);
                        break;
                }
            }

            public IMessage EndTask()
            {
                WaitForCompletion();
                return responseMessage;
            }
        }
    }
}