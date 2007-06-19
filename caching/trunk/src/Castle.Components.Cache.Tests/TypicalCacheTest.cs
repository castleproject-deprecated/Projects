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
using MbUnit.Framework;

namespace Castle.Components.Cache.Tests
{
    /// <summary>
    /// Over short time spans, we expect most cases will persist the values
    /// we give them without any problems.  This isn't the case for caches
    /// like NullCache which we consider a-typical and don't handle here.
    /// </summary>
    /// <remarks>
    /// Cache implementations may vary in terms of their expected response
    /// so the tests may be parameterized by a set of policy parameters that can
    /// be tuned accordingly for each cache type.  It is expected that additional
    /// specific tests will be defined for each cache implementation elsewhere.
    /// </remarks>
    public abstract class TypicalCacheTest : BaseCacheTest
    {
        private delegate void Procedure();

        /// <summary>
        /// Set to true if the cache blocks on Get while the Populator
        /// for that key is running.
        /// </summary>
        public bool BlocksGetWhilePopulatorForThatKeyIsRunning = true;

        [Test]
        public void ContainsKeyReturnsUpToDateResultAfterSetOrRemove()
        {
            Assert.IsFalse(Cache.ContainsKey("key"));

            Cache.Set("key", "value");
            Assert.IsTrue(Cache.ContainsKey("key"));

            Cache.Set("key", "newvalue");
            Assert.IsTrue(Cache.ContainsKey("key"));

            Cache.Remove("key");
            Assert.IsFalse(Cache.ContainsKey("key"));

            Cache.Set("key", "NEW", new CacheOptions());
            Assert.IsTrue(Cache.ContainsKey("key"));
        }
        
        [Test]
        public void GetReturnsUpToDateValuesAfterSetOrRemove()
        {
            Assert.IsNull(Cache.Get("key"));

            Cache.Set("key", "value");
            Assert.AreEqual("value", Cache.Get("key"));

            Cache.Set("key", "newvalue");
            Assert.AreEqual("newvalue", Cache.Get("key"));

            Cache.Remove("key");
            Assert.IsNull(Cache.Get("key"));

            Cache.Set("key", "NEW", new CacheOptions());
            Assert.AreEqual("NEW", Cache.Get("key"));
        }

        [Test]
        public void GetMultipleReturnsUpToDateValuesAfterSetOrRemove()
        {
            Cache.Set("key1", "abc");
            Cache.Set("key3", "def");
            CollectionAssert.AreElementsEqual(new object[] { "abc", null, "def" },
                Cache.GetMultiple(new string[] { "key1", "key2", "key3" }));

            Cache.Remove("key1");
            Cache.Set("key2", "ghi", new CacheOptions());
            Cache.Set("key3", "DEF");
            CollectionAssert.AreElementsEqual(new object[] { null, "ghi", "DEF" },
                Cache.GetMultiple(new string[] { "key1", "key2", "key3" }));
        }

        [Test]
        public void FlushClearsAllValues()
        {
            Cache.Set("key1", "abc");
            Cache.Set("key2", "def");
            Cache.Set("key3", "ghi");
            Cache.Flush();

            CollectionAssert.AreElementsEqual(new object[] { null, null, null },
                Cache.GetMultiple(new string[] { "key1", "key2", "key3" }));
            Assert.IsFalse(Cache.ContainsKey("key1"));
            Assert.IsFalse(Cache.ContainsKey("key2"));
            Assert.IsFalse(Cache.ContainsKey("key3"));
        }

        [Test]
        public void GetOrPopulateLazilyPopulatesTheValueWhenAbsent()
        {
            Assert.AreEqual("value", Cache.GetOrPopulate("key", delegate(string key, out CacheOptions options)
            {
                options = new CacheOptions();
                Assert.AreEqual("key", key);
                return "value";
            }));

            Assert.IsTrue(Cache.ContainsKey("key"));
            Assert.AreEqual("value", Cache.Get("key"));
        }

        [Test]
        public void GetOrPopulateDoesNothingIfTheValueIsAlreadyPopulated()
        {
            Cache.Set("key", "EXISTING");

            Assert.AreEqual("EXISTING", Cache.GetOrPopulate("key", delegate(string key, out CacheOptions options)
            {
                throw new Exception("Should not get here!");
            }));

            Assert.IsTrue(Cache.ContainsKey("key"));
            Assert.AreEqual("EXISTING", Cache.Get("key"));
        }

        [Test]
        public void GetOrPopulateReturnsNullAndDoesAbsolutelyNothingElseIfThePopulatorReturnsNull()
        {
            Assert.IsNull(Cache.GetOrPopulate("key", delegate(string key, out CacheOptions options)
            {
                options = new CacheOptions();
                Assert.AreEqual("key", key);
                return null;
            }));

            Assert.IsFalse(Cache.ContainsKey("key"));
            Assert.IsNull(Cache.Get("key"));
        }

        [Test]
        public void GetOrPopulateRecoversFromExceptionsSafely()
        {
            try
            {
                Cache.GetOrPopulate("key", delegate(string key, out CacheOptions options)
                {
                    throw new Exception("Boom!");
                });

                Assert.Fail("Expected an exception to be thrown by the populator.");
            }
            catch (Exception ex)
            {
                // Expected.
                Assert.AreEqual("Boom!", ex.Message);
            }

            // Try again to ensure we have recovered.
            // Note: This helps to ensure that our exception cleanup logic works
            //       for caches that do more sophisticated locking during population.
            Assert.AreEqual("value", Cache.GetOrPopulate("key", delegate(string key, out CacheOptions options)
            {
                options = new CacheOptions();
                Assert.AreEqual("key", key);
                return "value";
            }));

            Assert.IsTrue(Cache.ContainsKey("key"));
            Assert.AreEqual("value", Cache.Get("key"));
        }

        [Test]
        public void PopulatePopulatesTheValueWhenAbsent()
        {
            Assert.AreEqual("value", Cache.Populate("key", delegate(string key, out CacheOptions options)
            {
                options = new CacheOptions();
                Assert.AreEqual("key", key);
                return "value";
            }));

            Assert.IsTrue(Cache.ContainsKey("key"));
            Assert.AreEqual("value", Cache.Get("key"));
        }

        [Test]
        public void PopulateOverwritesTheExistingValueWhenPresent()
        {
            Cache.Set("key", "EXISTING");

            Assert.AreEqual("value", Cache.Populate("key", delegate(string key, out CacheOptions options)
            {
                options = new CacheOptions();
                Assert.AreEqual("key", key);
                return "value";
            }));

            Assert.IsTrue(Cache.ContainsKey("key"));
            Assert.AreEqual("value", Cache.Get("key"));
        }

        [Test]
        public void PopulateReturnsNullAndDoesAbsolutelyNothingElseIfThePopulatorReturnsNull()
        {
            Assert.IsNull(Cache.Populate("key", delegate(string key, out CacheOptions options)
            {
                options = new CacheOptions();
                Assert.AreEqual("key", key);
                return null;
            }));

            Assert.IsFalse(Cache.ContainsKey("key"));
            Assert.IsNull(Cache.Get("key"));
        }

        [Test]
        public void PopulateRecoversFromExceptionsSafely()
        {
            try
            {
                Cache.Populate("key", delegate(string key, out CacheOptions options)
                {
                    throw new Exception("Boom!");
                });

                Assert.Fail("Expected an exception to be thrown by the populator.");
            }
            catch (Exception ex)
            {
                // Expected.
                Assert.AreEqual("Boom!", ex.Message);
            }

            // Try again to ensure we have recovered.
            // Note: This helps to ensure that our exception cleanup logic works
            //       for caches that do more sophisticated locking during population.
            Assert.AreEqual("value", Cache.Populate("key", delegate(string key, out CacheOptions options)
            {
                options = new CacheOptions();
                Assert.AreEqual("key", key);
                return "value";
            }));

            Assert.IsTrue(Cache.ContainsKey("key"));
            Assert.AreEqual("value", Cache.Get("key"));
        }

        [RowTest]
        [Row(true, 6)]
        [Row(false, 6)]
        public void Populate_ConcurrentGetOfSameKey(bool useGetOrPopulate, int numberOfConsumers)
        {
            ManualResetEvent sem = new ManualResetEvent(false);
            int populationCount = 0;
            Populator populator = delegate(string key, out CacheOptions options)
            {
                if (populationCount == 0)
                {
                    sem.Set();
                    Thread.Sleep(200);
                }

                populationCount += 1;
                Assert.AreEqual("key", key);
                options = CacheOptions.NoExpiration;
                return "value";
            };

            // Prepare a bunch of threads to concurrently get the same key.
            // They should all see the same value when released.
            // The populator will be called a varying number of times depending
            // on how the cache is implemented.
            Action<bool> proc = delegate(bool useGet)
            {
                sem.WaitOne();

                object value = useGet ? Cache.Get("key") : Cache.GetOrPopulate("key", populator);

                if (BlocksGetWhilePopulatorForThatKeyIsRunning)
                    Assert.AreEqual("value", value);
                else
                    Assert.IsTrue(value == null || value.Equals("value"));
            };

            IAsyncResult[] consumers = new IAsyncResult[numberOfConsumers];
            for (int i = 0; i < numberOfConsumers; i++)
                consumers[i] = proc.BeginInvoke(i % 2 == 0, null, this);

            // Now actually go populate it.
            Assert.AreEqual("value", useGetOrPopulate ? Cache.GetOrPopulate("key", populator) :
                Cache.Populate("key", populator));
            Assert.AreEqual("value", Cache.Get("key"));

            // Wait for the consumers to finish and ensure they did not fail.
            foreach (IAsyncResult consumer in consumers)
                proc.EndInvoke(consumer);

            // Ensure population happened the expected number of times.
            if (BlocksGetWhilePopulatorForThatKeyIsRunning)
                Assert.AreEqual(1, populationCount);
            else
                Assert.GreaterEqualThan(populationCount, 1);
        }

        [RowTest]
        [Row(true, 6)]
        [Row(false, 6)]
        public void Populate_ConcurrentPopulateIsCalledEveryTimeAndSeesOwnValues(bool useGetOrPopulate,
            int numberOfConsumers)
        {
            ManualResetEvent sem = new ManualResetEvent(false);
            int populationCount = 0;
            Populator populator = delegate(string key, out CacheOptions options)
            {
                if (populationCount == 0)
                {
                    sem.Set();
                    Thread.Sleep(200);
                }

                populationCount += 1;
                Assert.AreEqual("key", key);
                options = CacheOptions.NoExpiration;
                return "value" + Thread.CurrentThread.ManagedThreadId;
            };

            // Prepare a bunch of threads to concurrently populate the same key.
            // They should all see their own populated values when released
            // regardless of what the others are doing.
            Procedure proc = delegate
            {
                sem.WaitOne();
                Assert.AreEqual("value" + Thread.CurrentThread.ManagedThreadId,
                    Cache.Populate("key", populator));
            };

            IAsyncResult[] consumers = new IAsyncResult[numberOfConsumers];
            for (int i = 0; i < numberOfConsumers; i++)
                consumers[i] = proc.BeginInvoke(null, this);

            // Now actually go populate it.
            Assert.AreEqual("value" + Thread.CurrentThread.ManagedThreadId,
                useGetOrPopulate ? Cache.GetOrPopulate("key", populator) :
                Cache.Populate("key", populator));

            // Wait for the consumers to finish and ensure they did not fail.
            foreach (IAsyncResult consumer in consumers)
                proc.EndInvoke(consumer);

            // Ensure population happened the expected number of times.
            Assert.AreEqual(1 + numberOfConsumers, populationCount);
        }
    }
}
