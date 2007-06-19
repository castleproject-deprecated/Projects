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
using System.Web;
using Castle.Components.Cache.Tests;
using MbUnit.Framework;

namespace Castle.Components.Cache.SysCache.Tests
{
    [TestFixture]
    [TestsOn(typeof(SysCache))]
    public class SysCacheTest : TypicalCacheTest
    {
        new public SysCache Cache
        {
            get { return (SysCache)base.Cache; }
        }

        public override ICache CreateCache()
        {
            ICache cache = new SysCache();
            cache.Flush(); // ensure the cache is clean before starting

            return cache;
        }

        [Test]
        public void InnerCache_ReturnsGlobalASPNetCache()
        {
            Assert.AreSame(HttpRuntime.Cache, Cache.InnerCache);
        }
    }
}
