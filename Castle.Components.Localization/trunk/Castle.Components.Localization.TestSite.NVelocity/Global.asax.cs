
namespace Castle.Components.Localization.TestSite
{
	#region Using Directives

	using System;
	using System.Collections;
	using System.Configuration;
	using System.Globalization;
	using System.Threading;
	using System.Web;

	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;
	using Castle.Core.Resource;
	//using Castle.Components.Localization.MonoRail.Framework;

	#endregion Using Directives

	public class GlobalApplication : Castle.Components.Localization.MonoRail.GlobalApplication, IContainerAccessor
	{

		#region Static Variables 

        static WindsorContainer _Container;

		#endregion Static Variables 

		#region Instance Variables 

        bool _scopeDisposed;

		#endregion Instance Variables 

		#region Properties 

        public static IWindsorContainer Container
        {
            get { return _Container; }
        }

		IWindsorContainer IContainerAccessor.Container
		{
			get { return _Container; }
		}

		#endregion Properties 

		#region Constructors 

        public GlobalApplication()
        {
            BeginRequest += new EventHandler( Application_OnBeginRequest );
            EndRequest += new EventHandler( Application_OnEndRequest );
        }

		#endregion Constructors

        #region Application Event Handlers

        public void Application_OnStart()
        {
            _Container = new WindsorContainer( new XmlInterpreter( new ConfigResource() ) );
		}

        public void Application_OnEnd()
        {
            _Container.Dispose();
        }

        void Application_OnEndRequest( object sender, EventArgs e )
        {
        }

        #endregion Application Event Handlers

	}
}