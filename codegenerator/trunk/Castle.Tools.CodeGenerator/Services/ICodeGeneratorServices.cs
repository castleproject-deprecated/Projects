using System;

using Castle.MonoRail.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface ICodeGeneratorServices
  {
    Controller Controller
    {
      set;
      get;
    }

    IControllerReferenceFactory ControllerReferenceFactory
    {
      get;
    }

    IRedirectService RedirectService
    {
      get;
    }
	  
    IEngineContext RailsContext
    {
      get;
      set;
    }

    IArgumentConversionService ArgumentConversionService
    {
      get;
    }

    IRuntimeInformationService RuntimeInformationService
    {
      get;
    }
  }
}