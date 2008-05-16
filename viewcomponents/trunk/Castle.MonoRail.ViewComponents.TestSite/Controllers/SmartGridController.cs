namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
	using Castle.MonoRail.Framework;
    using System.Collections.Generic;
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
	}
}
