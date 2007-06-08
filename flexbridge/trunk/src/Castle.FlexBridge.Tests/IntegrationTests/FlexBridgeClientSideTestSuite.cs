using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests.IntegrationTests
{
    /// <summary>
    /// Runs the FlexBridge client-side unit tests using FlexUnit.
    /// Captures the cumulative results of the tests and reports them.
    /// </summary>
    [DependsOn(typeof(VerifyIntegrationTestPrerequisites))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class FlexBridgeClientSideTestSuite : BaseIntegrationTest
    {
        public FlexBridgeClientSideTestSuite()
        {
            HarnessMode = ConfigureWebResources.TestReportMode;
        }

        [Test]
        public void RunFlexBridgeUnitTests()
        {
            string report;
            for (;;)
            {
                report = (string)InvokeMethod("getTestReport");
                if (report != null)
                    break;

                Thread.Sleep(1000);
            }

            Console.WriteLine(report);
            Assert.IsTrue(report.Contains("OK"), "At least one test failed.");
        }
    }
}
