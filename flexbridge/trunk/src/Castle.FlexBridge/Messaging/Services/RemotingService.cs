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
using Castle.FlexBridge.Utilities;

namespace Castle.FlexBridge.Messaging.Services
{
    /// <summary>
    /// Handles <see cref="RemotingMessage" />.
    /// </summary>
    public sealed class RemotingService : BaseService
    {
        /// <inheritdoc />
        public override bool OwnsMessageType(string messageClassAlias)
        {
            return messageClassAlias == RemotingMessage.ClassAlias;
        }

        /// <inheritdoc />
        public override IAsyncResult BeginProcessRequest(IAMFContext context, IMessage request, AsyncCallback callback, object asyncState)
        {
            MessageProcessor processor = new MessageProcessor(context, request, this, callback, asyncState);
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
            private IMessage requestMessage;
            private IMessage responseMessage;
            private RemotingService service;

            public MessageProcessor(IAMFContext context, IMessage requestMessage, RemotingService service, AsyncCallback callback, object asyncState)
                : base(callback, asyncState)
            {
                this.context = context;
                this.requestMessage = requestMessage;
                this.service = service;
            }

            public void BeginTask()
            {
                CommandMessage commandMessage = requestMessage as CommandMessage;
                if (commandMessage != null)
                {
                    responseMessage = ErrorMessage.CreateErrorResponse(requestMessage,
                        "Remoting service does not handle command messages currently.",
                        "Gateway.RemotingService.UnsupportedCommandMessage", null);
                    SignalCompletion(true);
                    return;
                }

                RemotingMessage remotingMessage = (RemotingMessage) requestMessage;
                responseMessage = AcknowledgeMessage.CreateAcknowledgeResponse(remotingMessage, new object[] { "Response!" });
                SignalCompletion(true);
            }

            public IMessage EndTask()
            {
                WaitForCompletion();
                return responseMessage;
            }
        }
    }
}
