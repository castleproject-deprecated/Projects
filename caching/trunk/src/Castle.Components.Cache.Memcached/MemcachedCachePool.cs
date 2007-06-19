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
using System.Collections;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Castle.Components.Cache.Collections;
using Castle.Core;
using Memcached.ClientLibrary;

namespace Castle.Components.Cache.Memcached
{
    /// <summary>
    /// A Memcached cache pool manages a pool of Memcached servers that store
    /// the contents of a <see cref="MemcachedCache" />.  The pool consists
    /// of one or more servers (specified by their hostname or IP address)
    /// that provide the Memcached server service.  A pool must be created
    /// and started before a <see cref="MemcachedCache" /> can be used.
    /// </summary>
    /// <remarks>
    /// The Memcached .Net client library is LGPL (c) 2005 Tim Gebhardt.
    /// Please refer to the Memcache .Net client library and to the Memcached
    /// server document for more information.
    /// </remarks>
    [Singleton]
    public class MemcachedCachePool : IStartable, IDisposable
    {
        /// <summary>
        /// The default Memcached port number.
        /// </summary>
        /// <value>11211</value>
        public const int DefaultPort = 11211;

        private SockIOPool pool;
        private string poolName;

        /// <summary>
        /// Creates a wrapper for the default <see cref="SockIOPool" />.
        /// </summary>
        public MemcachedCachePool() : this(SockIOPool.GetInstance())
        {
        }

        /// <summary>
        /// Creates a wrapper for an existing <see cref="SockIOPool" />.
        /// </summary>
        /// <param name="pool">The existing pool</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pool"/> is null</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public MemcachedCachePool(SockIOPool pool)
        {
            if (pool == null)
                throw new ArgumentNullException("pool");

            SetPool(pool, GetSockIOPoolName(pool));
        }

        /// <summary>
        /// Creates a wrapper for a the <see cref="SockIOPool"/> instance with
        /// the specified pool name.  The pool name must be unique.
        /// </summary>
        /// <param name="poolName">The pool name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="poolName"/> is null</exception>
        public MemcachedCachePool(string poolName)
        {
            if (poolName == null)
                throw new ArgumentNullException("poolName");

            SetPool(SockIOPool.GetInstance(poolName), poolName);
        }

        /// <summary>
        /// Disposes of a pool.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (pool != null)
                {
                    pool.Shutdown();
                    DisposeSockIOPool(poolName, pool);

                    pool = null;
                }
            }
        }

        /// <summary>
        /// Gets the underlying <see cref="SockIOPool" /> wrapped by this instance.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public SockIOPool SockIOPool
        {
            get
            {
                ThrowIfPoolDisposed();
                return pool;
            }
        }

        /// <summary>
        /// Gets the name of the cache pool.
        /// Return the name of the pool even after it has been disposed.
        /// </summary>
        public string PoolName
        {
            get { return poolName; }
        }

        /// <summary>
        /// Returns true if the pool has not been disposed and is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                lock (this)
                    return pool != null && pool.Initialized;
            }
        }

        /// <summary>
        /// Gets or sets the list of all cache servers in the pool.
        /// </summary>
        /// <value>
        /// <para>
        /// The array of servers specified as "hostname" or "hostname:port".
        /// For convenience, hostnames are resolved to IP addresses when the property is set.
        /// Reading this property back will yield "ip-address:port" pairs even if a
        /// symbolic hostname was originally set.
        /// </para>
        /// <para>
        /// The default port for Memcached servers is 11211.
        /// </para>
        /// <para>
        /// The default value is an empty array.  This property must be set to a
        /// non-empty array before the pool is started.
        /// </para>
        /// </value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> or one of its
        /// elements is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> contains values
        /// that are not formatted as "ip-address:port" pairs</exception>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the property and the pool has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        /// <exception cref="SocketException">Thrown if one of the specified host names is unknown</exception>
        public string[] Servers
        {
            get
            {
                ThrowIfPoolDisposed();
                return (string[])pool.Servers.ToArray(typeof(string));
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                // The Memcached library does not report invalid server ip:port
                // port formatting errors very nicely (it throws an ArrayIndexOutOfBoundsException)
                // so we do a little extra work here to make the failure more obvious.
                // It seemed worthwhile to add extra checks after I spent 10 minutes figuring
                // out what was broken.  Then I needed to be able to resolve hostnames
                // to IPs, fortunately I already had a good place for it... -- Jeff.
                string[] resolvedValues = Array.ConvertAll<string, string>(value, delegate(string server)
                {
                    if (server == null)
                        throw new ArgumentNullException("value", "Must not contain nulls.");

                    string[] serverParts = server.Split(':');
                    int port = DefaultPort;
                    if (serverParts.Length == 0 || serverParts.Length > 2 ||
                        serverParts.Length == 2 && !int.TryParse(serverParts[1], out port))
                        throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                            "Server '{0}' was specified in the form 'hostname' or 'hostname:port'.", server),
                            "value");

                    IPAddress address = Dns.GetHostAddresses(serverParts[0])[0];
                    return address + ":" + port;
                });

                ThrowIfPoolDisposed();
                ThrowIfPoolStarted();
                pool.SetServers(resolvedValues);
            }
        }

        /// <summary>
        /// Gets or sets the weights assigned to each cache server in the pool.
        /// If set to null (the default), all servers will have the same weight.
        /// </summary>
        /// <value>
        /// <para>
        /// The array of server weights where each element corresponds to
        /// the server in the same position in the <see cref="Servers" /> list.
        /// Servers with heigher weight receive more traffic.
        /// </para>
        /// <para>
        /// The default value is null.
        /// </para>
        /// </value>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the property and the pool has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public int[] ServerWeights
        {
            get
            {
                ThrowIfPoolDisposed();

                ArrayList weights = pool.Weights;
                return weights != null ? (int[])weights.ToArray(typeof(int)) : null;
            }
            set
            {
                ThrowIfPoolDisposed();
                ThrowIfPoolStarted();

                if (value != null)
                    pool.SetWeights(value);
                else
                    pool.SetWeights((ArrayList)null);
            }
        }

        /// <summary>
        /// Gets or sets the number of initial connections to establish per server
        /// in the pool.
        /// </summary>
        /// <value>
        /// The default value is 3.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 0</exception>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the property and the pool has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public int InitialConnections
        {
            get
            {
                ThrowIfPoolDisposed();
                return pool.InitConnections;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "The number of initial connections must not be less than 0.");

                ThrowIfPoolDisposed();
                ThrowIfPoolStarted();
                pool.InitConnections = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum number of spare connections to maintain in the pool.
        /// </summary>
        /// <value>
        /// The default value is 3.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 0</exception>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the property and the pool has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public int MinSpareConnections
        {
            get
            {
                ThrowIfPoolDisposed();
                return pool.MinConnections;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "The minimum number of spare connections must not be less than 0.");

                ThrowIfPoolDisposed();
                ThrowIfPoolStarted();
                pool.MinConnections = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of spare connections to maintain in the pool.
        /// </summary>
        /// <value>
        /// The default value is 10.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 0</exception>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the property and the pool has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public int MaxSpareConnections
        {
            get
            {
                ThrowIfPoolDisposed();
                return pool.MaxConnections;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "The maximum number of spare connections must not be less than 0.");

                ThrowIfPoolDisposed();
                ThrowIfPoolStarted();
                pool.MaxConnections = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum amount of time available threads are allowed to remain
        /// idle before they are disposed in milliseconds.
        /// </summary>
        /// <value>
        /// The default value is 3 minutes (180,000).
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 0</exception>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the property and the pool has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public long IdleThreadTimeout
        {
            get
            {
                ThrowIfPoolDisposed();
                return pool.MaxIdle;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "The idle thread timeout must not be less than 0.");

                ThrowIfPoolDisposed();
                ThrowIfPoolStarted();
                pool.MaxIdle = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum amount of time busy threads are allowed to remain busy
        /// before they are aborted in milliseconds.
        /// </summary>
        /// <value>
        /// The default value is 5 minutes (300,000).
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 0</exception>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the property and the pool has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public long BusyThreadTimeout
        {
            get
            {
                ThrowIfPoolDisposed();
                return pool.MaxBusy;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "The busy thread timeout must not be less than 0.");

                ThrowIfPoolDisposed();
                ThrowIfPoolStarted();
                pool.MaxBusy = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of milliseconds between runs of the pool's
        /// socket and thread maintenance task which ensures that the pool contains the
        /// expected number of spare connections.  If set to 0, the maintenance
        /// thread will not be started.
        /// </summary>
        /// <value>
        /// The default value is 5 seconds (5,000).
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 0</exception>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the property and the pool has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public long MaintenanceInterval
        {
            get
            {
                ThrowIfPoolDisposed();
                return pool.MaintenanceSleep;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "The maintenance interval must not be less than 0.");

                ThrowIfPoolDisposed();
                ThrowIfPoolStarted();
                pool.MaintenanceSleep = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the pool will allow another server
        /// to service a cache request if a socket could not be established
        /// to the requested server.  Otherwise the cache request will fail.
        /// </summary>
        /// <value>
        /// The default value is true.
        /// </value>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the property and the pool has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public bool EnableFailover
        {
            get
            {
                ThrowIfPoolDisposed();
                return pool.Failover;
            }
            set
            {
                ThrowIfPoolDisposed();
                ThrowIfPoolStarted();
                pool.Failover = value;
            }
        }

#if false
        /// Note: In the Memcached .Net client v1.1.4, the value of the socket timeout property is ignored!
        /// <summary>
        /// Gets or sets the socket send and receive timeout in milliseconds.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 0</exception>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the property and the pool has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public int SocketTimeout
        {
            get { return pool.SocketTimeout; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "The socket timeout must not be less than 0.");

                ThrowIfPoolStarted();
                pool.SocketTimeout = value;
            }
        }
#endif

        /// <summary>
        /// Gets or sets the socket connection timeout in milliseconds.
        /// If less than or equal to zero, waits an indefinite amount of time.
        /// </summary>
        /// <value>
        /// The default value is 50 milliseconds (50).
        /// </value>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the property and the pool has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public int SocketConnectTimeout
        {
            get
            {
                ThrowIfPoolDisposed();
                return pool.SocketConnectTimeout;
            }
            set
            {
                ThrowIfPoolDisposed();
                ThrowIfPoolStarted();
                pool.SocketConnectTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to enable Nagle's algorithm flag for all TCP sockets created.
        /// </summary>
        /// <value>
        /// The default value is true.
        /// </value>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the property and the pool has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public bool EnableSocketNagleOption
        {
            get
            {
                ThrowIfPoolDisposed();
                return pool.Nagle;
            }
            set
            {
                ThrowIfPoolDisposed();
                ThrowIfPoolStarted();
                pool.Nagle = value;
            }
        }

        /// <summary>
        /// Gets or sets the hashing algorithm to use.
        /// </summary>
        /// <value>
        /// The default value is <see cref="global::Memcached.ClientLibrary.HashingAlgorithm.Native" />.
        /// </value>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the property and the pool has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public HashingAlgorithm HashingAlgorithm
        {
            get
            {
                ThrowIfPoolDisposed();
                return pool.HashingAlgorithm;
            }
            set
            {
                ThrowIfPoolDisposed();
                ThrowIfPoolStarted();
                pool.HashingAlgorithm = value;
            }
        }

        /// <summary>
        /// Initializes the pool and readies it for use.
        /// This method must be called before any <see cref="MemcachedCache" />
        /// referring to this pool is used.
        /// </summary>
        /// <remarks>
        /// No changes to the configuration of the pool can be made after
        /// this time unless the pool is stopped first.
        /// </remarks>
        /// <seealso cref="Stop"/>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public void Start()
        {
            ThrowIfPoolDisposed();
            pool.Initialize();
        }

        /// <summary>
        /// Shuts down the pool and releases its resources.
        /// </summary>
        /// <remarks>
        /// The pool can be started again later on if desired.
        /// </remarks>
        /// <seealso cref="Start"/>
        /// <exception cref="ObjectDisposedException">Thrown if the pool has been disposed</exception>
        public void Stop()
        {
            ThrowIfPoolDisposed();
            pool.Shutdown();
        }

        private void SetPool(SockIOPool pool, string poolName)
        {
            this.pool = pool;
            this.poolName = poolName;

            // Fixup our invariants to avoid mysterious failures.
            if (pool.Servers == null)
                pool.SetServers(EmptyArray<string>.Instance);
        }

        private void ThrowIfPoolStarted()
        {
            if (pool.Initialized)
                throw new InvalidOperationException("This operation cannot be performed once the pool has been started.");
        }

        private void ThrowIfPoolDisposed()
        {
            if (pool == null)
                throw new ObjectDisposedException("Pool has been disposed.");
        }

        #region API Workarounds
        /// <summary>
        /// Gets all pools known to the Memcached client library.
        /// This is a bad breach of encapsulation but we absolutely need to be
        /// able to dispose Memcached Pool objects to ensure that we can shut down
        /// and restart dependent components or tests reliably. 
        /// FIXME: File a bug for this in the library!
        /// </summary>
        /// <returns></returns>
        private static IDictionary GetSockIOPools()
        {
            FieldInfo field = typeof(SockIOPool).GetField("Pools", BindingFlags.Static | BindingFlags.NonPublic);
            return (IDictionary)field.GetValue(null);
        }

        private static object SockIOPoolsLock
        {
            get { return typeof(SockIOPool); }
        }

        private static string GetSockIOPoolName(SockIOPool pool)
        {
            lock (SockIOPoolsLock)
            {
                foreach (DictionaryEntry entry in GetSockIOPools())
                {
                    if (entry.Value == pool)
                        return (string)entry.Key;
                }
            }

            throw new ObjectDisposedException("The pool has been disposed.");
        }

        private static void DisposeSockIOPool(string poolName, SockIOPool pool)
        {
            lock (SockIOPoolsLock)
            {
                IDictionary pools = GetSockIOPools();
                if (pools[poolName] == pool)
                    GetSockIOPools().Remove(poolName);
            }
        }
        #endregion API Workarounds
    }
}
