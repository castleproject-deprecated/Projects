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

using System.Collections.Generic;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.Messaging.DebugEvents
{
    /// <summary>
    /// A debug event containing Http headers.
    /// </summary>
    /// <todo>
    /// Decide whether to keep this class.
    /// NetDebug is only really useful with the NetConnection debugger in Adobe Flex (tm) 1.x.
    /// </todo>
    [ActionScriptClass]
    public sealed class HttpHeadersEvent : DebugEvent
    {
        private IDictionary<string, string> httpHeaders;

        /// <summary>
        /// Creates an Http headers event.
        /// </summary>
        public HttpHeadersEvent()
        {
            base.EventType = "HttpRequestHeaders";
            httpHeaders = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the dictionary of Http headers by name.
        /// </summary>
        [ActionScriptProperty("HttpHeaders")]
        public IDictionary<string, string> HttpHeaders
        {
            get { return httpHeaders; }
            set { httpHeaders = value; }
        }
    }
}