using System;
using System.Data;
using System.Configuration;
using DemoSite.Workflows;
using System.Workflow.Runtime;
using System.Timers;
using System.Collections.Generic;

namespace DemoSite.Service
{
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
