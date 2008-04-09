using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Activities;
using DemoSite.Workflows.Models;

namespace DemoSite.Workflows
{
    [ExternalDataExchange]
	public interface ICreditCardProvider
	{
        void PreApproveTransaction(BuyerInfo buyer, decimal price);
        void SendTransaction(BuyerInfo buyer, decimal price, string trackingNumber);
        void CancelTransaction(string trackingNumber);

        void FireResponseMessage(string trackingNumber, bool success, string message);

        event EventHandler<CreditCardEventArgs> TransactionCompleted;
        event EventHandler<CreditCardEventArgs> TransactionFailed;
	}


    [Serializable]
    public class CreditCardEventArgs : ExternalDataEventArgs
    {
        private BuyerInfo buyer;
        private decimal price;
        private string trackingNumber;
        private string failedMessage;

        public CreditCardEventArgs(Guid instanceId, BuyerInfo buyer, decimal price, string trackingNumber)
            : base(instanceId)
        {
            this.Buyer = buyer;
            this.Price = price;
            this.TrackingNumber = trackingNumber;
        }

        public CreditCardEventArgs(Guid instanceId, BuyerInfo buyer, decimal price, string trackingNumber, string failedMessage)
            : base(instanceId)
        {
            this.Buyer = buyer;
            this.Price = price;
            this.TrackingNumber = trackingNumber;
            this.FailedMessage = failedMessage;
        }

        public BuyerInfo Buyer
        {
            get { return buyer; }
            set { buyer = value; }
        }

        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }

        public string TrackingNumber
        {
            get { return trackingNumber; }
            set { trackingNumber = value; }
        }

        public string FailedMessage
        {
            get { return failedMessage; }
            set { failedMessage = value; }
        }
    }



}
