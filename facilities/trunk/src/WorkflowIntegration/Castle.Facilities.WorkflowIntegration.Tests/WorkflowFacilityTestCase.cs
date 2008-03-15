// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WorkflowIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    using System.Workflow.Runtime;
    using System.Workflow.Runtime.Tracking;
    using System.Workflow.Activities;

    using Castle.Windsor;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Facilities.WorkflowIntegration.Tests.Services;
    using Castle.Facilities.WorkflowIntegration.Tests.Workflows;

    using NUnit.Framework;

    [TestFixture]
    public class WorkflowFacilityTestCase
    {
        WindsorContainer _container;

        [SetUp]
        public void Init()
        {
            _container = CreateConfiguredContainer();
        }

        WindsorContainer CreateConfiguredContainer()
        {
            WindsorContainer container = new WindsorContainer(new DefaultConfigurationStore());
            container.AddFacility("workflow.facility", new WorkflowFacility());
            return container;
        }


        [Test]
        public void FacilityPutsRuntimeInContainer()
        {
            WorkflowRuntime runtime = _container.Resolve<WorkflowRuntime>();
            Assert.IsNotNull(runtime, "WorkflowRuntime must be available from kernel");
        }

        [Test]
        public void RunningSimpleWorkflow()
        {
            WorkflowRuntime runtime = _container.Resolve<WorkflowRuntime>();

            ManualResetEvent finished = new ManualResetEvent(false);
            runtime.WorkflowCompleted += delegate(object sender, WorkflowCompletedEventArgs e) { finished.Set(); };
            runtime.WorkflowTerminated += delegate(object sender, WorkflowTerminatedEventArgs e) { finished.Set(); };

            WorkflowInstance workflow = runtime.CreateWorkflow(typeof(SimpleWorkflow));
            workflow.Start();
            bool isFinished = finished.WaitOne(TimeSpan.FromSeconds(1), false);

            Assert.IsTrue(isFinished, "Workflow must finish in less than a second");
        }


        [Test]
        public void WorkflowRuntimeServicesAreAdded()
        {
            _container.AddComponent("explodingtracking.service", typeof(ExplodingTrackingService));

            WorkflowRuntime runtime = _container.Resolve<WorkflowRuntime>();
            TrackingService instance = runtime.GetService<TrackingService>();

            Assert.IsInstanceOfType(typeof(ExplodingTrackingService), instance, "Type based off of WorkflowRuntimeService should be added to workflowruntime");
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ServicesAreUsedByWorkflows()
        {
            _container.AddComponent("explodingtracking.service", typeof(ExplodingTrackingService));

            WorkflowRuntime runtime = _container.Resolve<WorkflowRuntime>();

            ManualResetEvent finished = new ManualResetEvent(false);
            runtime.WorkflowCompleted += delegate(object sender, WorkflowCompletedEventArgs e) { finished.Set(); };
            runtime.WorkflowTerminated += delegate(object sender, WorkflowTerminatedEventArgs e) { finished.Set(); };

            WorkflowInstance workflow = runtime.CreateWorkflow(typeof(SimpleWorkflow));
            workflow.Start();

            bool isFinished = finished.WaitOne(TimeSpan.FromSeconds(1), false);
        }


        [Test]
        public void ExternalDataExchangeInterfacesAreAdded()
        {
            _container.AddComponent("testingexternaldata.service", typeof(TestingExternalData));

            WorkflowRuntime runtime = _container.Resolve<WorkflowRuntime>();
            ExternalDataExchangeService externalDataExchangeService = runtime.GetService<ExternalDataExchangeService>();
            Assert.IsNotNull(externalDataExchangeService, "A default external data exchange service must be added");

            object instance = externalDataExchangeService.GetService(typeof(ITestingExternalData));
            Assert.IsInstanceOfType(typeof(TestingExternalData), instance, "External data service must be of expected type");
        }

        [Test]
        public void CallingExternalDataExchangeFromWindsor()
        {
            _container.AddComponent("testingexternaldata.service", typeof(ITestingExternalData), typeof(TestingExternalData));

            WorkflowRuntime runtime = _container.Resolve<WorkflowRuntime>();

            ManualResetEvent finished = new ManualResetEvent(false);
            string fullName = null;
            runtime.WorkflowCompleted += delegate(object sender, WorkflowCompletedEventArgs e)
            {
                fullName = Convert.ToString(e.OutputParameters["FullName"]);
                finished.Set();
            };
            runtime.WorkflowTerminated += delegate(object sender, WorkflowTerminatedEventArgs e) { finished.Set(); };

            WorkflowInstance workflow = runtime.CreateWorkflow(typeof(CreateNameWorkflow));
            workflow.Start();
            bool isFinished = finished.WaitOne(TimeSpan.FromSeconds(1), false);
            Assert.IsTrue(isFinished, "Workflow must finish in less than a second");

            Assert.AreEqual("hello world", fullName, "Full name must be set with default values");

            TestingExternalData testingExternalData = (TestingExternalData)_container.Resolve<ITestingExternalData>();
            Assert.AreEqual("hello world", testingExternalData.MostRecentFullName, "Container must return the singleton the workflow used to call method");
        }

        [Test]
        public void RaisingEventFromExternalDataExchange()
        {
            _container.AddComponent("testingexternaldata.service", typeof(ITestingExternalData), typeof(TestingExternalData));

            WorkflowRuntime runtime = _container.Resolve<WorkflowRuntime>();

            ManualResetEvent finished = new ManualResetEvent(false);
            runtime.WorkflowCompleted += delegate(object sender, WorkflowCompletedEventArgs e) { finished.Set(); };
            runtime.WorkflowTerminated += delegate(object sender, WorkflowTerminatedEventArgs e) { finished.Set(); };

            WorkflowInstance workflow = runtime.CreateWorkflow(typeof(PausingWorkflow));
            workflow.Start();
            bool isFinished = finished.WaitOne(TimeSpan.FromSeconds(.25), false);
            Assert.IsFalse(isFinished, "Workflow must not be finished yet");

            TestingExternalData testingExternalData = (TestingExternalData)_container.Resolve<ITestingExternalData>();
            testingExternalData.OnSurveyComplete(workflow.InstanceId, "a test name");

            isFinished = finished.WaitOne(TimeSpan.FromSeconds(1), false);
            Assert.IsTrue(isFinished, "Workflow must finish in less than a second");

            Assert.AreEqual("a test name called", testingExternalData.MostRecentFullName, "Workflow was supposed to call back with the value the test provided");
        }
    }
}
