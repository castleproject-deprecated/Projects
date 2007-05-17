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
    /// Represents an AMF header of an <see cref="AMFMessage" />.
    /// </summary>
    public class AMFHeader
    {
        private string name;
        private bool mustUnderstand;
        private IASValue content;

        /// <summary>
        /// Creates an uninitialized AMF header.
        /// </summary>
        public AMFHeader()
        {
        }

        /// <summary>
        /// Creates an AMF header.
        /// </summary>
        /// <param name="name">The name of the header</param>
        /// <param name="mustUnderstand">A flag that indicates whether the remote host must understand the header</param>
        /// <param name="content">The content of the header</param>
        public AMFHeader(string name, bool mustUnderstand, IASValue content)
        {
            this.name = name;
            this.mustUnderstand = mustUnderstand;
            this.content = content;
        }

        /// <summary>
        /// Gets or sets the header name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets a flag that indicates whether the remote host must understand the header.
        /// </summary>
        public bool MustUnderstand
        {
            get { return mustUnderstand; }
            set { mustUnderstand = value; }
        }

        /// <summary>
        /// Gets or sets the content of the header.
        /// </summary>
        public IASValue Content
        {
            get { return content; }
            set { content = value; }
        }
    }
}
