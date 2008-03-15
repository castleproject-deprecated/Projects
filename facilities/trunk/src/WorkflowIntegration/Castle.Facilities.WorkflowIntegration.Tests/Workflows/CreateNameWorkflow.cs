using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace Castle.Facilities.WorkflowIntegration.Tests.Workflows
{
	public sealed partial class CreateNameWorkflow: SequentialWorkflowActivity
	{
		public CreateNameWorkflow()
		{
			InitializeComponent();
		}

        public static DependencyProperty FullNameProperty = DependencyProperty.Register("FullName", typeof(System.String), typeof(Castle.Facilities.WorkflowIntegration.Tests.Workflows.CreateNameWorkflow));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Parameters")]
        public String FullName
        {
            get
            {
                return ((string)(base.GetValue(Castle.Facilities.WorkflowIntegration.Tests.Workflows.CreateNameWorkflow.FullNameProperty)));
            }
            set
            {
                base.SetValue(Castle.Facilities.WorkflowIntegration.Tests.Workflows.CreateNameWorkflow.FullNameProperty, value);
            }
        }
	}

}
