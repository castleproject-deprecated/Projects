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

namespace Castle.Components.Cache.Tests
{
    [TestFixture]
    [TestsOn(typeof(NullCache))]
    public class NullCacheTest : BaseCacheTest
    {
        public override ICache CreateCache()
        {
            return new NullCache();
        }

        [Test]
        public void Flush_DoesNotHaveAnySideEffects()
        {
            Cache.Flush();
        }

        [Test]
        public void ContainsKey_ReturnsFalse()
        {
            Assert.IsFalse(Cache.ContainsKey("key"));
        }

        [Test]
        public void Get_ReturnsNull()
        {
            Assert.IsNull(Cache.Get("key"));
        }

        [Test]
        public void GetMultiple_ReturnsNullForEachKey()
        {
            CollectionAssert.AreEqual(new object[] { null, null, null },
                Cache.GetMultiple(new string[] { "a", "b", "c" }));
        }

        [Test]
        public void Set_ForgetsEverything()
        {
            Cache.Set("key", "value");
            Assert.IsNull(Cache.Get("key"));
        }

        [Test]
        public void Set_WithOptions_ForgetsEverything()
        {
            Cache.Set("key", "value", new CacheOptions());
            Assert.IsNull(Cache.Get("key"));
        }

        [Test]
        public void Remove_HasNoSideEffects()
        {
            Cache.Remove("key");
        }

        [Test]
        public void GetOrPopulate_ForwardsToThePopulatorButForgetsTheResult()
        {
            Assert.AreEqual("value", Cache.GetOrPopulate("key", delegate(string key, out CacheOptions options)
            {
                options = new CacheOptions();
                Assert.AreEqual("key", key);
                return "value";
            }));
            Assert.IsNull(Cache.Get("key"));
        }

        [Test]
        public void Populate_ForwardsToThePopulatorButForgetsTheResult()
        {
            Assert.AreEqual("value", Cache.Populate("key", delegate(string key, out CacheOptions options)
            {
                options = new CacheOptions();
                Assert.AreEqual("key", key);
                return "value";
            }));
            Assert.IsNull(Cache.Get("key"));
        }
    }
}
