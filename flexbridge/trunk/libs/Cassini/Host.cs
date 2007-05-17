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
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Hosting;
    using System.Security.Permissions;
    using System.Security.Principal;

    internal class Host : MarshalByRefObject, IRegisteredObject {
        private Server _server;

        private int _port;
        private volatile int _pendingCallsCount;
        private string _virtualPath;
        private string _lowerCasedVirtualPath;
        private string _lowerCasedVirtualPathWithTrailingSlash;
        private string _physicalPath;
        private string _installPath;
        private string _physicalClientScriptPath;
        private string _lowerCasedClientScriptPathWithTrailingSlash;

        public override object InitializeLifetimeService() {
            // never expire the license
            return null;
        }

        public Host() {
            HostingEnvironment.RegisterObject(this);
        }

        public void Configure(Server server, int port, string virtualPath, string physicalPath) {
            _server = server;

            _port = port;
            _installPath = null;
            _virtualPath = virtualPath;

            _lowerCasedVirtualPath = CultureInfo.InvariantCulture.TextInfo.ToLower(_virtualPath);
            _lowerCasedVirtualPathWithTrailingSlash = virtualPath.EndsWith("/", StringComparison.Ordinal) ? virtualPath : virtualPath + "/";
            _lowerCasedVirtualPathWithTrailingSlash = CultureInfo.InvariantCulture.TextInfo.ToLower(_lowerCasedVirtualPathWithTrailingSlash);
            _physicalPath = physicalPath;
            _physicalClientScriptPath = HttpRuntime.AspClientScriptPhysicalPath + "\\";
            _lowerCasedClientScriptPathWithTrailingSlash = CultureInfo.InvariantCulture.TextInfo.ToLower(HttpRuntime.AspClientScriptVirtualPath + "/");
        }

        public void ProcessRequest(Connection conn) {
            // Add a pending call to make sure our thread doesn't get killed
            AddPendingCall();

            try {
                Request request = new Request(_server, this, conn);
                request.Process();
            }
            finally {
                RemovePendingCall();
            }
        }

        private void WaitForPendingCallsToFinish() {
            for (; ; ) {
                if (_pendingCallsCount <= 0)
                    break;

                Thread.Sleep(250);
            }
        }

        private void AddPendingCall() {
#pragma warning disable 0420
            Interlocked.Increment(ref _pendingCallsCount);
#pragma warning restore 0420
        }

        private void RemovePendingCall() {
#pragma warning disable 0420
            Interlocked.Decrement(ref _pendingCallsCount);
#pragma warning restore 0420
        }

        public void Shutdown() {
            HostingEnvironment.InitiateShutdown();
        }

        void IRegisteredObject.Stop(bool immediate) {
            // Unhook the Host so Server will process the requests in the new appdomain.
            if (_server != null) {
                _server.HostStopped();
            }

            // Make sure all the pending calls complete before this Object is unregistered.
            WaitForPendingCallsToFinish();

            HostingEnvironment.UnregisterObject(this);
        }

        public string InstallPath {
            get {
                return _installPath;
            }
        }

        public string NormalizedClientScriptPath {
            get {
                return _lowerCasedClientScriptPathWithTrailingSlash;
            }
        }

        public string NormalizedVirtualPath {
            get {
                return _lowerCasedVirtualPathWithTrailingSlash;
            }
        }

        public string PhysicalClientScriptPath {
            get {
                return _physicalClientScriptPath;
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

        public string VirtualPath {
            get {
                return _virtualPath;
            }
        }

        public bool IsVirtualPathInApp(String path) {
            bool isClientScriptPath;
            return IsVirtualPathInApp(path, out isClientScriptPath);
        }

        public bool IsVirtualPathInApp(String path, out bool isClientScriptPath) {
            isClientScriptPath = false;

            if (path == null) {
                return false;
            }

            if (_virtualPath == "/" && path.StartsWith("/", StringComparison.Ordinal)) {
                if (path.StartsWith(_lowerCasedClientScriptPathWithTrailingSlash, StringComparison.Ordinal))
                    isClientScriptPath = true;
                return true;
            }

            path = CultureInfo.InvariantCulture.TextInfo.ToLower(path);

            if (path.StartsWith(_lowerCasedVirtualPathWithTrailingSlash, StringComparison.Ordinal)) {
                return true;
            }

            if (path == _lowerCasedVirtualPath) {
                return true;
            }

            if (path.StartsWith(_lowerCasedClientScriptPathWithTrailingSlash, StringComparison.Ordinal)) {
                isClientScriptPath = true;
                return true;
            }

            return false;
        }

        public bool IsVirtualPathAppPath(String path) {
            if (path == null) {
                return false;
            }

            path = CultureInfo.InvariantCulture.TextInfo.ToLower(path);
            return (path == _lowerCasedVirtualPath || path == _lowerCasedVirtualPathWithTrailingSlash);
        }
    }
}
