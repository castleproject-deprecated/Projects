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
	partial class SimpleWorkflow
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
            this.veryShortdelay = new System.Workflow.Activities.DelayActivity();
            // 
            // veryShortdelay
            // 
            this.veryShortdelay.Name = "veryShortdelay";
            this.veryShortdelay.TimeoutDuration = System.TimeSpan.Parse("00:00:00.0500000");
            // 
            // SimpleWorkflow
            // 
            this.Activities.Add(this.veryShortdelay);
            this.Name = "SimpleWorkflow";
            this.CanModifyActivities = false;

		}

		#endregion

        private DelayActivity veryShortdelay;
	}
}
