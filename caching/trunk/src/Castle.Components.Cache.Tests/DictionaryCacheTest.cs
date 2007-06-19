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
using System.Threading;
using MbUnit.Framework;

namespace Castle.Components.Cache.Tests
{
    [TestFixture]
    [TestsOn(typeof(DictionaryCache))]
    public class DictionaryCacheTest : TypicalCacheTest
    {
        new public DictionaryCache Cache
        {
            get { return (DictionaryCache)base.Cache; }
        }

        public override ICache CreateCache()
        {
            return new DictionaryCache();
        }

        [Test]
        public void CleanupIntervalDefaultIsZero()
        {
            Assert.AreEqual(0, Cache.CleanupIntervalInMilliseconds);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CleanupInterval_ThrowsWhenSetNegative()
        {
            Cache.CleanupIntervalInMilliseconds = -1;
        }

        [Test]
        public void CleanupPurgesExpiredEntries()
        {
            Cache.Set("key1", "value1", CacheOptions.AbsoluteExpiration(DateTime.UtcNow));
            Cache.Set("key2", "value2", CacheOptions.AbsoluteExpiration(DateTime.MinValue));
            Cache.Set("key3", "value3", CacheOptions.AbsoluteExpiration(DateTime.MaxValue));
            Cache.Set("key4", "value4", CacheOptions.SlidingExpiration(TimeSpan.Zero));
            Cache.Set("key5", "value5", CacheOptions.NoExpiration);
            Thread.Sleep(100); // Cause keys to expire

            Cache.Cleanup();

            // Note: Order of ContainsKey / Get matters because ContainsKey
            //       doesn't auto-expire values like Get does.
            Assert.IsFalse(Cache.ContainsKey("key1"));
            Assert.AreEqual(null, Cache.Get("key1"));

            Assert.IsFalse(Cache.ContainsKey("key2"));
            Assert.AreEqual(null, Cache.Get("key2"));

            Assert.IsTrue(Cache.ContainsKey("key3"));
            Assert.AreEqual("value3", Cache.Get("key3"));

            Assert.IsFalse(Cache.ContainsKey("key4"));
            Assert.AreEqual(null, Cache.Get("key4"));

            Assert.IsTrue(Cache.ContainsKey("key5"));
            Assert.AreEqual("value5", Cache.Get("key5"));
        }

        [Test]
        public void BackgroundCleanupIsInitiatedWhenIntervalIsSet()
        {
            CacheOptions options = CacheOptions.SlidingExpiration(new TimeSpan(0, 0, 0, 0, 50));

            // Initially values are not getting purged because the interval is 0.
            Cache.Set("key", "value", options);
            Thread.Sleep(300);
            Assert.IsTrue(Cache.ContainsKey("key"));

            // Cleanup manually to demonstrate that the value should go away.
            Cache.Cleanup();
            Assert.IsFalse(Cache.ContainsKey("key"));

            // Now enable auto-cleanup
            Cache.CleanupIntervalInMilliseconds = 100;
            Assert.AreEqual(100, Cache.CleanupIntervalInMilliseconds);

            // Add some values and watch them get purged periodically.
            Cache.Set("key", "value", options);
            Thread.Sleep(300);
            Assert.IsFalse(Cache.ContainsKey("key"));

            Cache.Set("key", "value", options);
            Thread.Sleep(300);
            Assert.IsFalse(Cache.ContainsKey("key"));

            // Now stop the cleanup and notice that they don't get purged anymore.
            Cache.CleanupIntervalInMilliseconds = 0;

            Cache.Set("key", "value", options);
            Thread.Sleep(300);
            Assert.IsTrue(Cache.ContainsKey("key"));
        }
    }
}