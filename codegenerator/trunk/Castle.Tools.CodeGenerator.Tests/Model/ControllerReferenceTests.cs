using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Castle.MonoRail.Framework;
using Castle.Tools.CodeGenerator.Services;
using Rhino.Mocks;
using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.Model
{
	using Castle.MonoRail.Framework.Test;

	[TestFixture]
  public class ActionArgumentTests
  {
    #region Member Data
    private MockRepository _mocks;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _mocks = new MockRepository();
    }
    #endregion

    #region Test Methods
    [Test]
    public void Constructor_Always_ProperlyInitializes()
    {
      ActionArgument argument = new ActionArgument(0, "id", 1);
      Assert.AreEqual("id", argument.Name);
      Assert.AreEqual(typeof(Int32), argument.Type);
      Assert.AreEqual(1, argument.Value);
      Assert.AreEqual(0, argument.Index);
    }
    #endregion
  }

  [TestFixture]
  public class ControllerViewReferenceTests : ControllerReferenceTests
  {
    #region Test Setup and Teardown Methods
    [SetUp]
    public override void Setup()
    {
      base.Setup();
    }
    #endregion

    #region Test Methods
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NoServices_Throws()
    {
      new ControllerViewReference(null, typeof(TestController), "Area", "Test", "Action");
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NoType_Throws()
    {
      new ControllerViewReference(_services, null, "Area", "Test", "Action");
    }

    [Test]
    public void Constructor_Always_ProperlyInitializes()
    {
      ControllerViewReference reference = new ControllerViewReference(_services, typeof(TestController), "Area", "Test", "Action");
      Assert.AreEqual("Action", reference.ActionName);
      Assert.AreEqual("Area", reference.AreaName);
      Assert.AreEqual("Test", reference.ControllerName);
      Assert.AreEqual(typeof(TestController), reference.ControllerType);
    }

    [Test]
    public void Render_Always_SetsSelectedView()
    {
      Controller controller = new TestController();
    	controller.ControllerContext = this._mocks.Stub<IControllerContext>();
      ControllerViewReference reference = new ControllerViewReference(_services, typeof(TestController), "Area", "Test", "Action");

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
    	controller.ControllerContext = this._mocks.Stub<IControllerContext>();
      ControllerViewReference reference = new ControllerViewReference(_services, typeof(TestController), "", "Test", "Action");

      using (_mocks.Unordered())
      {
        Expect.Call(_services.Controller).Return(controller).Repeat.Any();
      }

      _mocks.ReplayAll();
      reference.Render();
      _mocks.VerifyAll();

      Assert.AreEqual(@"Test\Action", controller.SelectedViewName);
    }
    #endregion
  }

  public class ControllerReferenceTests
  {
    #region Member Data
    protected MockRepository _mocks;
    protected ICodeGeneratorServices _services;
    protected TestController _controller;
    protected ControllerActionReference _reference;
    protected IServerUtility _serverUtility;
    protected IEngineContext _railsContext;
    protected IRedirectService _redirectService;
    protected IArgumentConversionService _argumentConversionService;
    protected IMockResponse _response;
    protected string _virtualDirectory = String.Empty;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public virtual void Setup()
    {
      
      _mocks = new MockRepository();
      _services = _mocks.CreateMock<ICodeGeneratorServices>();
      _response = _mocks.CreateMock<IMockResponse>();
    	UrlInfo url =
    		new UrlInfo("eleutian.com", "www", _virtualDirectory, "http", 80,
    		            Path.Combine(Path.Combine("Area", "Controller"), "Action"), "Area", "Controller", "Action", "rails",
    		            "");
    	_railsContext =
    		new Castle.MonoRail.Framework.Test.MockEngineContext(new MockRequest(), _response, new MockServices(),url);
    	
			((MockEngineContext) _railsContext).Server = _mocks.DynamicMock<IServerUtility>();
    	_serverUtility = _railsContext.Server;

      _redirectService = _mocks.CreateMock<IRedirectService>();
      _argumentConversionService = _mocks.CreateMock<IArgumentConversionService>();
      _controller = new TestController();
			
    	
    }

		[TearDown]
		public virtual void Teardown() {
			_virtualDirectory = String.Empty;
		}
    #endregion

    #region Methods
    protected void SetupMocks()
    {
      Expect.Call(_services.ArgumentConversionService).Return(_argumentConversionService).Repeat.Any();
      Expect.Call(_services.RedirectService).Return(_redirectService).Repeat.Any();
      Expect.Call(_services.RailsContext).Return(_railsContext).Repeat.Any();
//      Expect.Call(_railsContext.Server).Return(_serverUtility).Repeat.Any();
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
    #region Test Methods
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NoController_ThrowsException()
    {
      New(null, null, "Action");
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NoAction_ThrowsException()
    {
      New(null, "Controller", null);
    }

    [Test]
    public void Url_NoArea_ReturnsUrl()
    {
      New(null, "Controller", "Action");

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

    [Test]
    public void Redirect_NoArgumentsWithArea_Redirects()
    {
      New("Area", "Controller", "Action");

      using (_mocks.Unordered())
      {
        SetupMocks();
        _redirectService.Redirect("/Area/Controller/Action.rails");
      }

      _mocks.ReplayAll();
      _reference.Redirect();
      _mocks.VerifyAll();
    }

    [Test]
    public void Redirect_Ajax_Works()
    {
      Controller controller = new TestController();
    	controller.ControllerContext = _mocks.Stub<IControllerContext>();
      New("Area", "Controller", "Action");
			
			controller.SetEngineContext(_railsContext);
      using (_mocks.Unordered())
      {
        SetupMocks();
        Expect.Call(_services.Controller).Return(controller).Repeat.Any();
        _response.Write("<script type=\"text/javascript\">window.location = \"/Area/Controller/Action.rails\";</script>");
      }

      _mocks.ReplayAll();
      _reference.Redirect(true);
      _mocks.VerifyAll();
    }

    [Test]
    public void Redirect_NoArgumentsWithoutArea_Redirects()
    {
      New(null, "Controller", "Action");

      using (_mocks.Unordered())
      {
        SetupMocks();
        _redirectService.Redirect("/Controller/Action.rails");
      }

      _mocks.ReplayAll();
      _reference.Redirect();
      _mocks.VerifyAll();
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
        _redirectService.Redirect("/Area/Controller/Action.rails?Ename=EJacob");
      }

      _mocks.ReplayAll();
      _reference.Redirect();
      _mocks.VerifyAll();
    }

    [Test]
    public void Transfer_MethodIsReal_InvokesAction()
    {
      Controller controller = new TestController();
    	controller.ControllerContext = _mocks.Stub<IControllerContext>();
      ActionArgument argument = new ActionArgument(0, "name", "Jacob");
      New("Area", "Controller", "Index", argument);

      using (_mocks.Unordered())
      {
        SetupMocks();
        Expect.Call(_services.Controller).Return(controller).Repeat.Any();
      }

      _mocks.ReplayAll();
      _reference.Transfer();
      _mocks.VerifyAll();
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void Transfer_MethodIsBad_Throws()
    {
      Controller controller = new TestController();
    	controller.ControllerContext = _mocks.Stub<IControllerContext>();
      ActionArgument argument = new ActionArgument(0, "name", "Jacob");
      New("Area", "Controller", "BadAction", argument);

      using (_mocks.Unordered())
      {
        SetupMocks();
        Expect.Call(_services.Controller).Return(controller).Repeat.Any();
      }

      _mocks.ReplayAll();
      _reference.Transfer();
    }
    #endregion

    #region Private Methods
    protected void New(string area, string controller, string action, params ActionArgument[] arguments)
    {
      _reference =
        new ControllerActionReference(_services, typeof(TestController), area, controller, action, null, arguments);
    }
    #endregion
  }

  [TestFixture]
  public class ControllerActionReferenceUnderVirtualDirectoryTests : ControllerReferenceTests
  {
    #region Test Setup and Teardown Methods
    [SetUp]
    public override void Setup()
    {
      _virtualDirectory = "Directory";
      base.Setup();
    }
    #endregion

    #region Test Methods
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

    [Test]
    public void Redirect_NoArgumentsWithArea_Redirects()
    {
      New("Area", "Controller", "Action");

      using (_mocks.Unordered())
      {
        SetupMocks();
        _redirectService.Redirect("/Directory/Area/Controller/Action.rails");
      }

      _mocks.ReplayAll();
      _reference.Redirect();
      _mocks.VerifyAll();
    }
    #endregion

    #region Private Methods
    protected void New(string area, string controller, string action, params ActionArgument[] arguments)
    {
      _reference =
        new ControllerActionReference(_services, typeof(TestController), area, controller, action, null, arguments);
    }
    #endregion
  }

  public class TestController : Controller
  {
    public void Index(string name)
    {
		
    }
  }
}
