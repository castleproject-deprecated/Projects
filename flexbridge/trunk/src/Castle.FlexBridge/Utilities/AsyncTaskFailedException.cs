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
using System.Runtime.Serialization;
using Castle.FlexBridge.Gateway.RequestProcessors;

namespace Castle.FlexBridge.Utilities
{
    /// <summary>
    /// Exception type thrown when a <see cref="AsyncTask" /> fails.
    /// </summary>
    [Serializable]
    public class AsyncTaskFailedException : Exception
    {
        public AsyncTaskFailedException(string message)
            : base(message)
        {
        }

        public AsyncTaskFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected AsyncTaskFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}