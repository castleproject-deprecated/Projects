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

namespace Castle.Tools.CodeGenerator.Model
{
	using System;
	using NUnit.Framework;
	using Rhino.Mocks;
	using External;

	[TestFixture]
	public class ControllerActionReferenceBlankVirtualDirectoryTests : ControllerReferenceTests
	{
		protected void New(string area, string controller, string action, params ActionArgument[] arguments)
		{
			reference = new ControllerActionReference(services, typeof (TestController), area, controller, action, null, arguments);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Constructor_NoAction_ThrowsException()
		{
			New(null, "Controller", null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Constructor_NoController_ThrowsException()
		{
			New(null, null, "Action");
		}

		[Test]
		public void Redirect_Ajax_Works()
		{
			New("Area", "Controller", "Action");

			response.Write("<script type=\"text/javascript\">window.location = \"/Area/Controller/Action.rails\";</script>");
			reference.Redirect(true);
		}

		[Test]
		public void Redirect_NoArgumentsWithArea_Redirects()
		{
			New("Area", "Controller", "Action");

			reference.Redirect();

			Assert.IsTrue(controller.Context.Response.WasRedirected);
			Assert.AreEqual(response.RedirectedTo, "/Area/Controller/Action.rails");
		}

		[Test]
		public void Redirect_NoArgumentsWithoutArea_Redirects()
		{
			New(string.Empty, "Controller", "Action");

			reference.Redirect();

			Assert.IsTrue(controller.Context.Response.WasRedirected);
			Assert.AreEqual(response.RedirectedTo, "/Controller/Action.rails");
		}

		[Test]
		public void Redirect_OneArgumentWithArea_Redirects()
		{
			var argument = new ActionArgument(0, "name", "Jacob");
			New("Area", "Controller", "Action", argument);

			argumentConversionService.Expect(s => s.ConvertArgument(null, argument, parameters)).Return(true);
			serverUtility.Expect(s => s.UrlEncode("name")).Return("name");
			serverUtility.Expect(s => s.UrlEncode("Jacob")).Return("Jacob");
			parameters.Add("name", "Jacob");
			
			reference.Redirect();
			
			Assert.IsTrue(controller.Context.Response.WasRedirected);
			Assert.AreEqual("/Area/Controller/Action.rails?name=Jacob", response.RedirectedTo);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void Transfer_MethodIsBad_Throws()
		{
			var argument = new ActionArgument(0, "name", "Jacob");
			New("Area", "Controller", "BadAction", argument);

			reference.Transfer();
		}

		[Test]
		public void Transfer_MethodIsReal_InvokesAction()
		{
			var argument = new ActionArgument(0, "name", "Jacob");
			New("Area", "Controller", "Index", argument);

			reference.Transfer();
		}

		[Test]
		public void Url_AnArea_ReturnsUrl()
		{
			New("Area", "Controller", "Action");
			
			var actual = reference.Url;

			Assert.AreEqual("/Area/Controller/Action.rails", actual);
		}

		[Test]
		public void Url_NoArea_ReturnsUrl()
		{
			New(string.Empty, "Controller", "Action");

			var actual = reference.Url;
			
			Assert.AreEqual("/Controller/Action.rails", actual);
		}

		[Test]
		public void Url_OneArgument_ReturnsUrl()
		{
			var argument = new ActionArgument(0, "name", "Jacob");
			New("Area", "Controller", "Action", argument);

			argumentConversionService.Expect(s => s.ConvertArgument(null, argument, parameters)).Return(true);
			serverUtility.Expect(s => s.UrlEncode("name")).Return("name");
			serverUtility.Expect(s => s.UrlEncode("Jacob")).Return("Jacob");
			parameters.Add("name", "Jacob");

			var actual = reference.Url;

			Assert.AreEqual("/Area/Controller/Action.rails?name=Jacob", actual);
		}
	}
}