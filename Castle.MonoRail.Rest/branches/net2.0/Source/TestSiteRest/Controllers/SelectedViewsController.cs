using Castle.MonoRail.Rest;
using Castle.MonoRail.Framework;

namespace TestSiteRest.Controllers
{
    [ControllerDetails(Area="V1")]
    public class SelectedViewsController : RestfulController
    {
        public void Index()
        {
        	RespondTo(delegate(ResponseFormat format) {
        	          	format.Xml(delegate(Responder xml) {
        	          	                   	xml.DefaultResponse();
        	          	                   });
        	          });
        	RespondTo(delegate(ResponseFormat format) {
        	          	format.Html(delegate(Responder html) {
        	          	                    	html.DefaultResponse();
        	          	                    });
        	          });
        }
    }
}
