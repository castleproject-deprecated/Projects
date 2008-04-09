using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSite.Workflows.Models
{
    [Serializable]
	public class BuyerInfo
	{
        private string name;

        private string card;

        private int expiresYear;

        private int expiresMonth;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Card
        {
            get { return card; }
            set { card = value; }
        }

        public int ExpiresYear
        {
            get { return expiresYear; }
            set { expiresYear = value; }
        }

        public int ExpiresMonth
        {
            get { return expiresMonth; }
            set { expiresMonth = value; }
        }
	}
}
