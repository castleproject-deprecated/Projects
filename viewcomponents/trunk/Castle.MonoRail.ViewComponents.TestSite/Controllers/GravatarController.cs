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
			PropertyBag["injection"] = @"blah:""><script>alert(""This is an (failed) Html injection attempt"");</script><p id=""x ";
        }
    }
}
