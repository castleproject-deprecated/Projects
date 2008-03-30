
#region License

// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion License

namespace Castle.Components.Localization.MonoRail.Framework
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Threading;
	using System.Web;

	using Castle.Core;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Adapters;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Container;
	using Castle.MonoRail.Framework.Descriptors;
	using Castle.MonoRail.Framework.Routing;
	using Castle.MonoRail.Framework.Services;

	#endregion Using Directives

	public class MonoRailHttpHandlerFactoryEx : IHttpHandlerFactory
	{

		#region Constant Variables 

		const string CurrentControllerContextKey = "currentmrcontrollercontext";
		const string CurrentControllerKey = "currentmrcontroller";
		const string CurrentEngineContextKey = "currentmrengineinstance";

		#endregion Constant Variables 

		#region Static Variables 

		static IMonoRailConfiguration _Configuration;
		static IControllerContextFactory _ControllerContextFactory;
		static IControllerFactory _ControllerFactory;
		static IEngineContextFactory _EngineContextFactory;
		static IMonoRailContainerEx _MonoRailContainer;
		static IServiceProviderLocator _ServiceProviderLocator;
		static IStaticResourceRegistryEx _StaticResourceRegistry;
		static IUrlTokenizer _UrlTokenizer;

		#endregion Static Variables 

		#region Instance Variables 

		readonly ReaderWriterLock _Locker;

		#endregion Instance Variables 

		#region Properties 

		public IMonoRailConfiguration Configuration
		{
			get
			{
				return _Configuration;
			}
			set
			{
				_Configuration = value;
			}
		}

		public IMonoRailContainerEx Container
		{
			get
			{
				return _MonoRailContainer;
			}
			set
			{
				_MonoRailContainer = value;
			}
		}

		public IMonoRailContainerEx ContainerEx
		{
			get { return Container as IMonoRailContainerEx; }
		}

		public IControllerContextFactory ControllerContextFactory
		{
			get
			{
				return _ControllerContextFactory;
			}
			set
			{
				_ControllerContextFactory = value;
			}
		}

		public IControllerFactory ControllerFactory
		{
			get
			{
				return _ControllerFactory;
			}
			set
			{
				_ControllerFactory = value;
			}
		}

		public static IController CurrentController
		{
			get
			{
				return ( HttpContext.Current.Items[ CurrentControllerKey ] as IController );
			}
		}

		public static IControllerContext CurrentControllerContext
		{
			get
			{
				return ( HttpContext.Current.Items[ CurrentControllerContextKey ] as IControllerContext );
			}
		}

		public static IEngineContext CurrentEngineContext
		{
			get
			{
				return ( HttpContext.Current.Items[ CurrentEngineContextKey ] as IEngineContext );
			}
		}

		public IEngineContextFactory EngineContextFactory
		{
			get
			{
				return _EngineContextFactory;
			}
			set
			{
				_EngineContextFactory = value;
			}
		}

		public IServiceProviderLocator ProviderLocator
		{
			get
			{
				return _ServiceProviderLocator;
			}
			set
			{
				_ServiceProviderLocator = value;
			}
		}

		public IStaticResourceRegistryEx StaticResourceRegistry
		{
			get { return _StaticResourceRegistry; }
			set { _StaticResourceRegistry = value; }
		}

		public IUrlTokenizer UrlTokenizer
		{
			get
			{
				return _UrlTokenizer;
			}
			set
			{
				_UrlTokenizer = value;
			}
		}

		#endregion Properties 

		#region Constructors 

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailHttpHandlerFactoryEx"/> class.
		/// </summary>
		public MonoRailHttpHandlerFactoryEx()
			: this( ServiceProviderLocator.Instance )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailHttpHandlerFactoryEx"/> class.
		/// </summary>
		/// <param name="serviceLocator">The service locator.</param>
		public MonoRailHttpHandlerFactoryEx( IServiceProviderLocator serviceLocator )
		{
			_Locker = new ReaderWriterLock();
			_ServiceProviderLocator = serviceLocator;
		}

		#endregion Constructors 

		#region Public Methods 

		public virtual IHttpHandler GetHandler( HttpContext context, string requestType, string url, string pathTranslated )
		{
			IController controller;
			PerformOneTimeInitializationIfNecessary( context );
			EnsureServices();

			HttpRequest request = context.Request;
			RouteMatch routeMatch = ( ( RouteMatch )context.Items[ RouteMatch.RouteMatchKey ] ) ?? new RouteMatch();

			UrlInfo urlInfo = UrlTokenizer.TokenizeUrl(
								request.FilePath,
								request.PathInfo,
								request.Url,
								request.IsLocal,
								request.ApplicationPath );

			if ( urlInfo.Area.Equals( "MonoRail", StringComparison.CurrentCultureIgnoreCase ) && 
				urlInfo.Controller.Equals("Files", StringComparison.CurrentCultureIgnoreCase ) )
			{
				return new ResourceFileHandler( urlInfo, new DefaultStaticResourceRegistry() );
			}

			if ( urlInfo.Area.Equals( "MonoRail", StringComparison.CurrentCultureIgnoreCase ) &&
				urlInfo.Controller.Equals( "Resources", StringComparison.CurrentCultureIgnoreCase ) )
			{
				return new ResourceFileHandlerEx( urlInfo, new DefaultStaticResourceRegistryEx() );
			}

			IEngineContext serviceInstance = _EngineContextFactory.Create( 
												_MonoRailContainer, 
												urlInfo, 
												context, 
												routeMatch );

			serviceInstance.AddService( typeof( IEngineContext ), serviceInstance );

			try
			{
				controller = _ControllerFactory.CreateController( urlInfo.Area, urlInfo.Controller );
			}
			catch ( ControllerNotFoundException )
			{
				return new MonoRailHttpHandlerFactory.NotFoundHandler( 
								urlInfo.Area, 
								urlInfo.Controller, 
								serviceInstance );
			}

			ControllerMetaDescriptor metaDescriptor = _MonoRailContainer.ControllerDescriptorProvider.BuildDescriptor( controller );

			IControllerContext context3 = _ControllerContextFactory.Create( 
											urlInfo.Area, 
											urlInfo.Controller, 
											urlInfo.Action, 
											metaDescriptor, 
											routeMatch );

			serviceInstance.CurrentController = controller;
			serviceInstance.CurrentControllerContext = context3;
			context.Items[ CurrentEngineContextKey ] = serviceInstance;
			context.Items[ CurrentControllerKey ] = controller;
			context.Items[ CurrentControllerContextKey ] = context3;

			if ( IgnoresSession( metaDescriptor.ControllerDescriptor ) )
			{
				return new SessionlessMonoRailHttpHandler( serviceInstance, controller, context3 );
			}

			return new MonoRailHttpHandler( serviceInstance, controller, context3 );
		}

		public virtual void ReleaseHandler( IHttpHandler handler )
		{
		}

		public void ResetState()
		{
			_Configuration = null;
			_MonoRailContainer = null;
			_UrlTokenizer = null;
			_EngineContextFactory = null;
			_ServiceProviderLocator = null;
			_ControllerFactory = null;
			_ControllerContextFactory = null;
			_StaticResourceRegistry = null;
		}

		#endregion Public Methods 

		#region Protected Methods 

		protected virtual IMonoRailContainerEx CreateDefaultMonoRailContainer( IServiceProviderEx userServiceProvider, HttpApplication appInstance )
		{
			DefaultMonoRailContainerEx container = new DefaultMonoRailContainerEx( userServiceProvider );

			container.UseServicesFromParent();
			container.InstallPrimordialServices();
			container.Configure( Configuration );
			FireContainerCreated( appInstance, container );

			if ( !container.HasService<IServerUtility>() )
			{
				container.AddService<IServerUtility>( new ServerUtilityAdapter( appInstance.Context.Server ) );
			}

			if ( !container.HasService<IRoutingEngine>() )
			{
				container.AddService<IRoutingEngine>( RoutingModuleEx.Engine );
			}

			container.InstallMissingServices();
			container.StartExtensionManager();
			FireContainerInitialized( appInstance, container );

			return container;
		}

		protected virtual bool IgnoresSession( ControllerDescriptor controllerDesc )
		{
			return controllerDesc.Sessionless;
		}

		#endregion Protected Methods 

		#region Private Methods 

		static void ExecuteContainerEvent( MethodInfo eventMethod, HttpApplication instance, DefaultMonoRailContainer container )
		{
			if ( eventMethod != null )
			{
				if ( eventMethod.IsStatic )
				{
					eventMethod.Invoke( null, new object[] { container } );
				}
				else
				{
					eventMethod.Invoke( instance, new object[] { container } );
				}
			}
		}

		void EnsureServices()
		{
			if ( _UrlTokenizer == null )
			{
				_UrlTokenizer = _MonoRailContainer.UrlTokenizer;
			}
			if ( _EngineContextFactory == null )
			{
				_EngineContextFactory = _MonoRailContainer.EngineContextFactory;
			}
			if ( _ControllerFactory == null )
			{
				_ControllerFactory = _MonoRailContainer.ControllerFactory;
			}
			if ( _ControllerContextFactory == null )
			{
				_ControllerContextFactory = _MonoRailContainer.ControllerContextFactory;
			}
			if ( _StaticResourceRegistry == null )
			{
				_StaticResourceRegistry = _MonoRailContainer.StaticResourceRegistryEx;
			}
		}

		void FireContainerCreated( HttpApplication instance, DefaultMonoRailContainer container )
		{
			ExecuteContainerEvent( 
				instance.GetType().GetMethod( "MonoRail_ContainerCreated" ), 
				instance, 
				container );
		}

		void FireContainerInitialized( HttpApplication instance, DefaultMonoRailContainer container )
		{
			ExecuteContainerEvent( 
				instance.GetType().GetMethod( "MonoRail_ContainerInitialized" ), 
				instance, 
				container );
		}

		IMonoRailConfiguration ObtainConfiguration( HttpApplication appInstance )
		{
			IMonoRailConfiguration config = MonoRailConfiguration.GetConfig();
			MethodInfo method = appInstance.GetType().GetMethod( "MonoRail_Configure" );

			if ( method != null )
			{
				config = config ?? new MonoRailConfiguration();

				if ( method.IsStatic )
				{
					method.Invoke( null, new object[] { config } );
				}
				else
				{
					method.Invoke( appInstance, new object[] { config } );
				}
			}

			if ( config == null )
			{
				throw new ApplicationException( "You have to provide a small configuration to use MonoRail. This can be done using the web.config or your global asax (your class that extends HttpApplication) through the method MonoRail_Configure(IMonoRailConfiguration config). Check the samples or the documentation." );
			}

			return config;
		}

		void PerformOneTimeInitializationIfNecessary( HttpContext context )
		{
			_Locker.AcquireReaderLock( -1 );

			if ( _MonoRailContainer != null )
			{
				_Locker.ReleaseReaderLock();
			}
			else
			{
				_Locker.UpgradeToWriterLock( -1 );

				if ( _MonoRailContainer != null )
				{
					_Locker.ReleaseWriterLock();
				}
				else
				{
					try
					{
						if ( _Configuration == null )
						{
							_Configuration = ObtainConfiguration( context.ApplicationInstance );
						}

						IServiceProviderEx userServiceProvider = _ServiceProviderLocator.LocateProvider();
						_MonoRailContainer = CreateDefaultMonoRailContainer( 
												userServiceProvider, 
												context.ApplicationInstance );
					}
					finally
					{
						_Locker.ReleaseWriterLock();
					}
				}
			}
		}

		#endregion Private Methods 

	}
}
