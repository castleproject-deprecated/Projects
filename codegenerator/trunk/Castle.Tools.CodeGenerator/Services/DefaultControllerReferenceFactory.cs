using System;
using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
	public class DefaultControllerReferenceFactory : IControllerReferenceFactory
	{
		public IControllerActionReference CreateActionReference(ICodeGeneratorServices services, Type controllerType,
		                                                        string areaName, string controllerName, string actionName,
		                                                        MethodSignature signature,
		                                                        params ActionArgument[] arguments)
		{
			return new ControllerActionReference(services, controllerType, areaName, controllerName, actionName, signature, arguments);
		}

		public IControllerActionReference CreateRouteReference(ICodeGeneratorServices services, string routeName,
		                                                       ActionArgument[] arguments)
		{
			return new ControllerRouteReference(services, routeName, arguments);
		}

		public IControllerViewReference CreateViewReference(ICodeGeneratorServices services, Type controllerType,
		                                                    string controllerName, string areaName, string actionName)
		{
			return new ControllerViewReference(services, controllerType, controllerName, areaName, actionName);
		}
	}
}
