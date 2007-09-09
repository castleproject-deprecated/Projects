using System;
using System.Collections.Generic;
using Castle.MonoRail.Framework;

namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
	[Layout("default"), Rescue("generalerror")]
    public class CheckboxListController : SmartDispatcherController
	{
        private IList<Status> selectedStatuses = new List<Status>();
        private IList<Color> colors = new List<Color>();
        private IList<Color> selectedColors = new List<Color>();

		public void Index()
		{
            selectedStatuses.Add(Status.InProcess);
            selectedStatuses.Add(Status.Billed);

            PropertyBag["statuses"] = Enum.GetValues(typeof(Status));
            PropertyBag["selectedStatuses"] = selectedStatuses;

            colors.Add(new Color("Red", "ff0000"));
            colors.Add(new Color("Blue", "0000ff"));
            colors.Add(new Color("Green", "008000"));

            selectedColors.Add(colors[1]);

            PropertyBag["colors"] = colors;
            PropertyBag["selectedColors"] = selectedColors;
		}

        private enum Status
        {
            New,
            Open,
            InProcess,
            OnHold,
            Delivered,
            Billed,
            Closed,
            PascalCaseWithALLCAPSWord
        }

        public class Color
        {
            public Color(string name, string code)
            {
                this.name = name;
                this.code = code;
            }

            public string name;
            public string code;

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public string Code
            {
                get { return code; }
                set { code = value; }
            }
        }
	}
}
