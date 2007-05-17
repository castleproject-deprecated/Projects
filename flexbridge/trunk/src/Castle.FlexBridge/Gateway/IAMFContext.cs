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

using System.Web;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.Gateway
{
    /// <summary>
    /// An AMF context contains information about an AMF request in process.
    /// </summary>
    public interface IAMFContext
    {
        /// <summary>
        /// Gets the HttpContext of the request, or null if not applicable.
        /// </summary>
        HttpContext HttpContext { get; }

        /// <summary>
        /// Gets the AMF request.
        /// </summary>
        IAMFRequest Request { get; }

        /// <summary>
        /// Gets the AMF response.
        /// </summary>
        IAMFResponse Response { get; }

        /// <summary>
        /// Gets the ActionScript serializer for the request.
        /// </summary>
        IActionScriptSerializer Serializer { get; }
    }
}