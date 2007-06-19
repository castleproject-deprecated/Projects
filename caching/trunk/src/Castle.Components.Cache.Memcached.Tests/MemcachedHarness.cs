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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using MbUnit.Framework;

namespace Castle.Components.Cache.Memcached.Tests
{
    /// <summary>
    /// Starts a local Memcached server for testing purposes.
    /// </summary>
    public static class MemcachedHarness
    {
        private static Process serverProcess;

        [SetUp]
        public static void SetUp()
        {
            string path = Path.GetDirectoryName(typeof(MemcachedHarness).Assembly.Location);
            string serverExe = Path.Combine(path, "memcached.exe");

            Console.WriteLine("Starting Memcache server from: {0}", serverExe);

            ProcessStartInfo startInfo = new ProcessStartInfo(serverExe, "-l 127.0.0.1");
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;
            startInfo.UseShellExecute = false;

            try
            {
                serverProcess = Process.Start(startInfo);
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Was unable to start the Memcached Server.  Some tests may fail!");
                Console.WriteLine(ex);
            }
        }

        [TearDown]
        public static void TearDown()
        {
            if (serverProcess != null)
            {
                serverProcess.CloseMainWindow();
                serverProcess.WaitForExit(2000);

                if (!serverProcess.HasExited)
                    serverProcess.Kill();

                serverProcess = null;
            }
        }
    }
}
