namespace ExampleProject.Controllers
{
	using Castle.MonoRail.Framework;

	[Rescue("generalerror")]
	public abstract class BaseController : SmartDispatcherController
	{
	}
}
