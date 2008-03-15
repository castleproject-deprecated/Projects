using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Runtime;
using Castle.Facilities.WorkflowIntegration.Tests.Services;
using Castle.Facilities.WorkflowIntegration.Tests.Workflows;
using System.Threading;
using System.Workflow.Runtime.Hosting;

namespace Castle.Facilities.WorkflowIntegration.Tests
{
	public class UsesDependencyInjection
	{
        public UsesDependencyInjection(WorkflowRuntime runtime)
        {
            _runtime = runtime;
        }

        WorkflowRuntime _runtime;

        ITestingExternalData _testingExternalData;
        public ITestingExternalData TestingExternalData
        {
            get { return _testingExternalData; }
            set { _testingExternalData = value; }
        }

        ManualWorkflowSchedulerService _scheduler;
        public ManualWorkflowSchedulerService Scheduler
        {
            get { return _scheduler; }
            set { _scheduler = value; }
        }

        public string RunCreateNameWorkflow()
        {
            WorkflowInstance instance = _runtime.CreateWorkflow(typeof(CreateNameWorkflow));
            instance.Start();
            _scheduler.RunWorkflow(instance.InstanceId);
            return _testingExternalData.MostRecentFullName;
        }
	}
}
