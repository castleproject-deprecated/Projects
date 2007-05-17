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

using Castle.FlexBridge.Serialization.AMF;

namespace Castle.FlexBridge.Gateway
{
    /// <summary>
    /// Contains information about an AMF request in progress.
    /// </summary>
    public interface IAMFRequest
    {
        /// <summary>
        /// Gets the AMF request message being processed.
        /// May be null if the message is not currently available.
        /// </summary>
        AMFMessage Message { get; }

        /// <summary>
        /// Returns true if the request was made over a secure channel.
        /// </summary>
        bool IsSecureConnection { get; }
    }
}