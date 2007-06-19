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
    [TestsOn(typeof(CacheOptions))]
    public class CacheOptionsTest
    {
        [Test]
        public void AbsoluteExpirationTime_GetterAndSetter()
        {
            CacheOptions options = new CacheOptions();
            Assert.IsNull(options.AbsoluteExpirationTime);

            options.AbsoluteExpirationTime = new DateTime(1970, 1, 3);
            Assert.AreEqual(new DateTime(1970, 1, 3), options.AbsoluteExpirationTime);
        }

        [Test]
        public void SlidingExpirationTime_GetterAndSetter()
        {
            CacheOptions options = new CacheOptions();
            Assert.IsNull(options.SlidingExpirationTimeSpan);

            options.SlidingExpirationTimeSpan = new TimeSpan(1, 2, 3);
            Assert.AreEqual(new TimeSpan(1, 2, 3), options.SlidingExpirationTimeSpan);
        }

        [Test]
        public void CustomOptions_EmptyUntilValuesAreAdded()
        {
            CacheOptions options = new CacheOptions();
            Assert.AreEqual(0, options.CustomOptions.Count);

            options.AddCustomOption("foo", "bar");
            options.AddCustomOption("xyzzy", "yiff");

            Assert.AreEqual(2, options.CustomOptions.Count);
            Assert.AreEqual("bar", options.CustomOptions["foo"]);
            Assert.AreEqual("yiff", options.CustomOptions["xyzzy"]);
        }

        [Test]
        public void AbsoluteExpirationFactory()
        {
            CacheOptions options = CacheOptions.AbsoluteExpiration(new DateTime(1970, 1, 3));
            Assert.AreEqual(new DateTime(1970, 1, 3), options.AbsoluteExpirationTime);
            Assert.IsNull(options.SlidingExpirationTimeSpan);
            Assert.AreEqual(0, options.CustomOptions.Count);
        }

        [Test]
        public void SlidingExpirationFactory()
        {
            CacheOptions options = CacheOptions.SlidingExpiration(new TimeSpan(1, 2, 3));
            Assert.AreEqual(new TimeSpan(1, 2, 3), options.SlidingExpirationTimeSpan);
            Assert.IsNull(options.AbsoluteExpirationTime);
            Assert.AreEqual(0, options.CustomOptions.Count);
        }

        [Test]
        public void NoExpirationFactory()
        {
            CacheOptions options = CacheOptions.NoExpiration;
            Assert.IsNull(options.AbsoluteExpirationTime);
            Assert.IsNull(options.SlidingExpirationTimeSpan);
            Assert.AreEqual(0, options.CustomOptions.Count);
        }

        [Test]
        public void GetUtcExpirationTimeRelativeToNow_WithAbsoluteExpirationTime()
        {
            CacheOptions options = new CacheOptions();
            options.AbsoluteExpirationTime = new DateTime(1970, 1, 3, 0, 0, 0, DateTimeKind.Utc);

            Assert.AreEqual(new DateTime(1970, 1, 3, 0, 0, 0, DateTimeKind.Utc), options.GetUtcExpirationTimeRelativeToNow());
        }

        [Test]
        public void GetUtcExpirationTimeRelativeToNow_WithSlidingExpirationTime()
        {
            CacheOptions options = new CacheOptions();
            options.SlidingExpirationTimeSpan = new TimeSpan(1, 2, 3);

            TimeSpan actualSpan = options.GetUtcExpirationTimeRelativeToNow() - DateTime.UtcNow;
            Assert.Between(actualSpan, new TimeSpan(1, 2, 1), new TimeSpan(1, 2, 5));
        }

        [Test]
        public void GetUtcExpirationTimeRelativeToNow_NoExpirationTime()
        {
            CacheOptions options = new CacheOptions();
            Assert.AreEqual(DateTime.MaxValue, options.GetUtcExpirationTimeRelativeToNow());
        }
    }
}
