
namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
    using System;
    using System.Collections.Generic;
    using Castle.MonoRail.Framework;
    using Castle.MonoRail.ViewComponents;

    [Layout("default"), Rescue("generalerror")]
    public class MoreLessTextController: SmartDispatcherController
    {
        public void Index()
        {
            string text = "Congress shall make no law respecting an establishment of religion, or prohibiting the free exercise thereof; or abridging the freedom of speech, or of the press; or the right of the people peaceably to assemble, and to petition the Government for a redress of grievances.";
            Index(text, 100);
        }

        public void Index(string text, int maxLength)
        {
            PropertyBag["text"] = text;
            PropertyBag["maxlength"] = maxLength;
        }
    }
}
