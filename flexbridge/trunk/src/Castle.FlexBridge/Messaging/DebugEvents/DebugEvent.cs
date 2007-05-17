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
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.Messaging.DebugEvents
{
    /// <summary>
    /// Debug events are used by the NetConnection debugger.
    /// </summary>
    /// <todo>
    /// Decide whether to keep this class.
    /// NetDebug is only really useful with the NetConnection debugger in Adobe Flex (tm) 1.x.
    /// </todo>
    [ActionScriptClass]
    public abstract class DebugEvent
    {
        private string eventType;
        private string source;
        private DateTime date;
        private DateTime time;

        /// <summary>
        /// Creates a debug event.
        /// </summary>
        public DebugEvent()
        {
            eventType = "DebugEvent";
            source = "Server";
            time = DateTime.UtcNow;
            date = time.Date;
        }

        /// <summary>
        /// Gets or sets the debug event type.
        /// </summary>
        [ActionScriptProperty("EventType")]
        public string EventType
        {
            get { return eventType; }
            set { eventType = value; }
        }

        /// <summary>
        /// Gets or sets the source of the event.
        /// </summary>
        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        /// <summary>
        /// Gets or sets the date of the event.
        /// </summary>
        [ActionScriptProperty("Date")]
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        /// <summary>
        /// Gets or sets the time of the event.
        /// </summary>
        [ActionScriptProperty("Time")]
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
    }
}
