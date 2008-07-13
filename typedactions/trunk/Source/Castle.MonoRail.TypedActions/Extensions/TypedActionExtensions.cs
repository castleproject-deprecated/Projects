#region Licence

// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections;
using System.Linq.Expressions;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.TypedActions;

namespace Castle.MonoRail.Framework.Extensions
{
	public static class TypedActionExtensions
	{
		public static bool DefaultUseRouting
		{
			get;
			set;
		}

		#region Extension methods

		public static void Redirect<TController>(
			this IRedirectSupport self, Expression<Action<TController>> action)
			where TController : IController
		{
			Redirect(self, DefaultUseRouting, action);
		}

		/// <summary>
		/// Performs a strongly typed redirect to an action of the <typeparamref name="TController"/>.
		/// <para /> 
		/// This must be a lamda in the form of <c>c => c.Action(optional, action, parameters)</c>, 
		/// where the action parameters can be anything you want (variable, property, method call etc.) 
		/// as long as they return the right parameter and the lambda function will compile.
		/// </summary>
		/// <typeparam name="TController">The type of the controller that has the action.</typeparam>
		/// <param name="self">The object that supports redirecting.</param>
		/// <param name="useRouting">If true, uses routing.</param>
		/// <param name="action">An expression that respresents the redirect in the form of a lambda function call.</param>
		public static void Redirect<TController>(
			this IRedirectSupport self, bool useRouting, Expression<Action<TController>> action)
			where TController : IController
		{
			AssertArguments(self, action);

			ActionMethodModel model = CreateModel(action);

			var redirecter = new ResponseRedirectInvoker
			{
				UseRouting = useRouting
			};
			redirecter.Redirect(self, model);
		}


		public static void RedirectToAction<TController>(
			this TController self, Expression<Action<TController>> action)
			where TController : Controller
		{
			RedirectToAction(self, DefaultUseRouting, action);
		}

		public static void RedirectToAction<TController>(
			this TController self, bool useRouting, Expression<Action<TController>> action)
			where TController : Controller
		{
			AssertArguments(self, action);

			ActionMethodModel model = CreateModel(action);

			IRedirectInvoker redirecter = new SameControllerRedirectInvoker(self)
			{
				UseRouting = useRouting
			};
			redirecter.Redirect(self, model);
		}


		public static string TypedFor<TController>(
			this UrlHelper self, Expression<Action<TController>> action)
			where TController : IController
		{
			AssertArguments(self, action);

			ActionMethodModel model = CreateModel(action);
			IDictionary urlParams = BuildUrlParameters(self, model);
			return self.For(urlParams);
		}


		/// <summary>
		/// Like Asp.net MVC HtmlHelper.ActionLink
		/// <para />
		/// You should be able to use this in AspView.
		/// </summary>
		/// <typeparam name="TController">The type of the controller.</typeparam>
		/// <param name="self">The self.</param>
		/// <param name="innerContent">Content of the inner.</param>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		public static string TypedLink<TController>(
			this UrlHelper self, string innerContent, Expression<Action<TController>> action)
			where TController : IController
		{
			return TypedLink(self, innerContent, action, null);
		}

		public static string TypedLink<TController>(
			this UrlHelper self, string innerContent, Expression<Action<TController>> action,
			IDictionary anchorAttributes)
			where TController : IController
		{
			AssertArguments(self, action);

			ActionMethodModel model = CreateModel(action);
			IDictionary urlParams = BuildUrlParameters(self, model);

			return self.Link(innerContent, urlParams, anchorAttributes);
		}

		#endregion

		private static void AssertArguments<TSelf, TAction>(TSelf self, TAction action)
		{
			if (self == null)
			{
				throw new ArgumentNullException("self");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
		}

		private static ActionMethodModel CreateModel<TController>(Expression<Action<TController>> action)
			where TController : IController
		{
			// TODO: Windsor integration?
			IActionParametersBuilder paramBuilder =
				new ExcludeNullOrEmptyParametersBuilder(
					new ARAwareActionParametersBuilder(
						new DefaultActionParametersBuilder()));
			IActionMethodBuilder actionBuilder =
				new DefaultActionMethodBuilder(paramBuilder);

			ActionMethodModel model = actionBuilder.BuildAction(action);
			return model;
		}

		private static IDictionary BuildUrlParameters(UrlHelper urlHelper, ActionMethodModel actionModel)
		{
			var urlParams = DictHelper
				.CreateN("Area", actionModel.Descriptor.Area)
				.N("Controller", actionModel.Descriptor.Name)
				.N("Action", actionModel.MethodInfo.Name);

			var queryParams = actionModel.GetParameterDictionary();

			if (queryParams != null && queryParams.Count > 0)
			{
				string queryString = urlHelper.BuildQueryString(queryParams);
				if (!string.IsNullOrEmpty(queryString))
				{
					urlParams.Add("QueryString", queryString);
				}

				// What was i thinking when i wrote this...?
				foreach (var queryKey in queryParams.Keys)
				{
					if (!urlParams.Contains(queryKey))
					{
						urlParams.Add(queryKey, queryParams[queryKey]);
					}
				}
			}

			return urlParams;
		}
	}
}