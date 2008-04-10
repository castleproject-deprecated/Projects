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


namespace DemoSite.Service
{
    using System;
    using System.Timers;
    using System.Collections.Generic;
    
    using System.Workflow.Runtime;
    
    using DemoSite.Workflows;
    
    public class CreditCardProvider : ICreditCardProvider
    {
        private decimal buyingLimit;
        private Dictionary<string, CreditCardEventArgs> jobs = new Dictionary<string, CreditCardEventArgs>();


        public decimal BuyingLimit
        {
            get { return buyingLimit; }
            set { buyingLimit = value; }
        }

        public void PreApproveTransaction(DemoSite.Workflows.Models.BuyerInfo buyer, decimal price)
        {
            if (price > buyingLimit)
                throw new ArgumentOutOfRangeException("price", price, "Price above buying limit");


        }

        public void SendTransaction(DemoSite.Workflows.Models.BuyerInfo buyer, decimal price, string trackingNumber)
        {
            PreApproveTransaction(buyer, price);
            DateTime expires = new DateTime(buyer.ExpiresYear, buyer.ExpiresMonth, 1);
            if (expires <= DateTime.Today)
            {
                TransactionFailed(null,
                                  new CreditCardEventArgs(WorkflowEnvironment.WorkflowInstanceId, buyer, price,
                                                          trackingNumber, "Credit card is expired"));
                return;
            }

            jobs.Add(trackingNumber, new CreditCardEventArgs(WorkflowEnvironment.WorkflowInstanceId, buyer, price,
                                                               trackingNumber, ""));
        }

        public void CancelTransaction(string trackingNumber)
        {
            if (jobs.ContainsKey(trackingNumber))
                jobs.Remove(trackingNumber);
        }

        public void FireResponseMessage(string trackingNumber, bool success, string message)
        {
            CreditCardEventArgs args = jobs[trackingNumber];
            jobs.Remove(trackingNumber);

            if (success)
            {
                TransactionCompleted(null, args);
            }
            else
            {
                args.FailedMessage = message;
                TransactionFailed(null, args);
            }
        }

        public event EventHandler<CreditCardEventArgs> TransactionCompleted;

        public event EventHandler<CreditCardEventArgs> TransactionFailed;


    }
}
