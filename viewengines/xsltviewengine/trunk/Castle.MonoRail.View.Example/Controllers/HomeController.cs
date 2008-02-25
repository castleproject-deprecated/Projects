using System;
namespace Castle.MonoRail.View.Xslt.Example
{
	using Castle.MonoRail.Framework;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	namespace Controllers
	{

		[Layout("default"), Rescue("generalerror")]
		[Helper(typeof(TestHelper))]
		public class HomeController : SmartDispatcherController
		{
			public void Index()
			{
				PropertyBag["people"] = PersonStore.Persons;
			}

			public void AddPerson([DataBind("Person")]Person p)
			{
				PersonStore.AddPerson(p);
				RedirectToAction("Index");
			}
		}


	}
}