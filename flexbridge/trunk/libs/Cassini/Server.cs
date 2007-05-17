/*=======================================================================
  Copyright (C) Microsoft Corporation.  All rights reserved.
 
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
=======================================================================*/

namespace Cassini {
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Threading;
    using System.Web;
    using System.Web.Hosting;

    public sealed class Server : MarshalByRefObject {
        private int _port;
        private string _virtualPath;
        private string _physicalPath;

        private WaitCallback _onStart;
        private WaitCallback _onSocketAccept;

        private bool _shutdownInProgress;

        private ApplicationManager _appManager;

        private Socket _socket;
        private Host _host;

        public Server(int port, string virtualPath, string physicalPath) {
            _port = port;
            _virtualPath = virtualPath;
            _physicalPath = physicalPath.EndsWith("\\", StringComparison.Ordinal) ? physicalPath : physicalPath + "\\";

            _onSocketAccept = new WaitCallback(OnSocketAccept);
            _onStart = new WaitCallback(OnStart);

            _appManager = ApplicationManager.GetApplicationManager();
        }

        public override object InitializeLifetimeService() {
            // never expire the license
            return null;
        }

        public string VirtualPath {
            get {
                return _virtualPath;
            }
        }

        public string PhysicalPath {
            get {
                return _physicalPath;
            }
        }

        public int Port {
            get {
                return _port;
            }
        }

        public string RootUrl {
            get {
                if (_port != 80) {
                    return "http://localhost:" + _port + _virtualPath;
                }
                else {
                    return "http://localhost" + _virtualPath;
                }
            }
        }

        //
        // Socket listening
        // 

        public void Start() {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.ExclusiveAddressUse = true;

            try {
                _socket.Bind(new IPEndPoint(IPAddress.Loopback /*IPAddress.Any*/, _port));
            }
            catch {
                _socket.ExclusiveAddressUse = false;
                _socket.Bind(new IPEndPoint(IPAddress.Loopback /*IPAddress.Any*/, _port));
            }

            _socket.Listen((int)SocketOptionName.MaxConnections);

            ThreadPool.QueueUserWorkItem(_onStart);
        }

        public void Stop() {
            _shutdownInProgress = true;

            try {
                if (_socket != null) {
                    _socket.Close();
                }
            }
            catch {
            }
            finally {
                _socket = null;
            }

            try {
                if (_host != null) {
                    _host.Shutdown();
                }

                while (_host != null) {
                    Thread.Sleep(100);
                }
            }
            catch {
            }
            finally {
                _host = null;
            }
        }

        private void OnSocketAccept(object acceptedSocket) {
            if (!_shutdownInProgress) {
                Connection conn =  new Connection(this, (Socket)acceptedSocket);

                // wait for at least some input
                if (conn.WaitForRequestBytes() == 0) {
                    conn.WriteErrorAndClose(400);
                    return;
                }

                // find or create host
                Host host = GetHost();
                if (host == null) {
                    conn.WriteErrorAndClose(500);
                    return;
                }

                // process request in worker app domain
                host.ProcessRequest(conn);
            }
        }

        // called at the end of request processing
        // to disconnect the remoting proxy for Connection object
        // and allow GC to pick it up
        internal void OnRequestEnd(Connection conn) {
            RemotingServices.Disconnect(conn);
        }

        private void OnStart(Object unused) {
            while (!_shutdownInProgress) {
                try {
                    Socket socket = _socket.Accept();
                    ThreadPool.QueueUserWorkItem(_onSocketAccept, socket);
                }
                catch {
                    Thread.Sleep(100);
                }
            }
        }

        private Host GetHost() {
            if (_shutdownInProgress)
                return null;

            Host host = _host;

            if (host == null) {
                lock (this) {
                    host = _host;
                    if (host == null) {
                        // Put together some unique app id based on the virtual and physical paths
                        string uniqueAppString = String.Concat(_virtualPath, _physicalPath).ToLowerInvariant();
                        string appId = (uniqueAppString.GetHashCode()).ToString("x", CultureInfo.InvariantCulture);
                        _host = (Host)_appManager.CreateObject(appId, typeof(Host), _virtualPath, _physicalPath, false /*failIfExists*/);
                        _host.Configure(this, _port, _virtualPath, _physicalPath);
                        host = _host;
                    }
                }
            }

            return host;
        }

        internal void HostStopped() {
            _host = null;
        }
    }
}
