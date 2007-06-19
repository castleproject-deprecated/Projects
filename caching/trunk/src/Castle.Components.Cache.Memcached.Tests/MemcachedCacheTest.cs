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
using System.Text;
using System.Threading;
using Castle.Components.Cache.Memcached;
using Castle.Components.Cache.Tests;
using MbUnit.Framework;

namespace Castle.Components.Cache.Memcached.Tests
{
    [TestFixture]
    [TestsOn(typeof(MemcachedCache))]
    public class MemcachedCacheTest : TypicalCacheTest
    {
        private MemcachedCachePool pool;

        new public MemcachedCache Cache
        {
            get { return (MemcachedCache)base.Cache; }
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.BlocksGetWhilePopulatorForThatKeyIsRunning = false;

            pool = new MemcachedCachePool();
            pool.Servers = new string[] { "localhost" };
            pool.Start();
            Thread.Sleep(200); // FIXME: There seems to be a delay before the 
            // first sockets in the pool become usable.  If I remove this sleep then
            // the tests will fail because they cannot obtain an open connection to
            // the server from the available pool.
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (pool != null)
            {
                pool.Dispose();
                pool = null;
            }
        }

        public override ICache CreateCache()
        {
            MemcachedCache cache = new MemcachedCache();
            cache.Flush();
            return cache;
        }

        [Test]
        public void PoolName_GetterAndSetter()
        {
            Assert.AreEqual(pool.PoolName, Cache.PoolName);

            Cache.PoolName = "pool";
            Assert.AreEqual("pool", Cache.PoolName);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PoolName_ThrowsIfValueIsNull()
        {
            Cache.PoolName = null;
        }

        [Test]
        public void MemcachedClient()
        {
            Assert.IsNotNull(Cache.MemcachedClient);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithClient_ThrowsIfClientIsNull()
        {
            new MemcachedCache(null);
        }

        [Test]
        public void Constructor_WithClient_SetsProperties()
        {
            MemcachedCache cache = new MemcachedCache(Cache.MemcachedClient);
            Assert.AreSame(Cache.MemcachedClient, cache.MemcachedClient);
        }

        [Test]
        public void CompressionThreshold_GetterAndSetter()
        {
            Assert.AreEqual(30720, Cache.CompressionThreshold);

            Cache.CompressionThreshold = 1000;
            Assert.AreEqual(1000, Cache.CompressionThreshold);
            Assert.AreEqual(1000, Cache.MemcachedClient.CompressionThreshold);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CompressionThreshold_ThrowsWhenValueIsNegative()
        {
            Cache.CompressionThreshold = -1;
        }

        [Test]
        public void DefaultEncoding_GetterAndSetter()
        {
            Assert.AreEqual("UTF-8", Cache.DefaultEncoding);

            Cache.DefaultEncoding = "US-ASCII";
            Assert.AreEqual("US-ASCII", Cache.DefaultEncoding);
            Assert.AreEqual("US-ASCII", Cache.MemcachedClient.DefaultEncoding);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DefaultEncoding_ThrowsWhenValueIsNull()
        {
            Cache.DefaultEncoding = null;
        }

        [Test]
        public void EnableCompression_GetterAndSetter()
        {
            Assert.AreEqual(true, Cache.EnableCompression);

            Cache.EnableCompression = false;
            Assert.AreEqual(false, Cache.EnableCompression);
            Assert.AreEqual(false, Cache.MemcachedClient.EnableCompression);
        }

        [Test]
        public void PrimitiveAsString_GetterAndSetter()
        {
            Assert.AreEqual(false, Cache.PrimitiveAsString);

            Cache.PrimitiveAsString = true;
            Assert.AreEqual(true, Cache.PrimitiveAsString);
            Assert.AreEqual(true, Cache.MemcachedClient.PrimitiveAsString);
        }
    }
}
