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
using System.Threading;
using Castle.Core;

namespace Castle.Components.Cache
{
    /// <summary>
    /// A very simple cache based on a dictionary.  Supports sliding and
    /// absolute expiration of cache items by testing for expiration of
    /// cache items before each access and via a background timer that
    /// runs periodically to remove expired entries.
    /// </summary>
    /// <remarks>
    /// This lightweight implementation is suitable when you are storing at most
    /// a few thousand items in local memory, aren't worried about lock contention
    /// and don't care about releasing cache items in response to elevated memory pressure.  
    /// If your needs are more demanding, then you should use a more sophisticated cache
    /// implementation instead.
    /// </remarks>
    /// <threadsafety>All operations of the cache are thread-safe.</threadsafety>
    [Singleton]
    public class DictionaryCache : BaseCache
    {
        private Dictionary<string, CacheItem> items;
        private int cleanupIntervalInMilliseconds;
        private DateTime nextExpirationTime;

        private int cleanupTimerNonce;
        private Timer cleanupTimer;

        /// <summary>
        /// Creates an empty dictionary cache.
        /// </summary>
        public DictionaryCache()
        {
            items = new Dictionary<string, CacheItem>();
            nextExpirationTime = DateTime.MaxValue;
        }

        /// <summary>
        /// Gets or sets the number of milliseconds to wait between automatic
        /// background invocations of <see cref="Cleanup" /> to clean up the cache.
        /// If the value is 0, the cache will not be cleaned up automatically.
        /// </summary>
        /// <remarks>
        /// The default value is 0 so cleanups do not occur automatically.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is negative</exception>
        public int CleanupIntervalInMilliseconds
        {
            get
            {
                lock (this)
                    return cleanupIntervalInMilliseconds;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "The number of milliseconds between cleanups must be non-negative.");

                lock (this)
                {
                    if (value != cleanupIntervalInMilliseconds)
                    {
                        cleanupIntervalInMilliseconds = value;

                        if (cleanupTimer != null)
                        {
                            cleanupTimer.Dispose();
                            cleanupTimer = null;
                        }

                        if (cleanupIntervalInMilliseconds != 0)
                        {
                            cleanupTimerNonce += 1;
                            cleanupTimer = new Timer(BackgroundCleanup, cleanupTimerNonce, cleanupIntervalInMilliseconds, cleanupIntervalInMilliseconds);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Scans the cache and removes all expired items.
        /// </summary>
        /// <remarks>
        /// This operation is somewhat costly so use it sparingly.
        /// If <see cref="CleanupIntervalInMilliseconds" /> is non-zero, this
        /// method is called automatically in the background.
        /// </remarks>
        public void Cleanup()
        {
            lock (items)
            {
                // Ensure we have some work to do.
                DateTime now = DateTime.UtcNow;
                if (nextExpirationTime > now)
                    return;

                // Make a list of all expired keys and update the next expiration time.
                List<string> expiredKeys = new List<string>();

                nextExpirationTime = DateTime.MaxValue;
                foreach (KeyValuePair<string, CacheItem> itemPair in items)
                {
                    DateTime itemExpirationTime = itemPair.Value.ExpirationTime;
                    if (itemExpirationTime <= now)
                        expiredKeys.Add(itemPair.Key);
                    else if (itemExpirationTime < nextExpirationTime)
                        nextExpirationTime = itemExpirationTime;
                }

                // Remove the expired values.
                if (expiredKeys.Count == items.Count)
                {
                    items.Clear();
                }
                else
                {
                    foreach (string expiredKey in expiredKeys)
                        items.Remove(expiredKey);
                }
            }
        }

        private void BackgroundCleanup(object nonceObject)
        {
            int nonce = (int)nonceObject;

            lock (this)
            {
                // Abort if the timer has been updated concurrently.
                if (nonce != cleanupTimerNonce)
                    return;

                Cleanup();
            }
        }

        /// <inheritdoc />
        protected override void InternalFlush()
        {
            lock (items)
            {
                nextExpirationTime = DateTime.MaxValue;
                items.Clear();
            }
        }

        /// <inheritdoc />
        protected override bool InternalContainsKey(string key)
        {
            lock (items)
            {
                return items.ContainsKey(key);
            }
        }

        /// <inheritdoc />
        protected override object InternalGet(string key)
        {
            lock (items)
            {
                return GetOrWaitUntilPopulatedInsideLock(key);
            }
        }

        /// <inheritdoc />
        protected override object[] InternalGetMultiple(string[] keys)
        {
            lock (items)
            {
                object[] values = Array.ConvertAll<string, object>(keys, GetOrWaitUntilPopulatedInsideLock);
                return values;
            }
        }

        /// <inheritdoc />
        protected override void InternalSet(string key, object value, CacheOptions options)
        {
            DateTime expirationTime = options.GetUtcExpirationTimeRelativeToNow();

            lock (items)
            {
                items[key] = new CacheItem(value, expirationTime);

                if (expirationTime < nextExpirationTime)
                    nextExpirationTime = expirationTime;
            }
        }

        /// <inheritdoc />
        protected override void InternalRemove(string key)
        {
            lock (items)
            {
                items.Remove(key);

                if (items.Count == 0)
                    nextExpirationTime = DateTime.MaxValue;
            }
        }

        /// <inheritdoc />
        protected override object InternalGetOrPopulate(string key, Populator populator)
        {
            Sentinel sentinel = null;
            try
            {
                lock (items)
                {
                    object existingValue = GetOrWaitUntilPopulatedInsideLock(key);
                    if (existingValue != null)
                        return existingValue;

                    sentinel = new Sentinel();
                    items[key] = new CacheItem(sentinel, DateTime.MaxValue);
                }

                return PopulateWithSentinel(key, populator, sentinel);
            }
            catch (Exception)
            {
                if (sentinel != null)
                    RemoveSentinel(key, sentinel);
                throw;
            }
        }

        /// <inheritdoc />
        protected override object InternalPopulate(string key, Populator populator)
        {
            Sentinel sentinel = null;
            try
            {
                lock (items)
                {
                    sentinel = new Sentinel();
                    items[key] = new CacheItem(sentinel, DateTime.MaxValue);
                }

                return PopulateWithSentinel(key, populator, sentinel);
            }
            catch (Exception)
            {
                if (sentinel != null)
                    RemoveSentinel(key, sentinel);
                throw;
            }
        }

        private object GetOrWaitUntilPopulatedInsideLock(string key)
        {
            for (; ; )
            {
                CacheItem item;
                if (!items.TryGetValue(key, out item))
                    return null;

                if (item.Value is Sentinel)
                {
                    Monitor.Wait(items);
                }
                else
                {
                    if (item.ExpirationTime > DateTime.UtcNow)
                        return item.Value;

                    items.Remove(key);
                    return null;
                }
            }
        }

        private object PopulateWithSentinel(string key, Populator populator, Sentinel sentinel)
        {
            CacheOptions options;
            object populatedValue = populator(key, out options);

            lock (items)
            {
                CacheItem item;
                if (items.TryGetValue(key, out item) && item.Value == sentinel)
                {
                    if (populatedValue != null)
                    {
                        DateTime expirationTime = options.GetUtcExpirationTimeRelativeToNow();

                        items[key] = new CacheItem(populatedValue, expirationTime);
                        if (expirationTime < nextExpirationTime)
                            nextExpirationTime = expirationTime;
                    }
                    else
                    {
                        items.Remove(key);
                    }
                }

                Monitor.PulseAll(items);
            }

            return populatedValue;
        }

        private void RemoveSentinel(string key, Sentinel sentinel)
        {
            lock (items)
            {
                CacheItem item;
                if (items.TryGetValue(key, out item) && item.Value == sentinel)
                    items.Remove(key);

                Monitor.PulseAll(items);
            }
        }

        private struct CacheItem
        {
            public readonly object Value;
            public readonly DateTime ExpirationTime;

            public CacheItem(object value, DateTime expirationTime)
            {
                this.Value = value;
                this.ExpirationTime = expirationTime;
            }
        }

        private sealed class Sentinel
        {
        }
    }
}
