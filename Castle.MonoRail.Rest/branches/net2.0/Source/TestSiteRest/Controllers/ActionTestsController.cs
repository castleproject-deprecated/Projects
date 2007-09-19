using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Castle.MonoRail.Rest;
using Castle.MonoRail.Framework;

namespace TestSiteRest.Controllers
{
    [ControllerDetails(Area="V1")]
    public class ActionTestsController : RestfulController
    {
        public void Index()
        {
            RenderText("Index");
        }

        public void Create()
        {
            RenderText("Create");
        }
        public void Show()
        {
            RenderText("Show");
        }
        public void Update()
        {
            RenderText("Update");
        }
        public void Destroy()
        {
            RenderText("Destroy");
        }
    }
}
