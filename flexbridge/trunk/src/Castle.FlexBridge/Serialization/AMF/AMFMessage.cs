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

namespace Castle.FlexBridge.Serialization.AMF
{
    /// <summary>
    /// Represents an AMF message.
    /// An AMF message may have any number of header or body sections.
    /// </summary>
    public class AMFMessage
    {
        private ushort version;
        private IList<AMFHeader> headers;
        private IList<AMFBody> bodies;

        /// <summary>
        /// Constructs an empty AMF message.
        /// </summary>
        public AMFMessage()
        {
            headers = new List<AMFHeader>();
            bodies = new List<AMFBody>();
        }

        /// <summary>
        /// Constructs an AMF message.
        /// </summary>
        /// <param name="version">The version code</param>
        /// <param name="headers">The list of AMF headers</param>
        /// <param name="bodies">The list of AMF bodies</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="headers"/>
        /// or <paramref name="bodies"/> is null</exception>
        public AMFMessage(ushort version, IList<AMFHeader> headers, IList<AMFBody> bodies)
        {
            if (headers == null)
                throw new ArgumentNullException("headers");
            if (bodies == null)
                throw new ArgumentNullException("bodies");

            this.version = version;
            this.headers = headers;
            this.bodies = bodies;
        }

        /// <summary>
        /// Gets or sets the version code.
        /// </summary>
        public ushort Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// Gets the list of AMF headers.
        /// </summary>
        public IList<AMFHeader> Headers
        {
            get { return headers; }
        }

        /// <summary>
        /// Gets the list of AMF bodies.
        /// </summary>
        public IList<AMFBody> Bodies
        {
            get { return bodies; }
        }
    }
}
