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

namespace Castle.FlexBridge.Serialization.AMF
{
    /// <summary>
    /// Represents an AMF body in an <see cref="AMFMessage" />.
    /// </summary>
    public class AMFBody
    {
        private string requestTarget;
        private string responseTarget;
        private IASValue content;

        /// <summary>
        /// Creates an uninitialized AMF body.
        /// </summary>
        public AMFBody()
        {
        }

        /// <summary>
        /// Creates an AMF body.
        /// </summary>
        /// <param name="requestTarget">The target of the message body</param>
        /// <param name="responseTarget">The response target of the message body, or the string "null" if none</param>
        /// <param name="content">The content of the body</param>
        public AMFBody(string requestTarget, string responseTarget, IASValue content)
        {
            this.requestTarget = requestTarget;
            this.responseTarget = responseTarget;
            this.content = content;
        }

        /// <summary>
        /// Gets or sets the target of the message body.
        /// The request target specifies the service to which the request body content should be sent.
        /// </summary>
        public string RequestTarget
        {
            get { return requestTarget; }
            set { requestTarget = value; }
        }

        /// <summary>
        /// Gets or sets the response target of the message body, or the string "null" if none.
        /// The response target specifies the service to which the response body content should be sent.
        /// When sending a response, a new <see cref="AMFBody" /> is created with the response target
        /// specified as the new request target and the response body content enclosed.
        /// </summary>
        public string ResponseTarget
        {
            get { return responseTarget; }
            set { responseTarget = value; }
        }

        /// <summary>
        /// Gets or sets the content of the body.
        /// </summary>
        public IASValue Content
        {
            get { return content; }
            set { content = value; }
        }
    }
}
