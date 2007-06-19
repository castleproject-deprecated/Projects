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
    /// A cache partition separates a cache into parts by prefixing
    /// the partition name and a colon onto each key.  For example,
    /// if the requested key is "foo" and the partition name is "box",
    /// a request will be made to the underlying cache for the key "box:foo".
    /// </summary>
    public class CachePartition : BaseCache
    {
        private ICache innerCache;
        private string partitionName;
        private string partitionPrefix;

        /// <summary>
        /// Creates a new partitioned cache.
        /// </summary>
        /// <param name="innerCache">The cache to partition</param>
        /// <param name="partitionName">The name of the partition</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="innerCache"/>
        /// or <paramref name="partitionName"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="partitionName"/>
        /// is empty</exception>
        public CachePartition(ICache innerCache, string partitionName)
        {
            if (innerCache == null)
                throw new ArgumentNullException("innerCache");
            if (partitionName == null)
                throw new ArgumentNullException("partitionName");
            if (partitionName.Length == 0)
                throw new ArgumentException("Partition name must not be an empty string.", "partitionName");

            this.innerCache = innerCache;
            this.partitionName = partitionName;

            partitionPrefix = partitionName + ":";
        }

        /// <summary>
        /// Gets the cache which has been partitioned.
        /// </summary>
        public ICache InnerCache
        {
            get { return innerCache; }
        }

        /// <summary>
        /// Gets the partition name.
        /// </summary>
        public string PartitionName
        {
            get { return partitionName; }
        }

        /// <inheritdoc />
        protected override void InternalFlush()
        {
            innerCache.Flush();
        }

        /// <inheritdoc />
        protected override bool InternalContainsKey(string key)
        {
            return innerCache.ContainsKey(partitionPrefix + key);
        }

        /// <inheritdoc />
        protected override object InternalGet(string key)
        {
            return innerCache.Get(partitionPrefix + key);
        }

        /// <inheritdoc />
        protected override object[] InternalGetMultiple(string[] keys)
        {
            string[] prefixedKeys = Array.ConvertAll<string, string>(keys, delegate(string key)
            {
                return partitionPrefix + key;
            });

            return innerCache.GetMultiple(prefixedKeys);
        }

        /// <inheritdoc />
        protected override object InternalGetOrPopulate(string key, Populator populator)
        {
            return innerCache.GetOrPopulate(partitionPrefix + key, delegate(string prefixedKey, out CacheOptions options)
            {
                return populator(key, out options);
            });
        }

        /// <inheritdoc />
        protected override object InternalPopulate(string key, Populator populator)
        {
            return innerCache.Populate(partitionPrefix + key, delegate(string prefixedKey, out CacheOptions options)
            {
                return populator(key, out options);
            });
        }

        /// <inheritdoc />
        protected override void InternalSet(string key, object value, CacheOptions options)
        {
            innerCache.Set(partitionPrefix + key, value, options);
        }

        /// <inheritdoc />
        protected override void InternalRemove(string key)
        {
            innerCache.Remove(partitionPrefix + key);
        }
    }
}
