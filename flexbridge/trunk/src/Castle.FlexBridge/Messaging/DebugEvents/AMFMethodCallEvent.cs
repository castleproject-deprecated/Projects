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
    /// A debug event describing a method call that was performed.
    /// </summary>
    /// <todo>
    /// Decide whether to keep this class.
    /// NetDebug is only really useful with the NetConnection debugger in Adobe Flex (tm) v1.x.
    /// </todo>
    [ActionScriptClass]
    public sealed class AMFMethodCallEvent : DebugEvent
    {
        private string targetUri;
        private string responseUri;
        private string type;
        private string classPath;
        private string className;
        private string methodName;
        private object[] parameters;

        /// <summary>
        /// Creates a method call event.
        /// </summary>
        public AMFMethodCallEvent()
        {
            base.EventType = "AmfMethodCall";
        }

        /// <summary>
        /// Gets or sets the Uri of the target method.
        /// </summary>
        [ActionScriptProperty("TargetURI")]
        public string TargetUri
        {
            get { return targetUri; }
            set { targetUri = value; }
        }

        /// <summary>
        /// Gets or sets the Uri of the target response.
        /// </summary>
        [ActionScriptProperty("ResponseURI")]
        public string ResponseUri
        {
            get { return responseUri; }
            set { responseUri = value; }
        }

        /// <summary>
        /// The response content type.
        /// </summary>
        [ActionScriptProperty("Type")]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// The class path of the requested service.
        /// </summary>
        [ActionScriptProperty("ClassPath")]
        public string ClassPath
        {
            get { return classPath; }
            set { classPath = value; }
        }
        
        /// <summary>
        /// The class name of the requested service.
        /// </summary>
        [ActionScriptProperty("ClassName")]
        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        /// <summary>
        /// The method name of the requested service.
        /// </summary>
        [ActionScriptProperty("MethodName")]
        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }

        /// <summary>
        /// The parameters of the requested service.
        /// </summary>
        [ActionScriptProperty("Parameters")]
        public object[] Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }
    }
}
