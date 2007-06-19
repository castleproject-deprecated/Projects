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
using Memcached.ClientLibrary;

namespace Castle.Components.Cache.Memcached
{
    /// <summary>
    /// An implementation of <see cref="ICache" /> based on Memcached.
    /// This cache requires connection parameters for the Memcached server
    /// to be provided to a <see cref="MemcachedCachePool" /> and started
    /// before it can be used.
    /// </summary>
    /// <remarks>
    /// The Memcached .Net client library is LGPL (c) 2005 Tim Gebhardt.
    /// Please refer to the Memcache .Net client library and to the Memcached
    /// server document for more information.
    /// </remarks>
    [Singleton]
    public class MemcachedCache : BaseCache
    {
        private MemcachedClient client;

        /// <summary>
        /// Creates a wrapper for a new <see cref="MemcachedClient" /> initially configured
        /// to refer to the default pool.
        /// </summary>
        public MemcachedCache()
        {
            client = new MemcachedClient();
        }

        /// <summary>
        /// Creates a wrapper for an existing <see cref="MemcachedClient" />.
        /// </summary>
        /// <param name="client">The existing client</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="client"/> is null</exception>
        public MemcachedCache(MemcachedClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            this.client = client;
        }

        /// <summary>
        /// Gets the underlying <see cref="MemcachedClient" /> wrapped by this instance.
        /// </summary>
        public MemcachedClient MemcachedClient
        {
            get { return client; }
        }

		/// <summary>
        /// Gets or sets the name of the cache pool that this cache will use.
        /// The named cache pool must be registered before the cache is used otherwise
        /// all operations will fail.
		/// </summary>
        /// <remarks>
        /// The default value is the localized default pool name.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        /// <seealso cref="MemcachedCachePool" />
        public string PoolName
        {
            get { return client.PoolName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                client.PoolName = value;
            }
        }

		/// <summary>
        /// Gets or sets whether primitive types will be stored as their
        /// string values.
		/// </summary>
        /// <remarks>
        /// The default value is false.
        /// </remarks>
		public bool PrimitiveAsString
		{
			get { return client.PrimitiveAsString; }
			set { client.PrimitiveAsString = value; }
		}

		/// <summary>
		/// Gets or sets the name of the default string encoding when storing primitives as strings. 
        /// If the encoding name does not correspond to any of the built-in .Net encodings
        /// an error will occur when the cache is used.
		/// </summary>
        /// <remarks>
        /// The default value is "UTF-8".
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string DefaultEncoding
		{
			get { return client.DefaultEncoding; }
			set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                client.DefaultEncoding = value;
            }
		}

		/// <summary>
        /// Gets or sets whether compression of stored data is enabled provided
        /// the size of the data to be stored is at least <see cref="CompressionThreshold" />
        /// bytes.
		/// </summary>
        /// <remarks>
        /// The default value is "true".
        /// </remarks>
		public bool EnableCompression
		{
			get { return client.EnableCompression; }
			set { client.EnableCompression = value; }
		}

		/// <summary>
        /// Gets or sets the minimum size of stored data in bytes to consider
        /// for compression assuming <see cref="EnableCompression" /> is true.
		/// </summary>
        /// <remarks>
        /// The default value is 31.5 Kib (30720).
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/>
        /// is less than 0</exception>
		public long CompressionThreshold
		{
			get { return client.CompressionThreshold; }
			set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Compression threshold must not be negative.");

                client.CompressionThreshold = value;
            }
		}

        /// <inheritdoc />
        protected override void InternalFlush()
        {
            client.FlushAll();
        }

        /// <inheritdoc />
        protected override bool InternalContainsKey(string key)
        {
            return client.KeyExists(key);
        }

        /// <inheritdoc />
        protected override object InternalGet(string key)
        {
            return client.Get(key);
        }

        /// <inheritdoc />
        protected override object[] InternalGetMultiple(string[] keys)
        {
            return client.GetMultipleArray(keys);
        }

        /// <inheritdoc />
        protected override void InternalSet(string key, object value, CacheOptions options)
        {
            DateTime expirationTime = options.GetUtcExpirationTimeRelativeToNow();

            client.Set(key, value, expirationTime);
        }

        /// <inheritdoc />
        protected override object InternalGetOrPopulate(string key, Populator populator)
        {
            object value = InternalGet(key);
            if (value != null)
                return value;

            return InternalPopulate(key, populator);
        }

        /// <inheritdoc />
        protected override object InternalPopulate(string key, Populator populator)
        {
            CacheOptions options;
            object value = populator(key, out options);

            if (value != null)
            {
                InternalSet(key, value, options);
            }
            else
            {
                InternalRemove(key);
            }

            return value;
        }

        /// <inheritdoc />
        protected override void InternalRemove(string key)
        {
            client.Delete(key);
        }
    }
}
