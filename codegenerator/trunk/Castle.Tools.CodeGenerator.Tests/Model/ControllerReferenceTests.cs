using System;
using System.IO;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Test;
using Castle.Tools.CodeGenerator.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Castle.Tools.CodeGenerator.Model
{
	[TestFixture]
	public class ActionArgumentTests
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_mocks = new MockRepository();
		}

		#endregion

		private MockRepository _mocks;

		[Test]
		public void Constructor_Always_ProperlyInitializes()
		{
			ActionArgument argument = new ActionArgument(0, "id", 1);
			Assert.AreEqual("id", argument.Name);
			Assert.AreEqual(typeof (Int32), argument.Type);
			Assert.AreEqual(1, argument.Value);
			Assert.AreEqual(0, argument.Index);
		}
	}

	[TestFixture]
	public class ControllerViewReferenceTests : ControllerReferenceTests
	{
		#region Setup/Teardown

		[SetUp]
		public override void Setup()
		{
			base.Setup();
		}

		#endregion

		[Test]
		public void Constructor_Always_ProperlyInitializes()
		{
			ControllerViewReference reference =
				new ControllerViewReference(_services, typeof (TestController), "Area", "Test", "Action");
			Assert.AreEqual("Action", reference.ActionName);
			Assert.AreEqual("Area", reference.AreaName);
			Assert.AreEqual("Test", reference.ControllerName);
			Assert.AreEqual(typeof (TestController), reference.ControllerType);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Constructor_NoServices_Throws()
		{
			new ControllerViewReference(null, typeof (TestController), "Area", "Test", "Action");
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Constructor_NoType_Throws()
		{
			new ControllerViewReference(_services, null, "Area", "Test", "Action");
		}

		[Test]
		public void Render_Always_SetsSelectedView()
		{
			Controller controller = new TestController();
			controller.ControllerContext = _mocks.Stub<IControllerContext>();
			ControllerViewReference reference =
				new ControllerViewReference(_services, typeof (TestController), "Area", "Test", "Action");

			using (_mocks.Unordered())
			{
				Expect.Call(_services.Controller).Return(controller).Repeat.Any();
			}

			_mocks.ReplayAll();
			reference.Render();
			_mocks.VerifyAll();

			Assert.AreEqual(@"Area\Test\Action", controller.SelectedViewName);
		}

		[Test]
		public void Render_NoArea_SetsSelectedView()
		{
			Controller controller = new TestController();
			controller.ControllerContext = _mocks.Stub<IControllerContext>();
			ControllerViewReference reference =
				new ControllerViewReference(_services, typeof (TestController), "", "Test", "Action");

			using (_mocks.Unordered())
			{
				Expect.Call(_services.Controller).Return(controller).Repeat.Any();
			}

			_mocks.ReplayAll();
			reference.Render();
			_mocks.VerifyAll();

			Assert.AreEqual(@"Test\Action", controller.SelectedViewName);
		}
	}

	public class ControllerReferenceTests
	{
		#region Member Data

		protected IArgumentConversionService _argumentConversionService;
		protected TestController _controller;
		protected MockRepository _mocks;
		protected IEngineContext _railsContext;
		protected ControllerActionReference _reference;
		protected MockResponse _response;
		protected IServerUtility _serverUtility;
		protected ICodeGeneratorServices _services;
		protected string _virtualDirectory = String.Empty;

		#endregion

		#region Test Setup and Teardown Methods

		[SetUp]
		public virtual void Setup()
		{
			_mocks = new MockRepository();
			_services = _mocks.CreateMock<ICodeGeneratorServices>();
			_response = new MockResponse();
			UrlInfo url =
				new UrlInfo("eleutian.com", "www", _virtualDirectory, "http", 80,
				            Path.Combine(Path.Combine("Area", "Controller"), "Action"), "Area", "Controller", "Action", "rails",
				            "");
			_railsContext =
				new MockEngineContext(new MockRequest(), _response, new MockServices(), url);

			((MockEngineContext) _railsContext).Server = _mocks.DynamicMock<IServerUtility>();
			_serverUtility = _railsContext.Server;

			_argumentConversionService = _mocks.CreateMock<IArgumentConversionService>();
			_controller = new TestController();
		}

		[TearDown]
		public virtual void Teardown()
		{
			_virtualDirectory = String.Empty;
		}

		#endregion

		#region Methods

		protected void SetupMocks()
		{
			Expect.Call(_services.ArgumentConversionService).Return(_argumentConversionService).Repeat.Any();
			Expect.Call(_services.RailsContext).Return(_railsContext).Repeat.Any();
			Expect.Call(_services.Controller).Return(_controller).Repeat.Any();

			_controller.ControllerContext = _mocks.Stub<IControllerContext>();
			_controller.SetEngineContext(_railsContext);

			//Expect.Call(_railsContext.Server).Return(_serverUtility).Repeat.Any();
			//Expect.Call(_railsContext.UrlInfo).Return(
			//  new UrlInfo("eleutian.com", "www", _virtualDirectory, "http", 80,
			//              Path.Combine(Path.Combine("Area", "Controller"), "Action"), "Area", "Controller",
			//              "Action", "rails", "")).Repeat.Any();
		}

		#endregion
	}

	[TestFixture]
	public class ControllerActionReferenceBlankVirtualDirectoryTests : ControllerReferenceTests
	{
		protected void New(string area, string controller, string action, params ActionArgument[] arguments)
		{
			_reference =
				new ControllerActionReference(_services, typeof (TestController), area, controller, action, null, arguments);
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

			using (_mocks.Unordered())
			{
				SetupMocks();
				_response.Write("<script type=\"text/javascript\">window.location = \"/Area/Controller/Action.rails\";</script>");
			}

			_mocks.ReplayAll();
			_reference.Redirect(true);
			_mocks.VerifyAll();
		}

		[Test]
		public void Redirect_NoArgumentsWithArea_Redirects()
		{
			New("Area", "Controller", "Action");

			using (_mocks.Unordered())
			{
				SetupMocks();
			}

			_mocks.ReplayAll();
			_reference.Redirect();

			Assert.IsTrue(this._controller.Context.Response.WasRedirected);
			Assert.AreEqual(_response.RedirectedTo, "/Area/Controller/Action.rails");

			_mocks.VerifyAll();			
		}

		[Test]
		public void Redirect_NoArgumentsWithoutArea_Redirects()
		{
			New(string.Empty, "Controller", "Action");

			using (_mocks.Unordered())
			{
				SetupMocks();
			}

			_mocks.ReplayAll();
			_reference.Redirect();
			_mocks.VerifyAll();

			Assert.IsTrue(this._controller.Context.Response.WasRedirected);
			Assert.AreEqual(_response.RedirectedTo, "/Controller/Action.rails");
		}

		[Test]
		public void Redirect_OneArgumentWithArea_Redirects()
		{
			ActionArgument argument = new ActionArgument(0, "name", "Jacob");
			New("Area", "Controller", "Action", argument);

			using (_mocks.Unordered())
			{
				SetupMocks();
				Expect.Call(_argumentConversionService.ConvertArgument(null, argument)).Return("Jacob");
				Expect.Call(_argumentConversionService.ConvertKey(null, argument)).Return("name");
				Expect.Call(_serverUtility.UrlEncode("name")).Return("Ename");
				Expect.Call(_serverUtility.UrlEncode("Jacob")).Return("EJacob");
			}

			_mocks.ReplayAll();
			_reference.Redirect();
			_mocks.VerifyAll();

			Assert.IsTrue(this._controller.Context.Response.WasRedirected);
			Assert.AreEqual(_response.RedirectedTo, "/Area/Controller/Action.rails?Ename=EJacob");
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void Transfer_MethodIsBad_Throws()
		{
			ActionArgument argument = new ActionArgument(0, "name", "Jacob");
			New("Area", "Controller", "BadAction", argument);

			using (_mocks.Unordered())
			{
				SetupMocks();
			}

			_mocks.ReplayAll();
			_reference.Transfer();
		}

		[Test]
		public void Transfer_MethodIsReal_InvokesAction()
		{
			ActionArgument argument = new ActionArgument(0, "name", "Jacob");
			New("Area", "Controller", "Index", argument);

			using (_mocks.Unordered())
			{
				SetupMocks();
			}

			_mocks.ReplayAll();
			_reference.Transfer();
			_mocks.VerifyAll();
		}

		[Test]
		public void Url_AnArea_ReturnsUrl()
		{
			New("Area", "Controller", "Action");

			using (_mocks.Unordered())
			{
				SetupMocks();
			}

			_mocks.ReplayAll();
			string actual = _reference.Url;
			_mocks.VerifyAll();
			Assert.AreEqual("/Area/Controller/Action.rails", actual);
		}

		[Test]
		public void Url_NoArea_ReturnsUrl()
		{
			New(string.Empty, "Controller", "Action");

			using (_mocks.Unordered())
			{
				SetupMocks();
			}

			_mocks.ReplayAll();
			string actual = _reference.Url;
			_mocks.VerifyAll();
			Assert.AreEqual("/Controller/Action.rails", actual);
		}

		[Test]
		public void Url_OneArgument_ReturnsUrl()
		{
			ActionArgument argument = new ActionArgument(0, "name", "Jacob");
			New("Area", "Controller", "Action", argument);

			using (_mocks.Unordered())
			{
				SetupMocks();
				Expect.Call(_argumentConversionService.ConvertArgument(null, argument)).Return("Jacob");
				Expect.Call(_argumentConversionService.ConvertKey(null, argument)).Return("name");
				Expect.Call(_serverUtility.UrlEncode("name")).Return("name");
				Expect.Call(_serverUtility.UrlEncode("Jacob")).Return("Jacob");
			}

			_mocks.ReplayAll();
			string actual = _reference.Url;
			_mocks.VerifyAll();
			Assert.AreEqual("/Area/Controller/Action.rails?name=Jacob", actual);
		}

		[Test]
		public void Url_OneArgument_ReturnsUrlUsesEncoded()
		{
			ActionArgument argument = new ActionArgument(0, "name", "Jacob");
			New("Area", "Controller", "Action", argument);

			using (_mocks.Unordered())
			{
				SetupMocks();
				Expect.Call(_argumentConversionService.ConvertArgument(null, argument)).Return("Jacob");
				Expect.Call(_argumentConversionService.ConvertKey(null, argument)).Return("name");

				Expect.Call(_serverUtility.UrlEncode("name")).Return("Ename");
				Expect.Call(_serverUtility.UrlEncode("Jacob")).Return("EJacob");
			}

			_mocks.ReplayAll();
			string actual = _reference.Url;
			_mocks.VerifyAll();
			Assert.AreEqual("/Area/Controller/Action.rails?Ename=EJacob", actual);
		}
	}

	[TestFixture]
	public class ControllerActionReferenceUnderVirtualDirectoryTests : ControllerReferenceTests
	{
		#region Setup/Teardown

		[SetUp]
		public override void Setup()
		{
			_virtualDirectory = "Directory";
			base.Setup();
		}

		#endregion

		protected void New(string area, string controller, string action, params ActionArgument[] arguments)
		{
			_reference =
				new ControllerActionReference(_services, typeof (TestController), area, controller, action, null, arguments);
		}

		[Test]
		public void Redirect_NoArgumentsWithArea_Redirects()
		{
			New("Area", "Controller", "Action");

			using (_mocks.Unordered())
			{
				SetupMocks();
			}

			_mocks.ReplayAll();
			_reference.Redirect();
			_mocks.VerifyAll();

			Assert.IsTrue(this._controller.Context.Response.WasRedirected);
			Assert.AreEqual(_response.RedirectedTo, "/Directory/Area/Controller/Action.rails");
		}

		[Test]
		public void Url_OneArgument_ReturnsUrl()
		{
			ActionArgument argument = new ActionArgument(0, "name", "Jacob");
			New("Area", "Controller", "Action", argument);

			using (_mocks.Unordered())
			{
				SetupMocks();
				Expect.Call(_argumentConversionService.ConvertArgument(null, argument)).Return("Jacob");
				Expect.Call(_argumentConversionService.ConvertKey(null, argument)).Return("name");
				Expect.Call(_serverUtility.UrlEncode("name")).Return("name");
				Expect.Call(_serverUtility.UrlEncode("Jacob")).Return("Jacob");
			}

			_mocks.ReplayAll();
			string actual = _reference.Url;
			_mocks.VerifyAll();
			Assert.AreEqual("/Directory/Area/Controller/Action.rails?name=Jacob", actual);
		}
	}

	public class TestController : Controller
	{
		public void Index(string name)
		{
		}
	}
}