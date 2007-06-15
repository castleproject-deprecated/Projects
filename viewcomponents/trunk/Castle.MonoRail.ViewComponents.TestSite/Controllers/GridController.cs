namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
	using Castle.MonoRail.Framework;

	[Layout("default")]
	public class GridController : SmartDispatcherController
	{
		public void Index()
		{
			PropertyBag["users"] = User.FindAll();
		}

		public void Alternate()
		{
			PropertyBag["users"] = User.FindAll();
		}

		public void Empty()
		{
			PropertyBag["users"] = new User[0];
		}

		public void OverrideEmpty()
		{
			PropertyBag["users"] = new User[0];
		}
	}
}