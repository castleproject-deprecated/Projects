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
using Castle.Core;

namespace Castle.Components.Cache
{
    /// <summary>
    /// An implementation of <see cref="ICache" /> that checks its parameters
    /// but does not actually cache anything.  Values that are set in the
    /// cache are immediately forgotten.
    /// </summary>
    /// <remarks>
    /// This implementation is useful when you want to disable caching completely
    /// without requiring any modifications to your code.
    /// </remarks>
    [Singleton]
    public class NullCache : BaseCache
    {
        /// <inheritdoc />
        protected override void InternalFlush()
        {
        }

        /// <inheritdoc />
        protected override bool InternalContainsKey(string key)
        {
            return false;
        }

        /// <inheritdoc />
        protected override object[] InternalGetMultiple(string[] keys)
        {
            return new object[keys.Length];
        }

        /// <inheritdoc />
        protected override object InternalGet(string key)
        {
            return null;
        }

        /// <inheritdoc />
        protected override void InternalSet(string key, object value, CacheOptions options)
        {
        }

        /// <inheritdoc />
        protected override void InternalRemove(string key)
        {
        }

        /// <inheritdoc />
        protected override object InternalGetOrPopulate(string key, Populator populator)
        {
            CacheOptions options;
            return populator(key, out options);
        }

        /// <inheritdoc />
        protected override object InternalPopulate(string key, Populator populator)
        {
            CacheOptions options;
            return populator(key, out options);
        }
    }
}
