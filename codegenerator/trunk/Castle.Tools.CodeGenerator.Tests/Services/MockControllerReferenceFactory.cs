using System;
using System.Collections.Generic;
using System.Text;
using Castle.Tools.CodeGenerator.Model;
using Rhino.Mocks;

namespace Castle.Tools.CodeGenerator.Services
{
	public class MockControllerReferenceFactory : IControllerReferenceFactory
	{
		private Dictionary<string, ControllerActionReference> _actions = new Dictionary<string, ControllerActionReference>();
		private MockRepository _mocks;
		private Dictionary<string, ControllerViewReference> _views = new Dictionary<string, ControllerViewReference>();

		public MockControllerReferenceFactory(MockRepository mocks)
		{
			_mocks = mocks;
		}

		public IControllerActionReference CreateActionReference(ICodeGeneratorServices services, Type controllerType,
		                                                        string areaName, string controllerName, string actionName,
		                                                        MethodSignature signature, params ActionArgument[] arguments)
		{
			string key = MakeKey(areaName, controllerName, actionName);
			if (!_actions.ContainsKey(key))
			{
				_actions[key] =
					_mocks.CreateMock<ControllerActionReference>(services, controllerType, areaName, controllerName, actionName,
					                                             signature, arguments);
			}
			return _actions[key];
		}

		public IControllerViewReference CreateViewReference(ICodeGeneratorServices services, Type controllerType,
		                                                    string controllerName, string areaName, string actionName)
		{
			string key = MakeKey(areaName, controllerName, actionName);
			if (!_views.ContainsKey(key))
			{
				_views[key] =
					_mocks.CreateMock<ControllerViewReference>(services, controllerType, controllerName, areaName, actionName);
			}
			return _views[key];
		}

		private static string MakeKey(string areaName, string controllerName, string actionName)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(controllerName).Append("/");
			if (!String.IsNullOrEmpty(areaName))
			{
				sb.Append(areaName).Append("/");
			}
			sb.Append(actionName);
			return sb.ToString();
		}
	}
}
