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
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MbUnit.Framework;
using Memcached.ClientLibrary;

namespace Castle.Components.Cache.Memcached.Tests
{
    [TestFixture]
    [TestsOn(typeof(MemcachedCachePool))]
    public class MemcachedCachePoolTest
    {
        private MemcachedCachePool pool;

        [SetUp]
        public void SetUp()
        {
            pool = new MemcachedCachePool();
        }

        [TearDown]
        public void TearDown()
        {
            if (pool != null)
            {
                pool.Dispose();
                pool = null;
            }
        }

        [Test]
        public void DefaultPortConstantIsProvided()
        {
            Assert.AreEqual(11211, MemcachedCachePool.DefaultPort);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithPoolName_ThrowsIfPoolNameIsNull()
        {
            new MemcachedCachePool((string)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithSockIOPool_ThrowsIfPoolIsNull()
        {
            new MemcachedCachePool((SockIOPool)null);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Constructor_WithSockIOPool_ThrowsIfPoolWasDisposed()
        {
            SockIOPool oldSockIOPool = pool.SockIOPool;
            pool.Dispose();

            new MemcachedCachePool(oldSockIOPool);
        }

        [Test]
        public void Constructor_Default_SetsProperties()
        {
            Assert.AreSame(SockIOPool.GetInstance(), pool.SockIOPool);
            Assert.IsNotNull(pool.PoolName);
        }

        [Test]
        public void Constructor_WithPoolName_SetsProperties()
        {
            pool.Dispose();

            pool = new MemcachedCachePool("pool");
            Assert.AreEqual("pool", pool.PoolName);
            Assert.AreSame(SockIOPool.GetInstance("pool"), pool.SockIOPool);
        }

        [Test]
        public void Constructor_WithSockIOPool_SetsProperties()
        {
            pool.Dispose();

            pool = new MemcachedCachePool(SockIOPool.GetInstance("pool"));
            Assert.AreEqual("pool", pool.PoolName);
            Assert.AreSame(SockIOPool.GetInstance("pool"), pool.SockIOPool);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SockIOPool_ThrowsWhenDisposed()
        {
            pool.Dispose();
            GC.KeepAlive(pool.SockIOPool);
        }

        [Test]
        public void PoolName_ReturnsSameNameWhenDisposed()
        {
            string oldName = pool.PoolName;

            pool.Dispose();
            Assert.AreEqual(oldName, pool.PoolName);
        }

        [Test]
        public void IsRunning_ReturnsFalseWhenDisposed()
        {
            StartPool();
            Assert.IsTrue(pool.IsRunning);

            pool.Dispose();
            Assert.IsFalse(pool.IsRunning);
        }

        [Test]
        public void Servers_GetterAndSetter()
        {
            CollectionAssert.AreElementsEqual(new string[] { }, pool.Servers);

            pool.Servers = new string[] { "localhost", "192.168.0.1:2001" };
            CollectionAssert.AreElementsEqual(new string[] { "127.0.0.1:11211", "192.168.0.1:2001" }, pool.Servers);
            CollectionAssert.AreElementsEqual(new string[] { "127.0.0.1:11211", "192.168.0.1:2001" }, pool.SockIOPool.Servers);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Servers_GetterThrowsIfDisposed()
        {
            pool.Dispose();
            GC.KeepAlive(pool.Servers);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Servers_SetterThrowsIfDisposed()
        {
            pool.Dispose();
            pool.Servers = new string[] { };
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Servers_ThrowsIfValueIsNull()
        {
            pool.Servers = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Servers_ThrowsIfAnElementOfTheValueIsNull()
        {
            pool.Servers = new string[] { null };
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Servers_ThrowsIfAnElementOfTheValueIsMalformed()
        {
            pool.Servers = new string[] { ":::" };
        }

        [Test]
        [ExpectedException(typeof(SocketException))]
        public void Servers_ThrowsIfAnElementOfTheValueIsAnUnknownHostname()
        {
            pool.Servers = new string[] { "no-such.no-such" };
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Servers_ThrowsWhenSettingIfPoolStarted()
        {
            StartPool();
            pool.Servers = new string[] { };
        }

        [Test]
        public void ServerWeights_GetterAndSetter()
        {
            Assert.IsNull(pool.ServerWeights);

            int[] value = new int[] { 1, 2 };
            pool.ServerWeights = value;
            CollectionAssert.AreElementsEqual(value, pool.ServerWeights);
            CollectionAssert.AreElementsEqual(value, pool.SockIOPool.Weights);

            value = null;
            pool.ServerWeights = value;
            Assert.IsNull(pool.ServerWeights);
            Assert.IsNull(pool.SockIOPool.Weights);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ServerWeights_GetterThrowsIfDisposed()
        {
            pool.Dispose();
            GC.KeepAlive(pool.ServerWeights);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ServerWeights_SetterThrowsIfDisposed()
        {
            pool.Dispose();
            pool.ServerWeights = new int[] { };
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ServerWeights_ThrowsWhenSettingIfPoolStarted()
        {
            StartPool();
            pool.ServerWeights = new int[] { };
        }

        [Test]
        public void InitialConnections_GetterAndSetter()
        {
            Assert.AreEqual(3, pool.InitialConnections);

            int value = 5;
            pool.InitialConnections = value;
            Assert.AreEqual(value, pool.InitialConnections);
            Assert.AreEqual(value, pool.SockIOPool.InitConnections);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void InitialConnections_GetterThrowsIfDisposed()
        {
            pool.Dispose();
            GC.KeepAlive(pool.InitialConnections);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void InitialConnections_SetterThrowsIfDisposed()
        {
            pool.Dispose();
            pool.InitialConnections = 1;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InitialConnections_ThrowsIfValueIsNegative()
        {
            pool.InitialConnections = -1;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InitialConnections_ThrowsWhenSettingIfPoolStarted()
        {
            StartPool();
            pool.InitialConnections = 1;
        }

        [Test]
        public void MinSpareConnections_GetterAndSetter()
        {
            Assert.AreEqual(3, pool.MinSpareConnections);

            int value = 5;
            pool.MinSpareConnections = value;
            Assert.AreEqual(value, pool.MinSpareConnections);
            Assert.AreEqual(value, pool.SockIOPool.MinConnections);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void MinSpareConnections_GetterThrowsIfDisposed()
        {
            pool.Dispose();
            GC.KeepAlive(pool.MinSpareConnections);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void MinSpareConnections_SetterThrowsIfDisposed()
        {
            pool.Dispose();
            pool.MinSpareConnections = 1;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MinSpareConnections_ThrowsIfValueIsNegative()
        {
            pool.MinSpareConnections = -1;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MinSpareConnections_ThrowsWhenSettingIfPoolStarted()
        {
            StartPool();
            pool.MinSpareConnections = 1;
        }

        [Test]
        public void MaxSpareConnections_GetterAndSetter()
        {
            Assert.AreEqual(10, pool.MaxSpareConnections);

            int value = 5;
            pool.MaxSpareConnections = value;
            Assert.AreEqual(value, pool.MaxSpareConnections);
            Assert.AreEqual(value, pool.SockIOPool.MaxConnections);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void MaxSpareConnections_GetterThrowsIfDisposed()
        {
            pool.Dispose();
            GC.KeepAlive(pool.MaxSpareConnections);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void MaxSpareConnections_SetterThrowsIfDisposed()
        {
            pool.Dispose();
            pool.MaxSpareConnections = 1;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MaxSpareConnections_ThrowsIfValueIsNegative()
        {
            pool.MaxSpareConnections = -1;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MaxSpareConnections_ThrowsWhenSettingIfPoolStarted()
        {
            StartPool();
            pool.MaxSpareConnections = 1;
        }

        [Test]
        public void IdleThreadTimeout_GetterAndSetter()
        {
            Assert.AreEqual(180000, pool.IdleThreadTimeout);

            int value = 5;
            pool.IdleThreadTimeout = value;
            Assert.AreEqual(value, pool.IdleThreadTimeout);
            Assert.AreEqual(value, pool.SockIOPool.MaxIdle);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void IdleThreadTimeout_GetterThrowsIfDisposed()
        {
            pool.Dispose();
            GC.KeepAlive(pool.IdleThreadTimeout);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void IdleThreadTimeout_SetterThrowsIfDisposed()
        {
            pool.Dispose();
            pool.IdleThreadTimeout = 1;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IdleThreadTimeout_ThrowsIfValueIsNegative()
        {
            pool.IdleThreadTimeout = -1;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IdleThreadTimeout_ThrowsWhenSettingIfPoolStarted()
        {
            StartPool();
            pool.IdleThreadTimeout = 1;
        }

        [Test]
        public void BusyThreadTimeout_GetterAndSetter()
        {
            Assert.AreEqual(300000, pool.BusyThreadTimeout);

            int value = 5;
            pool.BusyThreadTimeout = value;
            Assert.AreEqual(value, pool.BusyThreadTimeout);
            Assert.AreEqual(value, pool.SockIOPool.MaxBusy);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void BusyThreadTimeout_GetterThrowsIfDisposed()
        {
            pool.Dispose();
            GC.KeepAlive(pool.BusyThreadTimeout);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void BusyThreadTimeout_SetterThrowsIfDisposed()
        {
            pool.Dispose();
            pool.BusyThreadTimeout = 1;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BusyThreadTimeout_ThrowsIfValueIsNegative()
        {
            pool.BusyThreadTimeout = -1;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BusyThreadTimeout_ThrowsWhenSettingIfPoolStarted()
        {
            StartPool();
            pool.BusyThreadTimeout = 1;
        }

        [Test]
        public void MaintenanceInterval_GetterAndSetter()
        {
            Assert.AreEqual(5000, pool.MaintenanceInterval);

            int value = 5;
            pool.MaintenanceInterval = value;
            Assert.AreEqual(value, pool.MaintenanceInterval);
            Assert.AreEqual(value, pool.SockIOPool.MaintenanceSleep);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void MaintenanceInterval_GetterThrowsIfDisposed()
        {
            pool.Dispose();
            GC.KeepAlive(pool.MaintenanceInterval);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void MaintenanceInterval_SetterThrowsIfDisposed()
        {
            pool.Dispose();
            pool.MaintenanceInterval = 1;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MaintenanceInterval_ThrowsIfValueIsNegative()
        {
            pool.MaintenanceInterval = -1;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MaintenanceInterval_ThrowsWhenSettingIfPoolStarted()
        {
            StartPool();
            pool.MaintenanceInterval = 1;
        }

        [Test]
        public void EnableFailover_GetterAndSetter()
        {
            Assert.AreEqual(true, pool.EnableFailover);

            bool value = false;
            pool.EnableFailover = value;
            Assert.AreEqual(value, pool.EnableFailover);
            Assert.AreEqual(value, pool.SockIOPool.Failover);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void EnableFailover_GetterThrowsIfDisposed()
        {
            pool.Dispose();
            GC.KeepAlive(pool.EnableFailover);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void EnableFailover_SetterThrowsIfDisposed()
        {
            pool.Dispose();
            pool.EnableFailover = true;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EnableFailover_ThrowsWhenSettingIfPoolStarted()
        {
            StartPool();
            pool.EnableFailover = true;
        }

        [Test]
        public void EnableSocketNagleOption_GetterAndSetter()
        {
            Assert.AreEqual(true, pool.EnableSocketNagleOption);

            bool value = false;
            pool.EnableSocketNagleOption = value;
            Assert.AreEqual(value, pool.EnableSocketNagleOption);
            Assert.AreEqual(value, pool.SockIOPool.Nagle);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void EnableSocketNagleOption_GetterThrowsIfDisposed()
        {
            pool.Dispose();
            GC.KeepAlive(pool.EnableSocketNagleOption);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void EnableSocketNagleOption_SetterThrowsIfDisposed()
        {
            pool.Dispose();
            pool.EnableSocketNagleOption = true;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EnableSocketNagleOption_ThrowsWhenSettingIfPoolStarted()
        {
            StartPool();
            pool.EnableSocketNagleOption = true;
        }

        [Test]
        public void SocketConnectTimeout_GetterAndSetter()
        {
            Assert.AreEqual(50, pool.SocketConnectTimeout);

            pool.SocketConnectTimeout = 5;
            Assert.AreEqual(5, pool.SocketConnectTimeout);
            Assert.AreEqual(5, pool.SockIOPool.SocketConnectTimeout);

            pool.SocketConnectTimeout = -1;
            Assert.AreEqual(-1, pool.SocketConnectTimeout);
            Assert.AreEqual(-1, pool.SockIOPool.SocketConnectTimeout);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SocketConnectTimeout_GetterThrowsIfDisposed()
        {
            pool.Dispose();
            GC.KeepAlive(pool.SocketConnectTimeout);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SocketConnectTimeout_SetterThrowsIfDisposed()
        {
            pool.Dispose();
            pool.SocketConnectTimeout = 1;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SocketConnectTimeout_ThrowsWhenSettingIfPoolStarted()
        {
            StartPool();
            pool.SocketConnectTimeout = 1;
        }

        [Test]
        public void HashingAlgorithm_GetterAndSetter()
        {
            Assert.AreEqual(HashingAlgorithm.Native, pool.HashingAlgorithm);

            HashingAlgorithm value = HashingAlgorithm.NewCompatibleHash;
            pool.HashingAlgorithm = value;
            Assert.AreEqual(value, pool.HashingAlgorithm);
            Assert.AreEqual(value, pool.SockIOPool.HashingAlgorithm);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void HashingAlgorithm_GetterThrowsIfDisposed()
        {
            pool.Dispose();
            GC.KeepAlive(pool.HashingAlgorithm);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void HashingAlgorithm_SetterThrowsIfDisposed()
        {
            pool.Dispose();
            pool.HashingAlgorithm = HashingAlgorithm.Native;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void HashingAlgorithm_ThrowsWhenSettingIfPoolStarted()
        {
            StartPool();
            pool.HashingAlgorithm = HashingAlgorithm.Native;
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Start_ThrowsIfDisposed()
        {
            pool.Dispose();
            StartPool();
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Stop_ThrowsIfDisposed()
        {
            pool.Dispose();
            pool.Stop();
        }

        [Test]
        public void VerifyTheWholeLifecycleOfARunningPool()
        {
            StartPool();
            Assert.IsTrue(pool.IsRunning);

            MemcachedClient client = new MemcachedClient();
            client.PoolName = pool.PoolName;
            client.Set("key", "value");
            Assert.AreEqual("value", client.Get("key"));

            pool.Stop();
            Assert.IsFalse(pool.IsRunning);
        }

        private void StartPool()
        {
            pool.Servers = new string[] { "127.0.0.1" };
            pool.Start();

            Thread.Sleep(200); // FIXME: There seems to be a delay before the 
            // first sockets in the pool become usable.  If I remove this sleep then
            // the tests will fail because they cannot obtain an open connection to
            // the server from the available pool.
        }
    }
}
