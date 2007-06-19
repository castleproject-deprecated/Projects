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
    /// <summary>
    /// Base-class for cache tests.
    /// Only common contract tests are verified here.
    /// </summary>
    [TestFixture]
    public abstract class BaseCacheTest : BaseUnitTest
    {
        protected delegate object PopulateDelegate(string key, Populator populator);

        private ICache cache;

        public ICache Cache
        {
            get { return cache; }
        }

        public abstract ICache CreateCache();

        public override void SetUp()
        {
            base.SetUp();

            cache = CreateCache();
        }

        #region Argument Exceptions
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainsKey_ThrowsIfKeyIsNull()
        {
            cache.ContainsKey(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ContainsKey_ThrowsIfKeyIsEmptyString()
        {
            cache.ContainsKey("");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Get_ThrowsIfKeyIsNull()
        {
            cache.Get(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Get_ThrowsIfKeyIsEmptyString()
        {
            cache.Get("");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetMultiple_ThrowsIfKeyArrayIsNull()
        {
            cache.GetMultiple(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetMultiple_ThrowsIfKeyIsNull()
        {
            cache.GetMultiple(new string[] { null });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMultiple_ThrowsIfKeyIsEmptyString()
        {
            cache.GetMultiple(new string[] { "" });
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetOrPopulate_ThrowsIfKeyIsNull()
        {
            cache.GetOrPopulate(null, NullPopulator);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetOrPopulate_ThrowsIfKeyIsEmptyString()
        {
            cache.GetOrPopulate("", NullPopulator);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetOrPopulate_ThrowsIfPopulatorIsNull()
        {
            cache.GetOrPopulate("abc", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Populate_ThrowsIfKeyIsNull()
        {
            cache.Populate(null, NullPopulator);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Populate_ThrowsIfKeyIsEmptyString()
        {
            cache.Populate("", NullPopulator);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Populate_ThrowsIfPopulatorIsNull()
        {
            cache.Populate("abc", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Set_ThrowsIfKeyIsNull()
        {
            cache.Set(null, "value");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Set_ThrowsIfKeyIsEmptyString()
        {
            cache.Set("", "value");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Set_ThrowsIfValueIsNull()
        {
            cache.Set("abc", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Set_WithOptions_ThrowsIfKeyIsNull()
        {
            cache.Set(null, "value", new CacheOptions());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Set_WithOptions_ThrowsIfKeyIsEmptyString()
        {
            cache.Set("", "value", new CacheOptions());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Set_WithOptions_ThrowsIfValueIsNull()
        {
            cache.Set("abc", null, new CacheOptions());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Remove_ThrowsIfKeyIsNull()
        {
            cache.Remove(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Remove_ThrowsIfKeyIsEmptyString()
        {
            cache.Remove("");
        }
        #endregion Argument Exceptions

        protected object NullPopulator(string key, out CacheOptions options)
        {
            options = new CacheOptions();
            return null;
        }
    }
}
