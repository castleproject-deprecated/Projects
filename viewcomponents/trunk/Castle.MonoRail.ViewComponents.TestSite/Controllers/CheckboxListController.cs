using System;
using System.Collections.Generic;
using Castle.MonoRail.Framework;

namespace Lunaverse.Tools.Monorail.Samples.Controllers
{
	[Layout("default"), Rescue("generalerror")]
    public class CheckboxListController : SmartDispatcherController
	{
        private IList<Status> statuses = new List<Status>();

		public void Index()
		{
            statuses.Add(Status.InProcess);
            statuses.Add(Status.Billed);

            PropertyBag["statuses"] = Enum.GetValues(typeof(Status));
            PropertyBag["selectedStatuses"] = statuses;
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
	}
}
