using System;
using System.Collections.Generic;
using Castle.MonoRail.Framework;

using Castle.Tools.CodeGenerator.Model;

using Rhino.Mocks;
using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class DefaultCodeGeneratorServicesTests
  {
    #region Member Data
    private MockRepository _mocks;
    private DefaultCodeGeneratorServices _services;
    private IRedirectService _redirectService;
    private IControllerReferenceFactory _controllerReferenceFactory;
    private IArgumentConversionService _argumentConversionService;
    private IEngineContext _context;
    private IRuntimeInformationService _runtimeInformationService;
    private TestController _controller;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _controller = new TestController();
      _mocks = new MockRepository();
      _controllerReferenceFactory = _mocks.CreateMock<IControllerReferenceFactory>();
      _redirectService = _mocks.CreateMock<IRedirectService>();
      _argumentConversionService = _mocks.CreateMock<IArgumentConversionService>();
      _runtimeInformationService = _mocks.CreateMock<IRuntimeInformationService>();
      _services = new DefaultCodeGeneratorServices(_controllerReferenceFactory, _redirectService, _argumentConversionService, _runtimeInformationService);
      _context = _mocks.CreateMock<IEngineContext>();
      Assert.AreEqual(_controllerReferenceFactory, _services.ControllerReferenceFactory);
      Assert.AreEqual(_redirectService, _services.RedirectService);
      Assert.AreEqual(_argumentConversionService, _services.ArgumentConversionService);
      Assert.AreEqual(_runtimeInformationService, _services.RuntimeInformationService);
    }
    #endregion

    #region Test Methods
    [Test]
    public void GetAndSetController_Always_Work()
    {
      Assert.IsNull(_services.Controller);
      _services.Controller = _controller;
      Assert.AreEqual(_controller, _services.Controller);
    }

    [Test]
    public void GetAndSetRailsContext_Always_Works()
    {
      Assert.IsNull(_services.RailsContext);
      _services.RailsContext = _context;
      Assert.AreEqual(_context, _services.RailsContext);
    }
    #endregion
  }
}
