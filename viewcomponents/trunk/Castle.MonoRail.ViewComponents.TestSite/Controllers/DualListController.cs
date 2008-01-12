namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
	using System;
	using System.Collections.Generic;
	using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.ViewComponents;

	[Layout("default")]
    public class DualListController : SmartDispatcherController
    {
        public void Index()
        {
			PropertyBag["sourceColumnsType"] = this;
            PropertyBag["selectedColumns"] = new string[] { "PropertyBag", "Query", "Name" };

			PropertyBag["sourceColumnsType2"] = ParamStore.Form;
			PropertyBag["selectedColumns2"] = new string[] {ParamStore.Form.ToString()};

			string[] someColors = new string[]{"Red", "Green", "Blue"};
			string[] otherColors = new string[]{"Magenta", "Cyan", "Yellow"};

			PropertyBag["selectedColumns3"] = someColors;

			List<string> colors = new List<string>();
			colors.AddRange(someColors);
			colors.AddRange(otherColors);
			PropertyBag["sourceColumnsType3"] = colors;
		
		}
    }
}
