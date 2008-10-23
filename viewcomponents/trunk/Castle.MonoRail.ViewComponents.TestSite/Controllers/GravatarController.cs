using System;
using System.Collections.Generic;
using System.Web;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Helpers;

namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
    [Layout("default")]
    public class GravatarController : SmartDispatcherController
    {
        public void Index()
        {
            PropertyBag["Gravatar"] = new GravatarHelper();
        }
    }
}
