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
    /// <para>
    /// A cache maintains transient associations between string-based keys and
    /// cached objects.  Each association can be qualified with a predetermined
    /// expiration time to ensure that the contentsd are periodically refreshed.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Though not explicitly specified by the contract of the caching service,
    /// caches are generally intended to be shared across multiple threads
    /// or processes for the lifetime of the system.  Per-request caching is
    /// certainly supported by the interface though certain cache implementations
    /// may be unsuitable for the purpose.  Per-request caching will generally
    /// associate a distict cache instance for each request scope using some
    /// additional external mechanism not provided by this interface.
    /// </para>
    /// <para>
    /// Multiple cache implementations are supported as long as they adhere to
    /// a few basic rules of the contract.  In general such implementations
    /// should be thread-safe and designed to minimize the costs of contention
    /// during heavy concurrent load.  While not mandatory, the interface suggests
    /// that operations involving lazy population of cache values should be
    /// synchronized so as to avoid possibly expensive redundant population of the
    /// same keys by multiple threads.  Likewise an implementation may employ
    /// prioritization, pre-fetching, batching, and hierarchical storage techniques
    /// to improve performance under heavy load.
    /// </para>
    /// <para>
    /// The client of a cache should not assume any particular characteristics of
    /// its runtime behavior.  A cache is not intended to be a persistent storage medium!
    /// A cache may forget values sooner than might be suggested by the expiration
    /// time of individual items.  A cache might be affected by external factors that
    /// do not appear in the interface such as when a cache is allowed to span multiple
    /// processes or is managed externally.  
    /// </para>
    /// </remarks>
    /// <todo>
    /// Should caches be allowed to store nulls?  That's often useful
    /// behavior particularly when it is expensive to populate that null.
    /// It's quite straightforward for client code to store DBNulls or other
    /// marker values instead though it can be quite cumbersome.
    /// The problem is that permitting nulls makes the interface a little more clunky
    /// since we end up with methods like TryGet and TryPopulate.  It may also
    /// conceal programming errors.
    /// </todo>
    /// <todo>
    /// Include some kind of statistics API.  At the least to determine
    /// how much is in the cache or how heavily loaded it is.  This might
    /// only be feasible or useful for certain implementation so perhaps
    /// we can create an extended interface just for those that can be
    /// managed adaptively.
    /// </todo>
    public interface ICache
    {
        /// <summary>
        /// Clears the cache completely.
        /// </summary>
        void Flush();

        /// <summary>
        /// Returns true if the cache contains a value with the specified key.
        /// </summary>
        /// <remarks>
        /// The value returned by <see cref="ContainsKey" /> is only valid at the time
        /// the request was made.  It does not imply any guarantees over whether subsequent
        /// calles to <see cref="Get" /> will return a value.
        /// </remarks>
        /// <param name="key">The cache key</param>
        /// <returns>True if the cache contains a value with the specified key</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is empty</exception>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets the cached value with the specified key or null if not found.
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <returns>The cached value or null if not found</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is empty</exception>
        object Get(string key);

        /// <summary>
        /// Gets multiple cached values associated with the specified keys, or nulls
        /// when the associated values are not found.
        /// </summary>
        /// <param name="keys">The cache keys</param>
        /// <returns>The cached values in the same order as the keys, may contain nulls for
        /// each key that is not found</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="keys"/> or any of
        /// the keys it contains is null</exception>
        /// <exception cref="ArgumentException">Thrown if any of the keys in <paramref name="keys"/> is empty</exception>
        object[] GetMultiple(string[] keys);

        /// <summary>
        /// Gets the value associated with the specified key from the cache
        /// or populates it if it is not available at the time of the request.
        /// </summary>
        /// <remarks>
        /// The cache implementation should try to lock the cache key being
        /// populated so that concurrent requests to get or populate the value
        /// are blocked to avoid redundant cache population.  Locking might
        /// only work within the scope of a single process however.
        /// </remarks>
        /// <param name="key">The cache key</param>
        /// <param name="populator">The cache population delegate</param>
        /// <returns>The value or null if the populator did not produce a value</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or <paramref name="populator"/>
        /// is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is empty</exception>
        object GetOrPopulate(string key, Populator populator);

        /// <summary>
        /// Sets the value associated with the specified key by invoking
        /// the specified populator.  If the populator returns null, the value
        /// associated with the key will be removed from the cache.
        /// </summary>
        /// <remarks>
        /// The cache implementation should try to lock the cache key being
        /// populated so that concurrent requests to get or populate the value
        /// are blocked to avoid redundant cache population.  Locking might
        /// only work within the scope of a single process however.
        /// </remarks>
        /// <param name="key">The cache key</param>
        /// <param name="populator">The cache population delegate</param>
        /// <returns>The value or null if the populator did not produce a value</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or <paramref name="populator"/>
        /// is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is empty</exception>
        object Populate(string key, Populator populator);

        /// <summary>
        /// Sets the value of the specified key with infinite duration.
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <param name="value">The value to set, must not be null</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or <paramref name="value"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is empty</exception>
        void Set(string key, object value);

        /// <summary>
        /// Sets the value of the specified key with the specified options.
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <param name="value">The value to set, must not be null</param>
        /// <param name="options">The cache options for the value</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or <paramref name="value"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is empty</exception>
        void Set(string key, object value, CacheOptions options);

        /// <summary>
        /// Removes the value associated with the specified key, if any.
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is empty</exception>
        void Remove(string key);

        /*
        /// <summary>
        /// Tries to obtain a lock on the cached value of the specified key.
        /// If a shared lock is obtained, other threads can read the value of the key
        /// but they will block when attempting to modify it until the lock is released.
        /// If an exclusive lock is obtained, other threads will block when attempting
        /// to read or modify the value until the lock is released.
        /// A lock can be obtained on a key that has no value which ensures that other
        /// threads cannot concurrently insert a value for it.
        /// TODO: What happens during a flush?
        /// TODO: Specify that concurrent processes may be exempted from the locking
        ///       rules depending on the implementation and that locking is an advisory hint
        ///       for optimization purposes only.
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <param name="lockMode">The cache locking mode to obtain exclusive or shared
        /// access to the key</param>
        /// <returns>The lock object, or null if the lock was not granted</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is empty</exception>
        ICacheLock Lock(string key, CacheLockMode lockMode);
         */
    }
}
