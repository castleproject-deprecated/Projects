
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

namespace Castle.Components.Localization.MonoRail.Tests
{
	#region Using Directives

	using System;
	using System.IO;
	using System.Web;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Routing;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Container;
	using Castle.MonoRail.Framework.Descriptors;

	using NUnit.Framework;

	using Rhino.Mocks;

	#endregion Using Directives

	[TestFixture]
	public class MRHandlerFactoryTestCase
	{

		#region Instance Variables 

		private IMonoRailContainer _Container;
		private IControllerContextFactory _ControllerContextFactoryMock;
		private IControllerDescriptorProvider _ControllerDescriptorProviderMock;
		private IControllerFactory _ControllerFactoryMock;
		private IController _ControllerMock;
		private MonoRailHttpHandlerFactory _HandlerFactory;
		private MockRepository _MockRepository = new MockRepository();
		private IServiceProviderLocator _ServiceProviderLocatorMock;

		#endregion Instance Variables 

		#region Public Methods 

		[SetUp]
		public void Init()
		{
			_Container = _MockRepository.CreateMock<IMonoRailContainer>();
			_ServiceProviderLocatorMock = _MockRepository.CreateMock<IServiceProviderLocator>();
			_ControllerFactoryMock = _MockRepository.CreateMock<IControllerFactory>();
			_ControllerMock = _MockRepository.CreateMock<IController>();
			_ControllerDescriptorProviderMock = _MockRepository.CreateMock<IControllerDescriptorProvider>();
			_ControllerContextFactoryMock = _MockRepository.CreateMock<IControllerContextFactory>();

			SetupResult.For( _Container.UrlTokenizer ).Return( new DefaultUrlTokenizer() );
			SetupResult.For( _Container.UrlBuilder ).Return( new DefaultUrlBuilder() );
			SetupResult.For( _Container.EngineContextFactory ).Return( new DefaultEngineContextFactory() );
			SetupResult.For( _Container.ControllerFactory ).Return( _ControllerFactoryMock );
			SetupResult.For( _Container.ControllerContextFactory ).Return( _ControllerContextFactoryMock );
			SetupResult.For( _Container.ControllerDescriptorProvider ).Return( _ControllerDescriptorProviderMock );
			SetupResult.For( _Container.StaticResourceRegistry ).Return( new DefaultStaticResourceRegistry() );

			_HandlerFactory = new MonoRailHttpHandlerFactory( _ServiceProviderLocatorMock );
			_HandlerFactory.ResetState();
			_HandlerFactory.Configuration = new MonoRailConfiguration();
			_HandlerFactory.Container = _Container;
		}

		[Test]
		public void Request_CreatesSessionfulHandler()
		{
			StringWriter writer = new StringWriter();

			HttpResponse res = new HttpResponse( writer );
			HttpRequest req = new HttpRequest( Path.Combine(
												AppDomain.CurrentDomain.BaseDirectory, "Handlers/Files/simplerequest.txt" ),
											  "http://localhost:1333/home/something", "" );
			RouteMatch routeMatch = new RouteMatch();
			HttpContext httpCtx = new HttpContext( req, res );
			httpCtx.Items[ RouteMatch.RouteMatchKey ] = routeMatch;

			using ( _MockRepository.Record() )
			{
				ControllerMetaDescriptor controllerDesc = new ControllerMetaDescriptor();
				controllerDesc.ControllerDescriptor = new ControllerDescriptor( typeof( Controller ), "home", "", false );

				Expect.Call( _ControllerFactoryMock.CreateController( "", "home" ) ).IgnoreArguments().Return( _ControllerMock );
				Expect.Call( _ControllerDescriptorProviderMock.BuildDescriptor( _ControllerMock ) ).Return( controllerDesc );
				Expect.Call( _ControllerContextFactoryMock.Create( "", "home", "something", controllerDesc, routeMatch ) ).
					Return( new ControllerContext() );
			}

			using ( _MockRepository.Playback() )
			{
				IHttpHandler handler = _HandlerFactory.GetHandler( httpCtx, "GET", "", "" );

				Assert.IsNotNull( handler );
				Assert.IsInstanceOfType( typeof( MonoRailHttpHandler ), handler );
			}
		}

		[Test]
		public void Request_CreatesSessionlessHandler()
		{
			StringWriter writer = new StringWriter();

			HttpResponse res = new HttpResponse( writer );
			HttpRequest req = new HttpRequest( Path.Combine(
												AppDomain.CurrentDomain.BaseDirectory, "Handlers/Files/simplerequest.txt" ),
											  "http://localhost:1333/home/something", "" );
			RouteMatch routeMatch = new RouteMatch();
			HttpContext httpCtx = new HttpContext( req, res );
			httpCtx.Items[ RouteMatch.RouteMatchKey ] = routeMatch;

			using ( _MockRepository.Record() )
			{
				ControllerMetaDescriptor controllerDesc = new ControllerMetaDescriptor();
				controllerDesc.ControllerDescriptor = new ControllerDescriptor( typeof( Controller ), "home", "", true );

				Expect.Call( _ControllerFactoryMock.CreateController( "", "home" ) ).IgnoreArguments().Return( _ControllerMock );
				Expect.Call( _ControllerDescriptorProviderMock.BuildDescriptor( _ControllerMock ) ).Return( controllerDesc );
				Expect.Call( _ControllerContextFactoryMock.Create( "", "home", "something", controllerDesc, routeMatch ) ).
					Return( new ControllerContext() );
			}

			using ( _MockRepository.Playback() )
			{
				IHttpHandler handler = _HandlerFactory.GetHandler( httpCtx, "GET", "", "" );

				Assert.IsNotNull( handler );
				Assert.IsInstanceOfType( typeof( SessionlessMonoRailHttpHandler ), handler );
			}
		}

		#endregion Public Methods 

	}
}