namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
	using Castle.MonoRail.Framework;

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
	}
}