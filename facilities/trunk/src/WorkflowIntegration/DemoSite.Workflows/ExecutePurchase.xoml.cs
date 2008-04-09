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

namespace DemoSite.Workflows
{
	public partial class ExecutePurchase : SequentialWorkflowActivity
	{
        public static DependencyProperty BuyerInfoProperty = DependencyProperty.Register("BuyerInfo", typeof(DemoSite.Workflows.Models.BuyerInfo), typeof(DemoSite.Workflows.ExecutePurchase));
        public static DependencyProperty PriceProperty = DependencyProperty.Register("Price", typeof(System.Decimal), typeof(DemoSite.Workflows.ExecutePurchase));
        public static DependencyProperty TrackingNumberProperty = DependencyProperty.Register("TrackingNumber", typeof(System.String), typeof(DemoSite.Workflows.ExecutePurchase));
        public static DependencyProperty DeclinedArgsProperty = DependencyProperty.Register("DeclinedArgs", typeof(DemoSite.Workflows.CreditCardEventArgs), typeof(DemoSite.Workflows.ExecutePurchase));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Parameters")]
        public DemoSite.Workflows.Models.BuyerInfo BuyerInfo
        {
            get
            {
                return ((DemoSite.Workflows.Models.BuyerInfo)(base.GetValue(DemoSite.Workflows.ExecutePurchase.BuyerInfoProperty)));
            }
            set
            {
                base.SetValue(DemoSite.Workflows.ExecutePurchase.BuyerInfoProperty, value);
            }
        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Parameters")]
        public decimal Price
        {
            get
            {
                return ((decimal)(base.GetValue(DemoSite.Workflows.ExecutePurchase.PriceProperty)));
            }
            set
            {
                base.SetValue(DemoSite.Workflows.ExecutePurchase.PriceProperty, value);
            }
        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Parameters")]
        public string TrackingNumber
        {
            get
            {
                return ((string)(base.GetValue(DemoSite.Workflows.ExecutePurchase.TrackingNumberProperty)));
            }
            set
            {
                base.SetValue(DemoSite.Workflows.ExecutePurchase.TrackingNumberProperty, value);
            }
        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Parameters")]
        public CreditCardEventArgs DeclinedArgs
        {
            get
            {
                return ((DemoSite.Workflows.CreditCardEventArgs)(base.GetValue(DemoSite.Workflows.ExecutePurchase.DeclinedArgsProperty)));
            }
            set
            {
                base.SetValue(DemoSite.Workflows.ExecutePurchase.DeclinedArgsProperty, value);
            }
        }

        private void handleDeclined_Invoked(object sender, ExternalDataEventArgs e)
        {
            throwDeclined.Fault = new ApplicationException(DeclinedArgs.FailedMessage);
            TrackData("FailedMessage", DeclinedArgs.FailedMessage);
        }

        private void trackArguments_ExecuteCode(object sender, EventArgs e)
        {
            TrackData("TrackingNumber", TrackingNumber);
            TrackData("BuyerInfo", BuyerInfo); 
            TrackData("Price", Price);
        }
    }
}
