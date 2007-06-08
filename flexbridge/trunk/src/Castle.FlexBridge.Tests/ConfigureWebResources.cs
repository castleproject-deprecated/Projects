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
using System.Globalization;
using System.IO;
using System.Text;
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests
{
    /// <summary>
    /// Performs SetUp and TearDown activities on behalf of the test
    /// assembly before tests run.  This is used to ensure that the
    /// WebResources used by the test are available.
    /// </summary>
    public static class ConfigureWebResources
    {
        private const int PortNumber = 3700;

        private static Cassini.Server cassiniServer;

        public const string TestRunnerMode = "testRunner";
        public const string TestReportMode = "testReport";
        public const string IntegrationTestHarnessMode = "integrationTestHarness";

        /// <summary>
        /// Gets the WebResources root Url.
        /// </summary>
        public static string WebResourcesRootUrl
        {
            get { return "http://localhost:" + PortNumber + "/"; }
        }

        /// <summary>
        /// Gets the Url of the FlexBridge flash-based test harness for the
        /// specified mode.
        /// </summary>
        /// <param name="mode">The mode for the test harness</param>
        public static string GetHarnessUrl(string mode)
        {
            return WebResourcesRootUrl + "Flash/FlexBridgeTests.html?mode=" + mode;
            //get { return "http://localhost.:3700/Harness/FlexBridgeTests-debug.html?debug=true"; }
        }

        [SetUp]
        public static void SetUp()
        {
            string testsBin = Path.GetDirectoryName(typeof(ConfigureWebResources).Assembly.Location);
            string webResourcesPath = Path.GetDirectoryName(Path.GetFullPath(testsBin));

            if (File.Exists(Path.Combine(webResourcesPath, "Web.config")))
            {
                Console.WriteLine("Starting Cassini on port {0} to host web resources in '{1}'.",
                    PortNumber, webResourcesPath);

                cassiniServer = new Cassini.Server(PortNumber, "/", webResourcesPath);
                cassiniServer.Start();
            }
            else
            {
                Console.WriteLine("Warning: Expected to find Web.config in '{0}' "
                    + "based on the location of the tests assembly but it was not there.\n"
                    + "Some tests may fail unless those resources are loaded in a web server at port {1}",
                    webResourcesPath, PortNumber);
            }
        }

        [TearDown]
        public static void TearDown()
        {
            if (cassiniServer != null)
            {
                cassiniServer.Stop();
                cassiniServer = null;
            }
        }
    }
}
