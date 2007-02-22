using System;
using System.Collections.Generic;
using Castle.MonoRail.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  public class DefaultCodeGeneratorServices : ICodeGeneratorServices
  {
    #region Methods
    private Controller _controller;
    private IControllerReferenceFactory _controllerReferenceFactory;
    private IRedirectService _redirectService;
    private IRailsEngineContext _railsContext;
    private IArgumentConversionService _argumentConversionService;
    #endregion

    #region DefaultCodeGeneratorServices()
    public DefaultCodeGeneratorServices(IControllerReferenceFactory controllerReferenceFactory, IRedirectService redirectService, IArgumentConversionService argumentConversionService)
    {
      _controllerReferenceFactory = controllerReferenceFactory;
      _redirectService = redirectService;
      _argumentConversionService = argumentConversionService;
    }
    #endregion

    #region ICodeGeneratorServices Members
    public Controller Controller
    {
      get { return _controller; }
      set { _controller = value; }
    }

    public IControllerReferenceFactory ControllerReferenceFactory
    {
      get { return _controllerReferenceFactory; }
    }

    public IRedirectService RedirectService
    {
      get { return _redirectService; }
    }

    public IRailsEngineContext RailsContext
    {
      get { return _railsContext; }
      set { _railsContext = value; }
    }

    public IArgumentConversionService ArgumentConversionService
    {
      get { return _argumentConversionService; }
    }
    #endregion
  }
}
