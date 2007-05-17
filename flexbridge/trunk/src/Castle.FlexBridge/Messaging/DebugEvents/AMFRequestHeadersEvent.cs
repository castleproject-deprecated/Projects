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
    /// A debug event describing the AMF request headers of an remote service call.
    /// </summary>
    /// <todo>
    /// Decide whether to keep this class.
    /// NetDebug is only really useful with the NetConnection debugger in Adobe Flex (tm) 1.x.
    /// </todo>
    [ActionScriptClass]
    public sealed class AMFRequestHeadersEvent : DebugEvent
    {
        private bool mustUnderstand;
        private IDictionary<string, object> amfHeader;

        /// <summary>
        /// Creates a method call event.
        /// </summary>
        public AMFRequestHeadersEvent()
        {
            base.EventType = "AmfRequestHeaders";
            amfHeader = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the value of the MustUnderstand flag of the original header.
        /// </summary>
        [ActionScriptProperty("MustUnderstand")]
        public bool MustUnderstand
        {
            get { return mustUnderstand; }
            set { mustUnderstand = value; }
        }

        /// <summary>
        /// Gets or sets the contents of the header, represented as a one-element dictionary with
        /// the header name as key and the header contents as value.
        /// </summary>
        [ActionScriptProperty("AmfHeader")]
        public IDictionary<string, object> AMFHeader
        {
            get { return amfHeader; }
            set { amfHeader = value; }
        }
    }
}