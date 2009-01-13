// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Rest
{
	using Castle.Components.Binder;
	using Castle.MonoRail.Framework.Routing;

	public class RoutingAwareControllerBridge : ControllerBridge
	{
		readonly RestfulController controller;

		public RoutingAwareControllerBridge(RestfulController controller, string controllerAction)
			: base(controller, controllerAction)
		{
			this.controller = controller;
		}

		public override string GetFormat()
		{
			return GetParamFromRouteOrRequest("format");
		}

		private string GetParamFromRouteOrRequest(string key)
		{
			var routeMatch = (RouteMatch) controller.Context.Items[RouteMatch.RouteMatchKey];

			if ((routeMatch != null) && routeMatch.Parameters.ContainsKey(key))
				return routeMatch.Parameters[key];

			var node = (LeafNode) controller.Context.Request.ParamsNode.GetChildNode(key);

			return node != null ? node.Value.ToString() : null;
		}
	}
}