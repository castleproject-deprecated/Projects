namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
	using Castle.MonoRail.Framework;

	[Layout("default")]
	public class CaptchaController : Controller
	{
		public void Index()
		{
		}

		[AccessibleThrough(Verb.Post)]
		public void TestValue()
		{
			 PropertyBag["IsCorrect"] =
				CaptchaComponent.IsValid(this)  ? "Correct" : "WRONG";
		}
	}
}