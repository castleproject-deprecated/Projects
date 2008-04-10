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


namespace DemoSite.Workflows
{
    using System;
    using System.Workflow.Activities;

    using DemoSite.Workflows.Models;
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
