using System;
using System.Collections.Generic;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class DefaultControllerReferenceFactory : IControllerReferenceFactory
  {
    #region Methods
    public ControllerActionReference CreateActionReference(ICodeGeneratorServices services, Type controllerType,
														   string areaName, string controllerName, string actionName,
                                                           MethodSignature signature,
                                                           params ActionArgument[] arguments)
    {
      return new ControllerActionReference(services, controllerType,areaName, controllerName , actionName, signature, arguments);
    }

    public ControllerViewReference CreateViewReference(ICodeGeneratorServices services, Type controllerType,
                                                       string controllerName, string areaName, string actionName)
    {
      return new ControllerViewReference(services, controllerType, controllerName, areaName, actionName);
    }
    #endregion
  }
}
