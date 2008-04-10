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

namespace DemoSite.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Workflow.Runtime;
    using System.Workflow.Runtime.Tracking;
    
    using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.Helpers;

    using DemoSite.Workflows;
    using DemoSite.Workflows.Models;


    public class HomeController : BaseController
    {
        private WorkflowRuntime runtime;
        private SqlTrackingQuery trackingQuery;
        private ICreditCardProvider creditCardProvider;

        public HomeController(
            WorkflowRuntime runtime, 
            SqlTrackingQuery trackingQuery,
            ICreditCardProvider creditCardProvider)
        {
            string t = typeof (SqlTrackingQuery).Assembly.FullName;
            this.runtime = runtime;
            this.trackingQuery = trackingQuery;
            this.creditCardProvider = creditCardProvider;
        }

        public void Index()
        {
            PropertyBag["buyertype"] = typeof (BuyerInfo);

            BuyerInfo defaultBuyer = new BuyerInfo();
            defaultBuyer.Name = "Louis DeJardin";
            defaultBuyer.Card = "abcdefg";
            defaultBuyer.ExpiresYear = 2018;
            defaultBuyer.ExpiresMonth = 12;

            PropertyBag["buyer"] = defaultBuyer;
            PropertyBag["price"] = 23.95;
        }

        public void BlueRain()
        {
        }

        public void Purchase([DataBind("buyer")] BuyerInfo buyer, decimal price)
        {
            creditCardProvider.PreApproveTransaction(buyer, price);

            string trackingNumber = string.Format("#{0:00000000}", new Random().Next(1000, 1000000));

            Dictionary<string, object> args = new Dictionary<string, object>();
            args["BuyerInfo"] = buyer;
            args["Price"] = price;
            args["TrackingNumber"] = trackingNumber;
            
            WorkflowInstance workflow = runtime.CreateWorkflow(typeof (ExecutePurchase), args);
            workflow.Start();

            RedirectToAction("Confirmed", "workflowId=" + workflow.InstanceId, "trackingNumber=" + trackingNumber);
        }

        public void Confirmed(Guid workflowId, string trackingNumber)
        {
            SqlTrackingWorkflowInstance tracking;
            if (!trackingQuery.TryGetWorkflow(workflowId, out tracking))
            {
                throw new ApplicationException("Workflow instance not found");
            }
            
            PropertyBag["tracking"] = tracking;

            foreach (UserTrackingRecord record in tracking.UserEvents)
            {              
                PropertyBag[record.UserDataKey] = record.UserData;
            }
        }

        [AjaxAction]
        [AccessibleThrough(Verb.Post)]
        [Layout("ajax")]
        public void FireSuccess(string trackingNumber)
        {
            creditCardProvider.FireResponseMessage(trackingNumber, true, null);
        }

        [AjaxAction]
        [AccessibleThrough(Verb.Post)]
        [Layout("ajax")]
        public void FireDeclined(string trackingNumber, string message)
        {
            creditCardProvider.FireResponseMessage(trackingNumber, false, message ?? "Purchase declined");
        }

        [AjaxAction]
        [AccessibleThrough(Verb.Post)]
        [Layout("ajax")]
        public void Confirmed_Left(Guid workflowId, string trackingNumber)
        {
            Confirmed(workflowId, trackingNumber);
        }

        [AjaxAction]
        [AccessibleThrough(Verb.Post)]
        [Layout("ajax")]
        public void Confirmed_Right(Guid workflowId, string trackingNumber)
        {
            Confirmed(workflowId, trackingNumber);
        }

        [AjaxAction]
        [AccessibleThrough(Verb.Post)]
        [Layout("ajax")]
        public void Confirmed_Status(Guid workflowId, string trackingNumber)
        {
            Confirmed(workflowId, trackingNumber);
        }
    }
}
