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
    /// A debug event containing trace messages.
    /// </summary>
    /// <todo>
    /// Decide whether to keep this class.
    /// NetDebug is only really useful with the NetConnection debugger in Adobe Flex (tm) 1.x.
    /// </todo>
    [ActionScriptClass]
    public sealed class TraceEvent : DebugEvent
    {
        private IList<object> messages;

        /// <summary>
        /// Creates a trace event.
        /// </summary>
        public TraceEvent()
        {
            base.EventType = "trace";
            messages = new List<object>();
        }

        /// <summary>
        /// Gets or sets the list of trace messages.
        /// </summary>
        [ActionScriptProperty("messages")]
        public IList<object> Messages
        {
            get { return messages; }
            set { messages = value; }
        }
    }
}