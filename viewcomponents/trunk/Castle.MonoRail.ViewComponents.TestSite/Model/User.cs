namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
	using System;

	public class User
	{
		private Guid id;
		private string name;
		private string email;
		private User manager;

		public User Manager
		{
			get { return manager; }
			set { manager = value; }
		}

		public User(string name, string email)
		{
			id = Guid.NewGuid();
			this.name = name;
			this.email = email;
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

		public static User[] FindAll()
		{
			User[] users = new User[3];
			users[0] = new User("ayende", "Ayende@ayende.com");
			users[1] = new User("foo", "foo@bar.com");
			users[2] = new User("bar", "bar@foo.com");
			users[2].Manager = users[1];
			return users;
		}
	}
}