using System;

namespace Castle.MonoRail.View.Xslt.Example
{
	using Castle.MonoRail.Framework;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	public static class PersonStore
	{
		static PersonStore()
		{
			Persons = new List<Person>();
			AddPerson(new Person("Dick", "Tracy"));
			AddPerson(new Person("Mick", "Jagger"));
		}

		private static IList<Person> _Persons;
		public static IList<Person> Persons
		{
			get { return new ReadOnlyCollection<Person>(_Persons); }
			private set { _Persons = value; }
		}

		public static void AddPerson(Person p)
		{
			_Persons.Add(p);
		}
	}
}
