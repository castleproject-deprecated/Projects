using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using System.Workflow.Runtime;
using Castle.Facilities.WorkflowIntegration.Tests.Services;
using System.Workflow.Runtime.Tracking;
using System.Workflow.Activities;
using Castle.Facilities.WorkflowIntegration.Tests.Workflows;
using System.Threading;

namespace Castle.Facilities.WorkflowIntegration.Tests
{
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
