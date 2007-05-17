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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Castle.FlexBridge.Gateway.RequestProcessors;
using Castle.FlexBridge.Messaging;
using Castle.FlexBridge.Messaging.Messages;
using Castle.FlexBridge.Serialization;
using Castle.FlexBridge.Serialization.AMF;

namespace Castle.FlexBridge.Gateway
{
    /// <summary>
    /// The FlexGateway is an ASP.Net HttpHandler that accepts HTTP-encoded requests from
    /// Adobe Flex (tm) applications and submits them to the message broker to be serviced.
    /// </summary>
    /// <remarks>
    /// How to enable the FlexGateway in your application:
    /// 
    /// <list type="numbered">
    /// <item>
    /// Create a file named FlexGateway.ashx in your ASP.Net web application with
    /// the following contents:
    /// <code>
    /// &lt;%@ WebHandler Language="C#" Class="Castle.FlexBridge.Gateway.FlexGateway" %&gt;
    /// </code>
    /// </item>
    /// <item>
    /// Create a folder named WEB-INF\flex in the root of your ASP.Net application.  Within it
    /// create a file named services-config.xml.  Consult the Adobe Flex (tm) documentation
    /// or the examples for information on the structure and contents of this file.
    /// </item>
    /// <item>
    /// Configure the channels within the services-config.xml to point to the FlexGateway.
    /// Assuming you created the FlexGateway.ashx in the root of your ASP.Net web application,
    /// the endpoint uri should be "http://{server.name}:{server.port}/{context.root}/FlexGateway.ashx".
    /// Use "https" to create secure channels.
    /// Note that Adobe Flex (tm) will automatically substitute the value of the brace-delimited tokens
    /// when constructing the Url so this default uri is usually sufficient.  However, you may
    /// customize the uri in any way you like so long as it refers to a resource on your
    /// web server that causes the FlexGateway to be invoked.
    /// </item>
    /// </list>
    /// </remarks>
    public class FlexGateway : IHttpAsyncHandler
    {
        private MappedActionScriptSerializerFactory serializerFactory;
        private IMessageBroker messageBroker;

        /// <summary>
        /// Initializes the gateway.
        /// </summary>
        public FlexGateway()
        {
            // TODO: Should construct and initialize these services based on configuration values.
            serializerFactory = new MappedActionScriptSerializerFactory(true);
            messageBroker = new DefaultMessageBroker(true);
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext httpContext, AsyncCallback callback, object asyncState)
        {
            if (httpContext.Request.ContentType == AMFRequestProcessor.AMFContentType)
            {
                AMFRequestProcessor processor = new AMFRequestProcessor(httpContext, serializerFactory, messageBroker, callback, asyncState);
                processor.BeginTask();
                return processor;
            }
            else
            {
                UnknownRequestProcessor processor = new UnknownRequestProcessor(httpContext, callback, asyncState);
                processor.BeginTask();
                return processor;
            }
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            AsyncTask task = (AsyncTask) result;
            task.EndTask();
        }

        public void ProcessRequest(HttpContext httpContext)
        {
            EndProcessRequest(BeginProcessRequest(httpContext, null, null));
        }

        /// <summary>
        /// Processes AMFMessage requests.
        /// </summary>
        /// <todo>
        /// Should represent the processing chain as a sequence of filters or handlers.
        /// </todo>
        private sealed class AMFRequestProcessor : AsyncTask
        {
            public const string AMFContentType = "application/x-amf";

            private IMessageBroker messageBroker;
            private GatewayAMFContext context;

            public AMFRequestProcessor(HttpContext httpContext,
                IActionScriptSerializerFactory serializerFactory,
                IMessageBroker messageBroker, AsyncCallback callback, object asyncState)
                : base(callback, asyncState)
            {
                this.messageBroker = messageBroker;

                context = new GatewayAMFContext(httpContext, serializerFactory);
            }

            protected override void OnBeginTask()
            {
                AMFContextHolder.Current = context;
            }

            protected override void OnEndTask(Exception exception)
            {
                AMFContextHolder.Current = null;
            }

            protected override IEnumerable<Step> GetSteps()
            {
                // Set properties from context.
                context.Request.IsSecureConnection = context.HttpContext.Request.IsSecureConnection;

                // Read the AMF message and prepare the response context.
                AMFDataInput dataInput = new AMFDataInput(context.HttpContext.Request.InputStream, context.Serializer);
                AMFMessageReader reader = new AMFMessageReader(dataInput);

                AMFMessage amfRequestMessage = reader.ReadAMFMessage();
                context.Request.SetMessage(amfRequestMessage);
                context.Response.PrepareResponseMessage(amfRequestMessage);

                // Process all message bodies.
                foreach (AMFBody requestBody in amfRequestMessage.Bodies)
                {
                    object nativeContent;
                    try
                    {
                        nativeContent = context.Serializer.ToNative(requestBody.Content, null);
                    }
                    catch (Exception ex)
                    {
                        context.Response.AddErrorResponse(requestBody.ResponseTarget,
                            "An error occurred while deserializing the request body.", ex);
                        continue;
                    }

                    if (requestBody.RequestTarget == "null")
                    {
                        IMessage requestMessage = nativeContent as IMessage;
                        if (requestMessage == null)
                        {
                            object[] array = nativeContent as object[];
                            if (array != null)
                                requestMessage = array[0] as IMessage;

                            if (requestMessage != null)
                            {
                                IAsyncResult result;
                                try
                                {
                                    result = messageBroker.BeginProcessRequest(context, requestMessage, OnAsyncResultCallbackResume, null);
                                }
                                catch (Exception ex)
                                {
                                    ErrorMessage errorMessage = ErrorMessage.CreateErrorResponse(requestMessage,
                                        "An error occurred while the message broker was processing the message.",
                                        "Gateway.MessageBroker.BeginProcessRequestFailed",
                                        ex);

                                    context.Response.AddErrorResponse(requestBody.ResponseTarget, errorMessage);
                                    continue;
                                }

                                yield return new Step("Waiting for message broker.");

                                try
                                {
                                    IMessage responseMessage = messageBroker.EndProcessRequest(result);
                                    context.Response.AddResultResponse(requestBody.ResponseTarget, responseMessage);
                                }
                                catch (Exception ex)
                                {
                                    ErrorMessage errorMessage = ErrorMessage.CreateErrorResponse(requestMessage,
                                        "An error occurred while the message broker was processing the message.",
                                        "Gateway.MessageBroker.EndProcessRequestFailed",
                                        ex);

                                    context.Response.AddErrorResponse(requestBody.ResponseTarget, errorMessage);
                                }

                                continue;
                            }
                        }
                    }

                    // Don't know what to do with this message.
                    context.Response.AddErrorResponse(requestBody.ResponseTarget,
                        String.Format(CultureInfo.CurrentCulture,
                        "Received a message for unsupported request target '{0}'.", requestBody.RequestTarget), null);
                }

                // Generate the response.
                AMFDataOutput dataOutput = new AMFDataOutput(context.HttpContext.Response.OutputStream, context.Serializer);
                AMFMessageWriter writer = new AMFMessageWriter(dataOutput);

                writer.WriteAMFMessage(context.Response.Message);

                context.HttpContext.Response.ContentType = AMFContentType;
                context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                yield break;
            }
        }

        /// <summary>
        /// Processes unrecognized requests.
        /// </summary>
        private sealed class UnknownRequestProcessor : AsyncTask
        {
            private HttpContext httpContext;

            public UnknownRequestProcessor(HttpContext httpContext,
                AsyncCallback callback, object asyncState)
                : base(callback, asyncState)
            {
                this.httpContext = httpContext;
            }

            protected override IEnumerable<Step> GetSteps()
            {
                httpContext.Response.StatusCode = 400;
                httpContext.Response.StatusDescription = "The Castle.FlexBridge Gateway does not understand this type of request.";
                httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                httpContext.Response.End();
                yield break;
            }
        }
    }
}
