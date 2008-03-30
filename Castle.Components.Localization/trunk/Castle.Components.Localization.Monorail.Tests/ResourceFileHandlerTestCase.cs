
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

	using Castle.Core.Resource;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Routing;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Container;
	using Castle.MonoRail.Framework.Descriptors;

	using NUnit.Framework;

	using Rhino.Mocks;
	using Castle.MonoRail.Framework.Test;
	using System.Collections.Generic;
	using Castle.MonoRail.TestSupport;

	#endregion Using Directives

	[TestFixture]
	public class ResourceFileHandlerTestCase 
	{

		#region Public Methods 

		[Test]
		public void IfFileDoesNotExistsSetsStatusTo404()
		{
			DefaultStaticResourceRegistry registry = new DefaultStaticResourceRegistry();

			ResourceFileHandler handler = new ResourceFileHandler( new UrlInfo( "", "controller", "action" ), registry );

			StringWriter writer = new StringWriter();

			HttpResponse response = new HttpResponse( writer );
			HttpRequest request = new HttpRequest(
				Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory, 
					"Handlers/Files/simplerequest.txt" ),
					"http://localhost:1333/controller/action", 
					"" );

			handler.ProcessRequest( new HttpContext( request, response ) );

			Assert.AreEqual( 404, response.StatusCode );
		}

		[Test]
		public void ReturnsResourceContentAndSetMimeType()
		{
			DefaultStaticResourceRegistry registry = new DefaultStaticResourceRegistry();
			registry.RegisterCustomResource( "key", null, null, new StaticContentResource( "js" ), "text/javascript" );

			ResourceFileHandler handler = new ResourceFileHandler( new UrlInfo( "", "controller", "key" ), registry );

			StringWriter writer = new StringWriter();

			HttpResponse response = new HttpResponse( writer );
			HttpRequest request = new HttpRequest(
				Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory, 
					"Handlers/Files/simplerequest.txt" ),
					"http://localhost:1333/controller/action", 
					"" );

			handler.ProcessRequest( new HttpContext( request, response ) );

			Assert.AreEqual( 200, response.StatusCode );
			Assert.AreEqual( "text/javascript", response.ContentType );
			Assert.AreEqual( "js", writer.GetStringBuilder().ToString() );
		}

		#endregion Public Methods 

	}
}