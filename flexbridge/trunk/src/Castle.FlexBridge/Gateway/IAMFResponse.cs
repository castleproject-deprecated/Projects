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

using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Messaging.DebugEvents;
using Castle.FlexBridge.Messaging.Messages;
using Castle.FlexBridge.Serialization.AMF;
using System;

namespace Castle.FlexBridge.Gateway
{
    /// <summary>
    /// Contains information about an AMF response being generated.
    /// </summary>
    public interface IAMFResponse
    {
        /// <summary>
        /// Gets the AMF response message being constructed.
        /// May be null if the message is not currently available.
        /// </summary>
        AMFMessage Message { get; }

        /// <summary>
        /// Adds a successful result response.
        /// </summary>
        /// <param name="responseTarget">The response target</param>
        /// <param name="content">The content of the response</param>
        void AddResultResponse(string responseTarget, object content);

        /// <summary>
        /// Adds an error response with an error message string and an optional exception.
        /// </summary>
        /// <param name="responseTarget">The response target</param>
        /// <param name="errorMessage">The error message string</param>
        /// <param name="exception">The exception, or null if none</param>
        void AddErrorResponse(string responseTarget, string errorMessage, Exception exception);

        /// <summary>
        /// Adds an error response with an error message object.
        /// </summary>
        /// <param name="responseTarget">The response target</param>
        /// <param name="errorMessage">The error message object</param>
        void AddErrorResponse(string responseTarget, ErrorMessage errorMessage);

        /// <summary>
        /// Adds a debug response.
        /// </summary>
        /// <param name="responseTarget">The response target</param>
        /// <param name="debugEvent">The debug event</param>
        void AddDebugEventResponse(string responseTarget, DebugEvent debugEvent);

        /// <summary>
        /// Adds a response body.
        /// </summary>
        /// <param name="body">The body to add</param>
        void AddBody(AMFBody body);
    }
}