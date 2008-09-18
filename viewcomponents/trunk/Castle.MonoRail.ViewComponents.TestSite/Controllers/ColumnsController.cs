namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
	using System;
	using System.Collections.Generic;
	using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.ViewComponents;

	[Layout("default")]
    public class ColumnsController : Controller
    {
        string[] names = new string[]
        {
            "Al Jones",
            "Bob Smith",
            "Charles Johnson",
            "David Davidson",
            "Eddie Edwards",
            "Frank Franklin",
            "George Getty",
            "Harry Harrison",
            "Ian Smith",
            "James Curran",
            "Kenny Kilkenny",
            "Larry Leisuresuit",
            "Michael Michaels",
            "Norbert Jones",
            "Oliver Lawerence"
        };
        public void Index()
        {
            PropertyBag["names"] = names;
        }
    }
}
