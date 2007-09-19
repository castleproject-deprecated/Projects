using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Collections.Generic;
namespace TestSiteRest.Models
{
    public class Customer
    {
        public int ID { get; set; }
        public String Name { get; set; }

        private static List<Customer> __customers;

        static Customer()
        {
            Reset();
        }

        public static void Reset() {
               __customers = new List<Customer>() {
                new Customer() { ID = 1, Name = "Homer" },
                new Customer() { ID = 2, Name = "Bart" },
                new Customer() { ID = 3, Name = "Maggie"},
                new Customer() { ID = 4, Name = "Lisa"},
                new Customer() { ID = 5, Name = "Marge"}
            };
        }
        public static Customer FindById(int id)
        {
            return __customers.Where(c => c.ID == id).First();
        }

        public static Customer[] FindAll()
        {
            return __customers.ToArray();
        }

        public static void AddNew(Customer c)
        {
            if (c.ID != 0)
            {
                throw new ApplicationException("Customer should not have id assigned yet");
            }

            int maxID = __customers.Max(cust => cust.ID);
            c.ID = maxID + 1;
            __customers.Add(c);
        }

        public static void UpdateCustomer(Customer c)
        {
            Customer current = __customers.Where(t => t.ID == c.ID).First();
            if (current != null)
            {
                __customers.Remove(current);
                __customers.Add(c);
            }
            else
            {
                throw new ApplicationException("Customer " + c.ID + " does not exist");
            }

        }

        public static void Delete(int id)
        {
            __customers.RemoveAll(t => t.ID == id);
        }
    }
}
