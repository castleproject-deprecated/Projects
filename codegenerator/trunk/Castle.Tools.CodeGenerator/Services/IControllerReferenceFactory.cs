using System;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface IControllerReferenceFactory
  {
    ControllerActionReference CreateActionReference(ICodeGeneratorServices services, Type controllerType, string area, string controller, string action, MethodSignature signature, ActionArgument[] arguments);
    ControllerViewReference CreateViewReference(ICodeGeneratorServices services, Type controllerType, string area, string controller, string action);
  }
}
