
namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
	using System;
	using Castle.MonoRail.Framework;

	[Layout("default")]
	public class CompositeController : SmartDispatcherController
	{
		public void Index()
		{
		}

		public void Index(string searchCriteria)
		{
		}
	}
}
