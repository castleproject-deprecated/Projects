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
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Castle.Components.Cache.Tests
{
    [TestFixture]
    [TestsOn(typeof(NullCache))]
    public class CachePartitionTest : BaseCacheTest
    {
        private ICache mockInnerCache;

        public override void SetUp()
        {
            mockInnerCache = Mocks.CreateMock<ICache>();

            base.SetUp();
        }

        new public CachePartition Cache
        {
            get { return (CachePartition)base.Cache; }
        }

        public override ICache CreateCache()
        {
            return new CachePartition(mockInnerCache, "partition");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowsWhenCacheIsNull()
        {
            new CachePartition(null, "partition");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowsWhenPartitionNameIsNull()
        {
            new CachePartition(mockInnerCache, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_ThrowsWhenPartitionNameIsEmpty()
        {
            new CachePartition(mockInnerCache, "");
        }

        [Test]
        public void Constructor_SetsPropertiesCorrectly()
        {
            Assert.AreSame(mockInnerCache, Cache.InnerCache);
            Assert.AreEqual("partition", Cache.PartitionName);
        }

        [Test]
        public void Flush_ForwardsToInnerCache()
        {
            mockInnerCache.Flush();
            Mocks.ReplayAll();

            Cache.Flush();
        }

        [Test]
        public void ContainsKey_ForwardsToInnerCache()
        {
            Expect.Call(mockInnerCache.ContainsKey("partition:key")).Return(true);
            Mocks.ReplayAll();

            Assert.IsTrue(Cache.ContainsKey("key"));
        }

        [Test]
        public void Get_ForwardsToInnerCache()
        {
            Expect.Call(mockInnerCache.Get("partition:key")).Return("value");
            Mocks.ReplayAll();

            Assert.AreEqual("value", Cache.Get("key"));
        }

        [Test]
        public void GetMultiple_ForwardsToInnerCache()
        {
            Expect.Call(mockInnerCache.GetMultiple(new string[] {
                "partition:a", "partition:b", "partition:c" }))
                .Return(new object[] { "x", null, "y" });
            Mocks.ReplayAll();
            
            CollectionAssert.AreEqual(new object[] { "x", null, "y" },
                Cache.GetMultiple(new string[] { "a", "b", "c" }));
        }

        [Test]
        public void Set_ForwardsToInnerCache()
        {
            mockInnerCache.Set("partition:key", "value", new CacheOptions());
            Mocks.ReplayAll();

            Cache.Set("key", "value");
        }

        [Test]
        public void Set_WithOptions_ForwardsToInnerCache()
        {
            CacheOptions options = CacheOptions.AbsoluteExpiration(DateTime.UtcNow);
            mockInnerCache.Set("partition:key", "value", options);
            Mocks.ReplayAll();

            Cache.Set("key", "value", options);
        }

        [Test]
        public void Remove_ForwardsToInnerCache()
        {
            mockInnerCache.Remove("partition:key");
            Mocks.ReplayAll();

            Cache.Remove("key");
        }

        [Test]
        public void GetOrPopulate_ForwardsToInnerCache()
        {
            CacheOptions options = CacheOptions.AbsoluteExpiration(DateTime.UtcNow);

            Populator populator = delegate(string key, out CacheOptions populatedOptions)
            {
                populatedOptions = options;
                Assert.AreEqual("key", key);
                return "value";
            };

            Expect.Call(mockInnerCache.GetOrPopulate("partition:key", null))
                .Constraints(Is.Equal("partition:key"), Is.NotSame(populator))
                .Do((PopulateDelegate)delegate(string actualKey, Populator actualPopulator)
                {
                    CacheOptions populatedOptions;
                    object populatedValue = actualPopulator(actualKey, out populatedOptions);
                    Assert.AreEqual("value", populatedValue);
                    Assert.AreEqual(options, populatedOptions);
                    return "value";
                });
            Mocks.ReplayAll();

            Assert.AreEqual("value", Cache.GetOrPopulate("key", populator));
        }

        [Test]
        public void Populate_ForwardsToInnerCache()
        {
            CacheOptions options = CacheOptions.AbsoluteExpiration(DateTime.UtcNow);

            Populator populator = delegate(string key, out CacheOptions populatedOptions)
            {
                populatedOptions = options;
                Assert.AreEqual("key", key);
                return "value";
            };

            Expect.Call(mockInnerCache.Populate("partition:key", null))
                .Constraints(Is.Equal("partition:key"), Is.NotSame(populator))
                .Do((PopulateDelegate)delegate(string actualKey, Populator actualPopulator)
                {
                    CacheOptions populatedOptions;
                    object populatedValue = actualPopulator(actualKey, out populatedOptions);
                    Assert.AreEqual("value", populatedValue);
                    Assert.AreEqual(options, populatedOptions);
                    return "value";
                });
            Mocks.ReplayAll();

            Assert.AreEqual("value", Cache.Populate("key", populator));
        }
    }
}
