// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Tools.CodeGenerator.Services
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using External;
	using Model;
	using Rhino.Mocks;

	public class MockControllerReferenceFactory : IControllerReferenceFactory
	{
		private readonly object control;
		private readonly Dictionary<string, ControllerActionReference> actions = new Dictionary<string, ControllerActionReference>();
		private readonly Dictionary<string, ControllerViewReference> views = new Dictionary<string, ControllerViewReference>();
		private readonly MockRepository mocks;
		
		public MockControllerReferenceFactory(MockRepository mocks)
		{
			this.mocks = mocks;
			control = mocks.DynamicMock<object>();
		}

		public IControllerActionReference CreateActionReference(
			ICodeGeneratorServices services, Type controllerType, string areaName, string controllerName, string actionName, 
			MethodSignature signature, params ActionArgument[] arguments)
		{
			var key = MakeKey(areaName, controllerName, actionName);
			
			if (!actions.ContainsKey(key))
				actions[key] = ReplayIfNecessary(
					mocks.DynamicMock<ControllerActionReference>(services, controllerType, areaName, controllerName, actionName, signature, arguments));

			return actions[key];
		}

		public IControllerViewReference CreateViewReference(ICodeGeneratorServices services, Type controllerType,
		                                                    string areaName, string controllerName, string actionName)
		{
			var key = MakeKey(areaName, controllerName, actionName);

			if (!views.ContainsKey(key))
				views[key] = ReplayIfNecessary(mocks.DynamicMock<ControllerViewReference>(services, controllerType, areaName, controllerName, actionName));
			
			return views[key];
		}

		private T ReplayIfNecessary<T>(T mock)
		{
			if (mocks.IsInReplayMode(control))
				mocks.Replay(mock);
			
			return mock;
		}

		private static string MakeKey(string areaName, string controllerName, string actionName)
		{
			var sb = new StringBuilder();
			sb.Append(controllerName).Append("/");
			
			if (!String.IsNullOrEmpty(areaName))
				sb.Append(areaName).Append("/");
			
			sb.Append(actionName);
			
			return sb.ToString();
		}
	}
}