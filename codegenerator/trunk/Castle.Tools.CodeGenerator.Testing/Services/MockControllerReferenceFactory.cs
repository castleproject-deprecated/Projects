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
		private readonly MockRepository _mocks;
		private readonly object _control;
		private Dictionary<string, ControllerViewReference> _views = new Dictionary<string, ControllerViewReference>();

		public MockControllerReferenceFactory(MockRepository mocks)
		{
			_mocks = mocks;
			_control = mocks.CreateMock<object>();
		}

		public IControllerActionReference CreateActionReference(ICodeGeneratorServices services, Type controllerType,
		                                                        string areaName, string controllerName, string actionName,
		                                                        MethodSignature signature, params ActionArgument[] arguments)
		{
			string key = MakeKey(areaName, controllerName, actionName);
			if (!_actions.ContainsKey(key))
			{
				_actions[key] =
					ReplayIfNecessary(_mocks.DynamicMock<ControllerActionReference>(services, controllerType, areaName, controllerName, actionName,
					                                             signature, arguments));
			}
			return _actions[key];
		}

		public IControllerViewReference CreateViewReference(ICodeGeneratorServices services, Type controllerType,
		                                                    string areaName, string controllerName, string actionName)
		{
			string key = MakeKey(areaName, controllerName, actionName);
			if (!_views.ContainsKey(key))
			{
				_views[key] =
					ReplayIfNecessary(_mocks.DynamicMock<ControllerViewReference>(services, controllerType, areaName, controllerName, actionName));
			}
			return _views[key];
		}

		private T ReplayIfNecessary<T>(T mock)
		{
			if (_mocks.IsInReplayMode(_control))
			{
				_mocks.Replay(mock);
			}

			return mock;
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
