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
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Castle.Core;
using ASPNetCache = System.Web.Caching.Cache;

namespace Castle.Components.Cache.SysCache
{
    /// <summary>
    /// An implementation of <see cref="ICache" /> based on the ASP.Net system
    /// <see cref="InnerCache" />
    /// </summary>
    [Singleton]
    public class SysCache : BaseCache
    {
        private ASPNetCache innerCache;

        /// <summary>
        /// Creates a wrapper for the ASP.Net system cache.
        /// </summary>
        public SysCache()
        {
            innerCache = HttpRuntime.Cache;
        }

        /// <summary>
        /// Gets the ASP.Net cache used by this wrapper.
        /// </summary>
        public ASPNetCache InnerCache
        {
            get { return innerCache; }
        }

        /// <inheritdoc />
        protected override void InternalFlush()
        {
            foreach (DictionaryEntry entry in innerCache)
                innerCache.Remove((string) entry.Key);
        }

        /// <inheritdoc />
        protected override bool InternalContainsKey(string key)
        {
            return innerCache.Get(key) != null;
        }

        /// <inheritdoc />
        protected override object InternalGet(string key)
        {
            return GetOrWaitUntilPopulated(key);
        }

        /// <inheritdoc />
        protected override object[] InternalGetMultiple(string[] keys)
        {
            object[] values = Array.ConvertAll<string, object>(keys, GetOrWaitUntilPopulated);
            return values;
        }

        /// <inheritdoc />
        protected override void InternalSet(string key, object value, CacheOptions options)
        {
            DateTime absoluteExpirationOption = options.AbsoluteExpirationTime.HasValue ?
                options.AbsoluteExpirationTime.Value : ASPNetCache.NoAbsoluteExpiration;
            TimeSpan slidingExpirationOption = options.SlidingExpirationTimeSpan.HasValue ?
                options.SlidingExpirationTimeSpan.Value : ASPNetCache.NoSlidingExpiration;

            innerCache.Insert(key, value, null, absoluteExpirationOption, slidingExpirationOption);
        }

        /// <inheritdoc />
        protected override object InternalGetOrPopulate(string key, Populator populator)
        {
            // Check if value has already been populated.
            object value = GetOrWaitUntilPopulated(key);
            if (value != null)
                return value;

            Sentinel sentinel = new Sentinel();
            try
            {
                // Add a sentinel into the cache unless some other thread has managed
                // to squeeze in here first.
                value = innerCache.Add(key, sentinel, null, ASPNetCache.NoAbsoluteExpiration, ASPNetCache.NoSlidingExpiration,
                    CacheItemPriority.NotRemovable, null);

                if (value == null)
                {
                    // The sentinel was added.
                    return PopulateWithSentinel(key, populator, sentinel);
                }
                else
                {
                    // The sentinel was not added.
                    // That means some other thread has set the value or begun population
                    // in the meantime so we have to handle each case.
                    sentinel = value as Sentinel;
                    if (sentinel != null)
                        value = sentinel.WaitUntilFinished();

                    return value;
                }
            }
            catch (Exception ex)
            {
                sentinel.FinishedWithException(ex);
                RemoveSentinel(key, sentinel);

                throw;
            }
        }

        /// <inheritdoc />
        protected override object InternalPopulate(string key, Populator populator)
        {
            Sentinel sentinel = new Sentinel();
            try
            {
                innerCache.Insert(key, sentinel, null, ASPNetCache.NoAbsoluteExpiration, ASPNetCache.NoSlidingExpiration,
                    CacheItemPriority.NotRemovable, null);

                return PopulateWithSentinel(key, populator, sentinel);
            }
            catch (Exception ex)
            {
                sentinel.FinishedWithException(ex);
                RemoveSentinel(key, sentinel);

                throw;
            }
        }

        /// <inheritdoc />
        protected override void InternalRemove(string key)
        {
            innerCache.Remove(key);
        }

        private object GetOrWaitUntilPopulated(string key)
        {
            object value = innerCache.Get(key);

            Sentinel sentinel = value as Sentinel;
            if (sentinel != null)
                return sentinel.WaitUntilFinished();

            return value;
        }

        private object PopulateWithSentinel(string key, Populator populator, Sentinel sentinel)
        {
            CacheOptions options;
            object value = populator(key, out options);

            if (value != null)
            {
                sentinel.FinishedWithValue(value);
                ReplaceSentinel(key, value, options, sentinel);
            }
            else
            {
                RemoveSentinel(key, sentinel);
            }

            return value;
        }

        private void RemoveSentinel(string key, Sentinel sentinel)
        {
            // Note: This is a minor optimization to ensure we don't blow away
            //       a newer value associated with the key when we're just trying
            //       to remove the sentinel we stored in there.
            if (innerCache.Get(key) == sentinel)
                innerCache.Remove(key);
        }

        private void ReplaceSentinel(string key, object value, CacheOptions options, Sentinel sentinel)
        {
            // Note: This is a minor optimization to ensure we don't blow away
            //       a newer value associated with the key when we're just trying
            //       to replace the sentinel we stored in there.
            if (innerCache.Get(key) == sentinel)
                InternalSet(key, value, options);
        }

        private sealed class Sentinel
        {
            private enum State
            {
                Pending = 0, Value, Exception
            }

            private object value;
            private State state;

            public object WaitUntilFinished()
            {
                lock (this)
                {
                    while (state == State.Pending)
                        Monitor.Wait(this);

                    if (state == State.Value)
                        return value;
                    else
                        throw (Exception)value;
                }
            }

            public void FinishedWithValue(object value)
            {
                Finished(value, State.Value);
            }

            public void FinishedWithException(Exception ex)
            {
                Finished(ex, State.Exception);
            }

            private void Finished(object value, State state)
            {
                lock (this)
                {
                    this.value = value;
                    this.state = state;
                    Monitor.PulseAll(this);
                }
            }
        }
    }
}
