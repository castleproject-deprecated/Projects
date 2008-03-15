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
	partial class CreateNameWorkflow
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
            System.Workflow.ComponentModel.WorkflowParameterBinding workflowparameterbinding1 = new System.Workflow.ComponentModel.WorkflowParameterBinding();
            System.Workflow.ComponentModel.WorkflowParameterBinding workflowparameterbinding2 = new System.Workflow.ComponentModel.WorkflowParameterBinding();
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.WorkflowParameterBinding workflowparameterbinding3 = new System.Workflow.ComponentModel.WorkflowParameterBinding();
            this.callCreateFullName = new System.Workflow.Activities.CallExternalMethodActivity();
            // 
            // callCreateFullName
            // 
            this.callCreateFullName.InterfaceType = typeof(Castle.Facilities.WorkflowIntegration.Tests.Services.ITestingExternalData);
            this.callCreateFullName.MethodName = "CreateFullName";
            this.callCreateFullName.Name = "callCreateFullName";
            workflowparameterbinding1.ParameterName = "first";
            workflowparameterbinding1.Value = "hello";
            workflowparameterbinding2.ParameterName = "last";
            workflowparameterbinding2.Value = "world";
            activitybind1.Name = "CreateNameWorkflow";
            activitybind1.Path = "FullName";
            workflowparameterbinding3.ParameterName = "(ReturnValue)";
            workflowparameterbinding3.SetBinding(System.Workflow.ComponentModel.WorkflowParameterBinding.ValueProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            this.callCreateFullName.ParameterBindings.Add(workflowparameterbinding1);
            this.callCreateFullName.ParameterBindings.Add(workflowparameterbinding2);
            this.callCreateFullName.ParameterBindings.Add(workflowparameterbinding3);
            // 
            // CreateNameWorkflow
            // 
            this.Activities.Add(this.callCreateFullName);
            this.Name = "CreateNameWorkflow";
            this.CanModifyActivities = false;

		}

		#endregion

        private CallExternalMethodActivity callCreateFullName;


    }
}
