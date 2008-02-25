using System;

namespace Castle.MonoRail.View.Xslt.Example
{
	using Castle.MonoRail.Framework;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	public class Person
	{

		private string _FirstName;
		public string FirstName
		{
			get { return _FirstName; }
			set
			{
				_FirstName = value;
			}
		}

		private string _LastName;
		public string LastName
		{
			get { return _LastName; }
			set
			{
				_LastName = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the Person class.
		/// </summary>
		/// <param name="firstName"></param>
		/// <param name="lastName"></param>
		public Person(string firstName, string lastName)
		{
			_FirstName = firstName;
			_LastName = lastName;
		}

		/// <summary>
		/// Initializes a new instance of the Person class.
		/// </summary>
		public Person()
		{
		}
	}
}
