using System;
using Castle.MonoRail.Framework;

namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
    [Layout("default")]
    public class GridController : SmartDispatcherController
    {
        public void Index()
        {
            PropertyBag["users"] = User.FindAll();
        }

        public void Alternate()
        {
            PropertyBag["users"] = User.FindAll();
        }

        public void Empty()
        {
            PropertyBag["users"] = new User[0];
        }

        public void OverrideEmpty()
        {
            PropertyBag["users"] = new User[0];
        }
    }
}