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
using System.Web;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.Gateway
{
    /// <summary>
    /// An AMF context contains information about an AMF request in process.
    /// </summary>
    internal class GatewayAMFContext : IAMFContext
    {
        private HttpContext httpContext;
        private GatewayAMFRequest request;
        private GatewayAMFResponse response;
        private IActionScriptSerializerFactory serializerFactory;

        private IActionScriptSerializer serializer;

        /// <summary>
        /// Creates an AMFContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext of the request, or null if not applicable</param>
        /// <param name="serializerFactory">The ActionScript serializer factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serializerFactory"/> is null</exception>
        public GatewayAMFContext(HttpContext httpContext, IActionScriptSerializerFactory serializerFactory)
        {
            if (serializerFactory == null)
                throw new ArgumentNullException("serializerFactory");

            this.httpContext = httpContext;
            this.serializerFactory = serializerFactory;

            request = new GatewayAMFRequest(this);
            response = new GatewayAMFResponse(this);
        }

        public HttpContext HttpContext
        {
            get { return httpContext; }
        }

        /// <summary>
        /// Gets the AMF request.
        /// </summary>
        public GatewayAMFRequest Request
        {
            get { return request; }
        }

        IAMFRequest IAMFContext.Request
        {
            get { return request; }
        }

        /// <summary>
        /// Gets the AMF response.
        /// </summary>
        public GatewayAMFResponse Response
        {
            get { return response; }
        }

        IAMFResponse IAMFContext.Response
        {
            get { return response; }
        }

        /// <summary>
        /// Gets the ActionScript serializer for the request.
        /// </summary>
        public IActionScriptSerializer Serializer
        {
            get
            {
                lock (this)
                {
                    if (serializer == null)
                        serializer = serializerFactory.CreateSerializer();
                    return serializer;
                }
            }
        }
    }
}
