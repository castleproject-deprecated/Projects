using System;
using AspViewTestSite.Interfaces.UsingDictionaryAdapter;
using Castle.MonoRail.Framework;

namespace AspViewTestSite.Controllers.UsingDictionaryAdapter
{
	[ControllerDetails(Area = "UsingDictionaryAdapter")]
	public class FlashAndDefaultsController : Controller<IStupidView>
	{
		public void Index()
		{
		}
		public void DoStuff(string name, string password)
		{
			if (password != "AspView Rocks")
			{
				TypedFlash.Name = name;
				TypedFlash.Message = "Wrong Password";
				RedirectToAction("Index"); 
				return;
			}
			TypedPropertyBag.Id = Guid.NewGuid();
			TypedPropertyBag.Name = name;
		}
	}
}