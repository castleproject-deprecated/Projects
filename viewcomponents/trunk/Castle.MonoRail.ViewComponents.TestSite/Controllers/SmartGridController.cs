namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
	using Castle.MonoRail.Framework;
    using System.Collections.Generic;
	using System.Data;
	using System;
	[Layout("default")]
	public class SmartGridController : SmartDispatcherController
	{
		public void Index()
		{
			PropertyBag["users"] = User.FindAll();
		}

		public void IgnoringProperties()
		{
			PropertyBag["users"] = User.FindAll();
		}

		public void OverridingHeaderBehavior()
		{
			PropertyBag["users"] = User.FindAll();
		}

		public void ColumnsOrderring()
		{
			PropertyBag["users"] = User.FindAll();
		}

		public void Empty()
		{
			PropertyBag["users"] = new User[0];
		}


		public void More()
		{
			PropertyBag["users"] = User.FindAll();
		}

		public void StartEndCell()
		{
			PropertyBag["users"] = User.FindAll();
		}

		public void OverridingColumnBehavior()
		{
			PropertyBag["users"] = User.FindAll();
		}

		public void StartEndRow()
		{
			PropertyBag["users"] = User.FindAll();
		}


        public void SortBehavior()
        {
            PropertyBag["users"] = User.FindAll();
            PropertyBag["sortBy"] = "";
            PropertyBag["sortAsc"] = false;
        }

        public void SortBehavior(string sortBy, bool sortAsc)
        {
            SortedList<string, User> list = new SortedList<string, User>();
            IList<User> userList = new List<User>();
            foreach (User user in User.FindAll())
            {
                if (sortBy == "Email")
                    list.Add(user.Email, user);
                else
                    list.Add(user.Name, user);
            }

            if (!sortAsc)
            {
                for (int x = list.Count - 1; x >= 0; x--)
                {
                    userList.Add(list[list.Keys[x]]);
                }
            }
            else
            {
                foreach (KeyValuePair<string, User> pair in list)
                {
                    userList.Add(pair.Value);
                }
            }

            PropertyBag["users"] = userList;
            PropertyBag["sortBy"] = sortBy;
            PropertyBag["sortAsc"] = sortAsc;
        }

        public void SortJSBehavior()
        {
            PropertyBag["users"] = User.FindAll();
            PropertyBag["sortBy"] = "";
            PropertyBag["sortAsc"] = false;
        }

        [AccessibleThrough(Verb.Post)]
        public void SortJSBehavior(string sortBy, bool sortAsc)
        {
            SortedList<string, User> list = new SortedList<string, User>();
            IList<User> userList = new List<User>();
            foreach (User user in User.FindAll())
            {
                if (sortBy == "Email")
                    list.Add(user.Email, user);
                else
                    list.Add(user.Name, user);
            }

            if (!sortAsc)
            {
                for (int x = list.Count - 1; x >= 0; x--)
                {
                    userList.Add(list[list.Keys[x]]);
                }
            }
            else
            {
                foreach (KeyValuePair<string, User> pair in list)
                {
                    userList.Add(pair.Value);
                }
            }

            PropertyBag["users"] = userList;
            PropertyBag["sortBy"] = sortBy;
            PropertyBag["sortAsc"] = sortAsc;

            RenderView("SortJSBehavior.brailjs");
        }

        public void NullText() {
            PropertyBag["users"] = User.FindAll();
        }

		public void DataTable()
		{
			var dt = new DataTable();
			dt.Columns.Add("id", typeof(Guid));
			dt.Columns.Add("name", typeof(string));
			dt.Columns.Add("email", typeof(string));

			var r1 = dt.NewRow();
			r1.ItemArray = new object[] { new Guid(), "ayende", "Ayende@ayende.com" };
			var r2 = dt.NewRow();
			r2.ItemArray = new object[] { new Guid(), "foo", "foo@bar.com" };
			var r3 = dt.NewRow();
			r3.ItemArray = new object[] { new Guid(), "bar", "bar@foo.com" };

			dt.Rows.Add(r1);
			dt.Rows.Add(r2);
			dt.Rows.Add(r3);
			PropertyBag["users"] = dt;

		}
	}
}
