using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using System.Workflow.Runtime;
using System.Threading;
using Castle.Facilities.WorkflowIntegration.Tests.Workflows;
using Castle.Facilities.WorkflowIntegration.Tests.Services;
using System.Workflow.Runtime.Hosting;

namespace Castle.Facilities.WorkflowIntegration.Tests
{
    [TestFixture]
	public class CastleConfigurationTestCase
    {
        WindsorContainer _container;

        [SetUp]
        public void Init()
        {
            _container = CreateConfiguredContainer();
        }

        WindsorContainer CreateConfiguredContainer()
        {
            WindsorContainer container = new WindsorContainer(new XmlInterpreter());

            return container;
        }

        [Test]
        public void ComponentsProvidedByConfiguration()
        {
            WorkflowRuntime runtime = _container.Resolve<WorkflowRuntime>();
            WorkflowInstance instance = runtime.CreateWorkflow(typeof(CreateNameWorkflow));
            instance.Start();

            ManualWorkflowSchedulerService scheduler = _container.Resolve<ManualWorkflowSchedulerService>();
            scheduler.RunWorkflow(instance.InstanceId);
            
            TestingExternalData testingExternalData = (TestingExternalData)_container.Resolve<ITestingExternalData>();
            Assert.AreEqual("You are \"{0} {1}\".", testingExternalData.FullNameFormat, "Format must have been provided by config");
            Assert.AreEqual("You are \"hello world\".", testingExternalData.MostRecentFullName, "Full name must have been set by workflow execution");
        }

        [Test]
        public void DependencyInjectionProvidesRuntimeAndServices()
        {
            UsesDependencyInjection worker = _container.Resolve<UsesDependencyInjection>();
            string fullName = worker.RunCreateNameWorkflow();
            Assert.AreEqual("You are \"hello world\".", fullName, "Full name must have been set by workflow execution");
        }
	}
}
