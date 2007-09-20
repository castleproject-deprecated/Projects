using System;
using System.Collections.Generic;
namespace TestSiteRest.Models
{
    public class Customer
    {
    	private int _id;
    	private String _name;

    	public Customer()
    	{
    	}

    	public Customer(int id, string name)
    	{
    		_id = id;
    		_name = name;
    	}

    	public int ID
    	{
    		get { return _id; }
    		set { _id = value; }
    	}

    	public string Name
    	{
    		get { return _name; }
    		set { _name = value; }
    	}

    	private static List<Customer> __customers;

        static Customer()
        {
            Reset();
        }

        public static void Reset() {
               __customers = new List<Customer>( new Customer[] {
                new Customer(1, "Homer"),
                new Customer(2, "Bart" ),
                new Customer(3, "Maggie"),
                new Customer(4, "Lisa"),
                new Customer(5, "Marge")
            });
        }
        public static Customer FindById(int id)
        {
        	return __customers.Find(delegate(Customer c) { return c.ID == id; });
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

            int maxID = __customers[__customers.Count-1].ID;
            c.ID = maxID + 1;
            __customers.Add(c);
        }

        public static void UpdateCustomer(Customer c)
        {
        	Customer current = __customers.Find(delegate(Customer t) { return t.ID == c.ID; });
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
			__customers.RemoveAll(delegate(Customer t) { return t.ID == id; });
        }
    }
}
