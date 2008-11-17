namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
	using System;
	using System.Collections.Generic;

	public class User
	{
		private Guid id;
		private string name;
		private string email;
		private User manager;

		public User(string name, string email)
		{
			id = Guid.NewGuid();
			this.name = name;
			this.email = email;
		}

		public User Manager
		{
			get { return manager; }
			set { manager = value; }
		}

		public List<User> Underlings   // Skipped, ICollection<T>
		{
			get { return new List<User>(); }
			set { }
		}

		public User[] Peers				// Skipped, ICollection
		{
			get { return new User[0]; }
		}

		// Will display random values, sometime true, sometime false, sometimes NULL.
		// Shows the SmartGrid will display generic types except ICollection<T>s
		public bool? Married			
		{
			get
			{
				switch (GetHashCode() % 4)
				{
					case 0:
						return true;
					case 1:
						return false;
					default:
						return null;
				}
			}
		}

		public Guid Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		// Uncomment, and SmartGrid will start displaying the Manager field 
		// (when commented, User class will inherit Object.ToString() and SmartGrid will therefore reject it)
//		public override string ToString()	{ return '['+Name.ToUpper()+']'; }

		public static User[] FindAll()
		{
			User[] users = new User[3];
			users[0] = new User("ayende", "Ayende@ayende.com");
			users[1] = new User("foo", "foo@bar.com");
			users[2] = new User("bar", "bar@foo.com");
			users[2].Manager = users[1];
			users[1].Underlings = new List<User> (new User[] { users[0], users[2] });
			return users;
		}
	}
}