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
