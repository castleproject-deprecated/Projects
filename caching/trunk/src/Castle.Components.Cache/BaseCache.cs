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

namespace Castle.Components.Cache
{
    /// <summary>
    /// Abstract base implementation of <see cref="ICache" />.
    /// Checks all parameters and provides a minimal implementation of the cache.
    /// </summary>
    public abstract class BaseCache : ICache
    {
        /// <inheritdoc />
        public void Flush()
        {
            InternalFlush();
        }

        /// <inheritdoc />
        public bool ContainsKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (key.Length == 0)
                throw new ArgumentException("Key must not be empty.", "key");

            return InternalContainsKey(key);
        }

        /// <inheritdoc />
        public object Get(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (key.Length == 0)
                throw new ArgumentException("Key must not be empty.", "key");

            return InternalGet(key);
        }

        /// <inheritdoc />
        public object[] GetMultiple(string[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException("keys");

            foreach (string key in keys)
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                if (key.Length == 0)
                    throw new ArgumentException("Key must not be empty.", "keys");
            }

            return InternalGetMultiple(keys);
        }

        /// <inheritdoc />
        public object GetOrPopulate(string key, Populator populator)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (key.Length == 0)
                throw new ArgumentException("Key must not be empty.", "keys");
            if (populator == null)
                throw new ArgumentNullException("populator");

            return InternalGetOrPopulate(key, populator);
        }

        /// <inheritdoc />
        public object Populate(string key, Populator populator)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (key.Length == 0)
                throw new ArgumentException("Key must not be empty.", "keys");
            if (populator == null)
                throw new ArgumentNullException("populator");

            return InternalPopulate(key, populator);
        }

        /// <inheritdoc />
        public void Set(string key, object value)
        {
            Set(key, value, CacheOptions.NoExpiration);
        }

        /// <inheritdoc />
        public void Set(string key, object value, CacheOptions options)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (key.Length == 0)
                throw new ArgumentException("Key must not be empty.", "key");
            if (value == null)
                throw new ArgumentNullException("value");

            InternalSet(key, value, options);
        }



        /// <inheritdoc />
        public void Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (key.Length == 0)
                throw new ArgumentException("Key must not be empty.", "key");

            InternalRemove(key);
        }

        /// <summary>
        /// Internal implementation of <see cref="Flush" />.
        /// All parameters have been validated by the base class.
        /// </summary>
        protected abstract void InternalFlush();

        /// <summary>
        /// Internal implementation of <see cref="ContainsKey" />.
        /// All parameters have been validated by the base class.
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <returns>True if the cache contains a value with the specified key</returns>
        protected abstract bool InternalContainsKey(string key);

        /// <summary>
        /// Internal implementation of <see cref="Get" />.
        /// All parameters have been validated by the base class.
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <returns>The cached value or null if not found</returns>
        protected abstract object InternalGet(string key);

        /// <summary>
        /// Internal implementation of <see cref="GetMultiple" />.
        /// All parameters have been validated by the base class.
        /// </summary>
        /// <param name="keys">The cache keys</param>
        /// <returns>The cached values in the same order as the keys, may contain nulls for
        /// each key that is not found</returns>
        protected abstract object[] InternalGetMultiple(string[] keys);

        /// <summary>
        /// Internal implementation of <see cref="Set(string, object, CacheOptions)" />.
        /// All parameters have been validated by the base class.
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <param name="value">The value to set, must not be null</param>
        /// <param name="options">The cache options for the value</param>
        protected abstract void InternalSet(string key, object value, CacheOptions options);

        /// <summary>
        /// Internal implementation of <see cref="GetOrPopulate(string, Populator)" />.
        /// All parameters have been validated by the base class.
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <param name="populator">The populator</param>
        /// <returns>The value or null if the populator did not produce a value</returns>
        protected abstract object InternalGetOrPopulate(string key, Populator populator);

        /// <summary>
        /// Internal implementation of <see cref="Populate(string, Populator)" />.
        /// All parameters have been validated by the base class.
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <param name="populator">The populator</param>
        /// <returns>The value or null if the populator did not produce a value</returns>
        protected abstract object InternalPopulate(string key, Populator populator);

        /// <summary>
        /// Internal implementation of <see cref="Remove" />.
        /// All parameters have been validated by the base class.
        /// </summary>
        /// <param name="key">The cache key</param>
        protected abstract void InternalRemove(string key);
    }
}
