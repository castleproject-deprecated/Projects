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
    using System.Workflow.Runtime.Hosting;

    using Castle.Facilities.WorkflowIntegration.Tests.Workflows;
    using Castle.Facilities.WorkflowIntegration.Tests.Services;
    using Castle.Windsor;
    using Castle.Windsor.Configuration.Interpreters;

    using NUnit.Framework;

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
