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
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.Messaging.Messages
{
    /// <summary>
    /// Abstract base class for all messages.
    /// 
    /// Messages have two customizable sections; headers and body.  The headers property provides
    /// access to specialized meta information for a specific message instance.  The headers property
    /// is an associative array with the specific header name as the key. 
    /// The body of a message contains the instance specific data that needs to be delivered and
    /// processed by the remote destination. The body is an object and is the payload for a message. 
    /// </summary>
    /// <remarks>
    /// This is a built-in message type ported from Adobe Flex 2 (tm) libraries.
    /// Refer to Adobe Flex (tm) documentation for more details.
    /// </remarks>
    [ActionScriptClass("flex.messaging.messages.AbstractMessage")]
    public class AbstractMessage : IMessage
    {
        private object body;
        private string clientId;
        private string destination;
        private IDictionary<string, object> headers;
        private string messageId;
        private double timestamp;
        private double timeToLive;

        /// <summary>
        /// Messages pushed from the server may arrive in a batch, with messages in the
        /// batch potentially targeted to different Consumer instances.
        /// Each message will contain this header identifying the Consumer instance
        /// that will receive the message. 
        /// </summary>
        public const string DestinationClientIdHeader = "DSDstClientId";

        /// <summary>
        /// Messages are tagged with the endpoint id for the Channel they are sent over.
        /// Channels set this value automatically when they send a message. 
        /// </summary>
        public const string EndpointHeader = "DSEndpoint";

        /// <summary>
        /// Messages that need to set remote credentials for a destination carry the
        /// Base64 encoded credentials in this header.
        /// </summary>
        public const string RemoteCredentialsHeader = "DSRemoteCredentials";

        /// <summary>
        /// Messages sent with a defined request timeout use this header.
        /// The request timeout value is set on outbound messages by services or channels
        /// and the value controls how long the corresponding MessageResponder will wait
        /// for an acknowledgement, result or fault response for the message before
        /// timing out the request. 
        /// </summary>
        public const string RequestTimeoutHeader = "DSRequestTimeout";

        /// <summary>
        /// The body of a message contains the specific data that needs to be delivered to the remote destination.
        /// </summary>
        [ActionScriptProperty("body")]
        public object Body
        {
            get { return body; }
            set { body = value; }
        }

        /// <summary>
        /// The clientId indicates which MessageAgent sent the message.
        /// </summary>
        [ActionScriptProperty("clientId")]
        public string ClientId
        {
            get { return clientId; }
            set { clientId = value; }
        }

        /// <summary>
        /// The message destination.
        /// </summary>
        [ActionScriptProperty("destination")]
        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        /// <summary>
        /// Provides access to the headers of the message.
        /// The headers of a message are an associative array where the key is the
        /// header name and the value is the header value.
        /// </summary>
        [ActionScriptProperty("headers")]
        public IDictionary<string, object> Headers
        {
            get { return headers; }
            set { headers = value; }
        }

        /// <summary>
        /// The unique id for the message.
        /// The message id can be used to correlate a response to the original request
        /// message in request-response messaging scenarios. 
        /// </summary>
        [ActionScriptProperty("messageId")]
        public string MessageId
        {
            get { return messageId; }
            set { messageId = value; }
        }

        /// <summary>
        /// Provides access to the time stamp for the message.
        /// A time stamp is the date and time that the message was sent.
        /// The time stamp is used for tracking the message through the system,
        /// ensuring quality of service levels and providing a mechanism for expiration. 
        /// </summary>
        [ActionScriptProperty("timestamp")]
        public double Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        /// <summary>
        /// The time to live value of a message indicates how long the message should be
        /// considered valid and deliverable. This value works in conjunction with the
        /// timestamp value. Time to live is the number of milliseconds that this message
        /// remains valid starting from the specified timestamp value. For example, if
        /// the timestamp value is 04/05/05 1:30:45 PST and the timeToLive value is 5000,
        /// then this message will expire at 04/05/05 1:30:50 PST. Once a message expires
        /// it will not be delivered to any other clients. 
        /// </summary>
        [ActionScriptProperty("timeToLive")]
        public double TimeToLive
        {
            get { return timeToLive; }
            set { timeToLive = value; }
        }
    }
}
