namespace ExampleProject.Controllers
{
	using System;
    using ExampleProject.Models;

	public class HomeController : BaseController
	{
		public void Index()
		{
			PropertyBag["AccessDate"] = DateTime.Now;
            var testData = new ContactInfo();
            testData.Email = "a@b.c";
            PropertyBag["testData"] = testData;
		}

		public void BlowItAway()
		{
			throw new Exception("Exception thrown from a MonoRail action");
		}
	}
}
