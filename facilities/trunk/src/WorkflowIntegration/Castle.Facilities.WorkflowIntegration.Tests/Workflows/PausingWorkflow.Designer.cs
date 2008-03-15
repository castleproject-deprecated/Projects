using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace Castle.Facilities.WorkflowIntegration.Tests.Workflows
{
	partial class PausingWorkflow
	{
		#region Designer generated code
		
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
		private void InitializeComponent()
		{
            this.CanModifyActivities = true;
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.WorkflowParameterBinding workflowparameterbinding1 = new System.Workflow.ComponentModel.WorkflowParameterBinding();
            System.Workflow.ComponentModel.WorkflowParameterBinding workflowparameterbinding2 = new System.Workflow.ComponentModel.WorkflowParameterBinding();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.WorkflowParameterBinding workflowparameterbinding3 = new System.Workflow.ComponentModel.WorkflowParameterBinding();
            this.makeFullName = new System.Workflow.Activities.CallExternalMethodActivity();
            this.waitForSurveyComplete = new System.Workflow.Activities.HandleExternalEventActivity();
            // 
            // makeFullName
            // 
            this.makeFullName.InterfaceType = typeof(Castle.Facilities.WorkflowIntegration.Tests.Services.ITestingExternalData);
            this.makeFullName.MethodName = "CreateFullName";
            this.makeFullName.Name = "makeFullName";
            activitybind1.Name = "PausingWorkflow";
            activitybind1.Path = "surveyCompleteArgs.Name";
            workflowparameterbinding1.ParameterName = "first";
            workflowparameterbinding1.SetBinding(System.Workflow.ComponentModel.WorkflowParameterBinding.ValueProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            workflowparameterbinding2.ParameterName = "last";
            workflowparameterbinding2.Value = "called";
            this.makeFullName.ParameterBindings.Add(workflowparameterbinding1);
            this.makeFullName.ParameterBindings.Add(workflowparameterbinding2);
            // 
            // waitForSurveyComplete
            // 
            this.waitForSurveyComplete.EventName = "SurveyComplete";
            this.waitForSurveyComplete.InterfaceType = typeof(Castle.Facilities.WorkflowIntegration.Tests.Services.ITestingExternalData);
            this.waitForSurveyComplete.Name = "waitForSurveyComplete";
            activitybind2.Name = "PausingWorkflow";
            activitybind2.Path = "surveyCompleteArgs";
            workflowparameterbinding3.ParameterName = "e";
            workflowparameterbinding3.SetBinding(System.Workflow.ComponentModel.WorkflowParameterBinding.ValueProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            this.waitForSurveyComplete.ParameterBindings.Add(workflowparameterbinding3);
            // 
            // PausingWorkflow
            // 
            this.Activities.Add(this.waitForSurveyComplete);
            this.Activities.Add(this.makeFullName);
            this.Name = "PausingWorkflow";
            this.CanModifyActivities = false;

		}

		#endregion

        private CallExternalMethodActivity makeFullName;
        private HandleExternalEventActivity waitForSurveyComplete;



    }
}
