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
	using System;
	using Framework;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using Components.Binder;
	using Framework.Services;
	using Mime;

	public class RestfulController : SmartDispatcherController
	{
		private string _controllerAction;

		protected override MethodInfo SelectMethod(string action, IDictionary actions, IRequest request,
		                                           IDictionary<string, object> actionArgs, ActionType actionType)
		{
			if (String.Equals("collection", action, StringComparison.InvariantCultureIgnoreCase) || String.IsNullOrEmpty(action))
				return SelectCollectionMethod(action, actions, request, actionArgs, actionType);

			if (String.Equals("new", action, StringComparison.InvariantCultureIgnoreCase))
				return SelectNewMethod(actions);

			return !actions.Contains(action)
					? SelectNonActionMethod(action, actions, request, actionArgs, actionType)
					: base.SelectMethod(action, actions, request, actionArgs, actionType);
		}

		protected MethodInfo SelectCollectionMethod(string action, IDictionary actions, IRequest request,
		                                            IDictionary<string, object> actionArgs, ActionType actionType)
		{
			switch (HttpMethod.ToUpper())
			{
				case "GET":
					_controllerAction = "Index";
					return (MethodInfo) actions["Index"];
				case "POST":
					_controllerAction = "Create";
					return (MethodInfo) actions["Create"];
				default:
					return base.SelectMethod(action, actions, request, actionArgs, actionType);
			}
		}

		private MethodInfo SelectNewMethod(IDictionary actions)
		{
			_controllerAction = "New";
			return (MethodInfo) actions["New"];
		}

		private MethodInfo SelectNonActionMethod(string action, IDictionary actions, IRequest request,
		                                         IDictionary<string, object> actionArgs, ActionType actionType)
		{
			MethodInfo selectedMethod = null;

			switch (HttpMethod.ToUpper())
			{
				case "GET":
					_controllerAction = "Show";
					selectedMethod = (MethodInfo) actions["Show"];
					break;
				case "PUT":
					_controllerAction = "Update";
					selectedMethod = (MethodInfo) actions["Update"];
					break;
				case "DELETE":
					_controllerAction = "Destroy";
					selectedMethod = (MethodInfo) actions["Destroy"];
					break;
				default:
					//Should maybe just throw here.
					return base.SelectMethod(action, actions, request, actionArgs, actionType);
			}

			if (selectedMethod != null)
			{
				LeafNode n = new LeafNode(typeof (String), "ID", action);
				Request.ParamsNode.AddChildNode(n);
			}

			return selectedMethod;
		}

		protected void RespondTo(Action<ResponseFormat> collectFormats)
		{
			MimeTypes registeredMimes = new MimeTypes();
			registeredMimes.RegisterBuiltinTypes();

			ResponseHandler handler = new ResponseHandler()
			{
				ControllerBridge = new ControllerBridge(this, _controllerAction),
				AcceptedMimes = AcceptType.Parse(Request.Headers["Accept"], registeredMimes),
				Format = new ResponseFormat()
			};

			collectFormats(handler.Format);
			handler.Respond();
		}

		protected string UrlFor(IDictionary parameters)
		{
			return Context.Services.UrlBuilder.BuildUrl(this.Context.UrlInfo, parameters);
		}

		protected string UrlFor(string action)
		{
			return Context.Services.UrlBuilder.BuildUrl(Context.UrlInfo, new UrlBuilderParameters(this.Name, action));
		}

		protected virtual string HttpMethod
		{
			get { return Request.HttpMethod; }
		}
	}
}
