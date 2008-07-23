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

namespace Castle.Tools.CodeGenerator.External
{
	using System;
	using System.Collections.Generic;
	using MonoRail.Framework.Services;
	using External;

	public class ControllerActionReference : ControllerViewReference, IControllerActionReference
	{
		public ControllerActionReference(ICodeGeneratorServices services, Type controllerType, string areaName,
										 string controllerName, string actionName, MethodSignature signature,
										 params ActionArgument[] arguments)
			: base(services, controllerType, areaName, controllerName, actionName)
		{
			Arguments = arguments;
			ActionMethodSignature = signature;
		}

		public ActionArgument[] Arguments { get; protected set; }
		public MethodSignature ActionMethodSignature { get; protected set; }

		public virtual void Transfer()
		{
			Services.Controller.CancelView();
			Render();

			var types = new List<Type>();
			var arguments = new List<object>();

			foreach (var argument in Arguments)
			{
				arguments.Add(argument.Value);
				types.Add(argument.Type);
			}

			var method = ControllerType.GetMethod(ActionName, types.ToArray());
			
			if (method == null)
				throw new ArgumentException("Transfer failed, no method named: " + ActionName);
			
			method.Invoke(Services.Controller, arguments.ToArray());
		}

		public virtual string Url
		{
			get
			{
				var urlBuilder = Services.RailsContext.Services.UrlBuilder;
				urlBuilder.UseExtensions = true;

				if (urlBuilder is DefaultUrlBuilder)
					((DefaultUrlBuilder) urlBuilder).ServerUtil = Services.RailsContext.Server;
				
				var conversionService = Services.ArgumentConversionService;
				var parameters = conversionService.CreateParameters();
				
				foreach (var argument in Arguments)
					conversionService.ConvertArgument(ActionMethodSignature, argument, parameters);
				
				var urlInfo = Services.RailsContext.UrlInfo;
				var urlBuilderParameters = new UrlBuilderParameters(AreaName, ControllerName, ActionName) {QueryString = parameters};
				
				return urlBuilder.BuildUrl(urlInfo, urlBuilderParameters);
			}
		}

		public virtual void Redirect(bool useJavascript)
		{
			if (useJavascript)
			{
				var type = "text/javascript";
				var javascript = String.Format("<script type=\"{0}\">window.location = \"{1}\";</script>", type, Url);

				Services.Controller.CancelView();
				Services.Controller.RenderText(javascript);
			}
			else
			{
				Services.RailsContext.Response.RedirectToUrl(Url, false);
			}
		}

		public virtual void Redirect()
		{
			Redirect(false);
		}
	}
}