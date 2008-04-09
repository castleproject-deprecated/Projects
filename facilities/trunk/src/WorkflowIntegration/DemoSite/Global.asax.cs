using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace DemoSite
{
    public class Global : System.Web.HttpApplication, IContainerAccessor
    {
        private static IWindsorContainer container;

        protected void Application_Start(object sender, EventArgs e)
        {
            container = new WindsorContainer(new XmlInterpreter());
        }

        protected void Application_End(object sender, EventArgs e)
        {
            container.Dispose();
        }

        #region IContainerAccessor Members

        public IWindsorContainer Container
        {
            get { return container; }
        }

        #endregion
    }
}