using System;
using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
	public interface IControllerReferenceFactory
	{
		IControllerActionReference CreateActionReference(ICodeGeneratorServices services, Type controllerType, string area,
		                                                 string controller, string action, MethodSignature signature,
		                                                 ActionArgument[] arguments);

		IControllerActionReference CreateRouteReference(ICodeGeneratorServices services, string routeName, ActionArgument[] arguments);

		IControllerViewReference CreateViewReference(ICodeGeneratorServices services, Type controllerType, string area,
		                                             string controller, string action);
	}
}
