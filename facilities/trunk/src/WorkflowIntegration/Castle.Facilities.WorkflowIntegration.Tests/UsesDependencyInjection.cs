// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

    using Castle.Facilities.WorkflowIntegration.Tests.Services;
    using Castle.Facilities.WorkflowIntegration.Tests.Workflows;
    

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
