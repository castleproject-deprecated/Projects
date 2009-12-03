namespace ExampleProject.Models
{
	using Castle.Components.Validator;
    using System;

	public class ContactInfo
	{
		private string name, email, message;

		[ValidateNonEmpty]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[ValidateNonEmpty, ValidateEmail]
		public string Email
		{
            get { return email; }
			set { email = value; }
		}

		[ValidateNonEmpty]
		public string Message
		{
			get { return message; }
			set { message = value; }
		}


	}
}
