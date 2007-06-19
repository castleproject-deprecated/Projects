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
using Castle.Components.Cache.Collections;

namespace Castle.Components.Cache
{
    /// <summary>
    /// Cache options specify control parameters determining the lifecycle
    /// of cached values.  The set of options that appear on this structure
    /// may not be supported by all caches in which case they are ignored.
    /// Thus cache options should usually be considered discretionary hints
    /// except when used with a particular cache implementation.
    /// </summary>
    public struct CacheOptions
    {
        private TimeSpan? slidingExpirationTimeSpan;
        private DateTime? absoluteExpirationTime;
        private IDictionary<string, object> customOptions;

        /// <summary>
        /// Gets or sets the sliding expiration time, or null if none.
        /// A sliding expiration time is specified relative to the time
        /// that a cached value is added to the cache.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A cached value might expire sooner than the specified time.
        /// </para>
        /// <para>
        /// This option should not be used together with <see cref="AbsoluteExpirationTime" />
        /// as the resulting specification would be ambiguous.
        /// </para>
        /// </remarks>
        public TimeSpan? SlidingExpirationTimeSpan
        {
            get { return slidingExpirationTimeSpan; }
            set { slidingExpirationTimeSpan = value; }
        }

        /// <summary>
        /// Gets or sets the absolute expiration time, or null if none.
        /// An absolute expiration time is specified a-priori.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A cached value might expire sooner than the specified time.
        /// </para>
        /// <para>
        /// A cache implementation should convert the absolute expiration
        /// time to Utc to ensure expected results are obtained.
        /// </para>
        /// <para>
        /// This option should not be used together with <see cref="SlidingExpirationTimeSpan" />
        /// as the resulting specification would be ambiguous.
        /// </para>
        /// </remarks>
        public DateTime? AbsoluteExpirationTime
        {
            get { return absoluteExpirationTime; }
            set { absoluteExpirationTime = value; }
        }

        /// <summary>
        /// Gets a dictionary of custom options for cache implementations with
        /// additional features that are not otherwise supported by this structure.
        /// </summary>
        public IDictionary<string, object> CustomOptions
        {
            get
            {
                if (customOptions == null)
                    return EmptyDictionary<string, object>.Instance;
                return customOptions;
            }
        }

        /// <summary>
        /// Adds a custom option.
        /// </summary>
        /// <param name="key">The option key</param>
        /// <param name="value">The option value</param>
        public void AddCustomOption(string key, object value)
        {
            if (customOptions == null)
                customOptions = new Dictionary<string, object>();
            customOptions.Add(key, value);
        }

        /// <summary>
        /// Returns cache options for no fixed expiration time.
        /// </summary>
        /// <remarks>
        /// A cached value might still expire for other reasons.
        /// </remarks>
        /// <returns>The cache options</returns>
        public static CacheOptions NoExpiration
        {
            get { return new CacheOptions(); }
        }

        /// <summary>
        /// Returns cache options for sliding expiration.
        /// </summary>
        /// <remarks>
        /// A cached value might expire sooner than the specified time.
        /// </remarks>
        /// <param name="slidingExpirationTimeSpan">The time when a cached value will
        /// expire relative to the time it was added to the cache</param>
        /// <returns>The cache options</returns>
        public static CacheOptions SlidingExpiration(TimeSpan slidingExpirationTimeSpan)
        {
            CacheOptions options = new CacheOptions();
            options.slidingExpirationTimeSpan = slidingExpirationTimeSpan;
            return options;
        }

        /// <summary>
        /// Returns cache options for absolute expiration.
        /// </summary>
        /// <remarks>
        /// A cached value might expire sooner than the specified time.
        /// </remarks>
        /// <param name="absoluteExpirationTime">The absolute time when the cached value
        /// will expire</param>
        /// <returns></returns>
        public static CacheOptions AbsoluteExpiration(DateTime absoluteExpirationTime)
        {
            CacheOptions options = new CacheOptions();
            options.absoluteExpirationTime = absoluteExpirationTime;
            return options;
        }

        /// <summary>
        /// Computes the cache expiration time relative to now.
        /// If <see cref="AbsoluteExpiration" /> is non-null, returns its value converted to UTC.
        /// If <see cref="SlidingExpiration" /> is non-null, returns its value summed with
        /// the current time in UTC.
        /// Otherwise returns <see cref="DateTime.MaxValue" />.
        /// </summary>
        /// <returns></returns>
        public DateTime GetUtcExpirationTimeRelativeToNow()
        {
            if (absoluteExpirationTime.HasValue)
                return absoluteExpirationTime.Value.ToUniversalTime();
            else if (slidingExpirationTimeSpan.HasValue)
                return DateTime.UtcNow.Add(slidingExpirationTimeSpan.Value);
            else
                return DateTime.MaxValue;
        }

        /*
        public static CacheOptions Priority(int priority)
        {
        }

        public static CacheOptions LoadFactor(int loadFactor)
        {
        }

        public static CacheOptions Dependency()
        {
        }

        public static CacheOptions OnExpiration()
        {
        }
         */
    }
}
