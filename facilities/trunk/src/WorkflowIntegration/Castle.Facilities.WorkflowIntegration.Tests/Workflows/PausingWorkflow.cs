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
	public sealed partial class PausingWorkflow: SequentialWorkflowActivity
	{
		public PausingWorkflow()
		{
			InitializeComponent();
		}

        public static DependencyProperty surveyCompleteArgsProperty = DependencyProperty.Register("surveyCompleteArgs", typeof(Castle.Facilities.WorkflowIntegration.Tests.Services.SurveyEventArgs), typeof(Castle.Facilities.WorkflowIntegration.Tests.Workflows.PausingWorkflow));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Parameters")]
        public Castle.Facilities.WorkflowIntegration.Tests.Services.SurveyEventArgs surveyCompleteArgs
        {
            get
            {
                return ((Castle.Facilities.WorkflowIntegration.Tests.Services.SurveyEventArgs)(base.GetValue(Castle.Facilities.WorkflowIntegration.Tests.Workflows.PausingWorkflow.surveyCompleteArgsProperty)));
            }
            set
            {
                base.SetValue(Castle.Facilities.WorkflowIntegration.Tests.Workflows.PausingWorkflow.surveyCompleteArgsProperty, value);
            }
        }
	}

}
