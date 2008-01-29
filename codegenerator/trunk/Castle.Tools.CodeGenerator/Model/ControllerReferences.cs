using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Routing;
using Castle.MonoRail.Framework.Services;
using Castle.Tools.CodeGenerator.Services;

namespace Castle.Tools.CodeGenerator.Model
{
	public class ActionArgument
	{
		#region Member Data

		private int _index;
		private Type _type;
		private string _name;
		private object _value;

		#endregion

		#region Properties

		public Type Type
		{
			get { return _type; }
		}

		public int Index
		{
			get { return _index; }
		}

		public string Name
		{
			get { return _name; }
		}

		public object Value
		{
			get { return _value; }
		}

		#endregion

		#region ActionArgument()

		public ActionArgument(int index, string name, object value)
			: this(index, name, value.GetType(), value)
		{
		}

		public ActionArgument(int index, string name, Type type, object value)
		{
			_index = index;
			_name = name;
			_type = type;
			_value = value;
		}

		#endregion
	}

	public class ControllerViewReference : IControllerViewReference
	{
		#region Member Data

		private ICodeGeneratorServices _services;
		private Type _controllerType;
		private string _controllerName;
		private string _areaName;
		private string _actionName;

		#endregion

		#region Properties

		public ICodeGeneratorServices Services
		{
			get { return _services; }
		}

		public Type ControllerType
		{
			get { return _controllerType; }
		}

		public string ControllerName
		{
			get { return _controllerName; }
		}

		public string AreaName
		{
			get { return _areaName; }
		}

		public string ActionName
		{
			get { return _actionName; }
		}

		#endregion

		#region ControllerViewReference()

		public ControllerViewReference(ICodeGeneratorServices services, Type controllerType, string areaName,
		                               string controllerName, string actionName)
		{
			if (services == null) throw new ArgumentNullException("services");
			if (controllerType == null) throw new ArgumentNullException("controllerType");
			if (String.IsNullOrEmpty(controllerName)) throw new ArgumentNullException("controllerName");
			if (String.IsNullOrEmpty(actionName)) throw new ArgumentNullException("actionName");
			_services = services;
			_controllerType = controllerType;
			_controllerName = controllerName;
			_areaName = areaName;
			_actionName = actionName;
		}

		#endregion

		#region Methods

		public virtual void Render(bool skiplayout)
		{
			string controller = _controllerName;
			if (!String.IsNullOrEmpty(_areaName))
				controller = Path.Combine(_areaName, _controllerName);
			_services.Controller.RenderView(controller, _actionName, skiplayout);
		}

		public virtual void Render()
		{
			Render(false);
		}

		#endregion
	}

	public class ControllerActionReference : ControllerViewReference, IControllerActionReference
	{
		#region Member Data

		protected ActionArgument[] _arguments;
		protected MethodSignature _signature;

		#endregion

		#region Properties

		public ActionArgument[] Arguments
		{
			get { return _arguments; }
		}

		public MethodSignature ActionMethodSignature
		{
			get { return _signature; }
		}

		#endregion

		#region ControllerActionReference()

		public ControllerActionReference(ICodeGeneratorServices services, Type controllerType, string areaName,
		                                 string controllerName, string actionName, MethodSignature signature,
		                                 params ActionArgument[] arguments)
			: base(services, controllerType, areaName, controllerName, actionName)
		{
			_arguments = arguments;
			_signature = signature;
		}

		#endregion

		#region Methods

		public virtual void Transfer()
		{
			this.Services.Controller.CancelView();
			this.Render();

			List<Type> types = new List<Type>();
			List<object> arguments = new List<object>();

			foreach (ActionArgument argument in _arguments)
			{
				arguments.Add(argument.Value);
				types.Add(argument.Type);
			}

			MethodInfo method = this.ControllerType.GetMethod(this.ActionName, types.ToArray());
			if (method == null)
			{
				throw new ArgumentException("Transfer failed, no method named: " + this.ActionName);
			}
			method.Invoke(this.Services.Controller, arguments.ToArray());
		}

		public virtual string Url
		{
			get
			{
				IUrlBuilder urlBuilder = this.Services.RailsContext.Services.UrlBuilder;
				urlBuilder.UseExtensions = true;

				if (urlBuilder is DefaultUrlBuilder)
					((DefaultUrlBuilder) urlBuilder).ServerUtil = this.Services.RailsContext.Server;

				IDictionary parameters = new Hashtable();
				IArgumentConversionService conversionService = this.Services.ArgumentConversionService;
				int index = 0;
				foreach (ActionArgument argument in this.Arguments)
				{
					parameters.Add(conversionService.ConvertKey(this.ActionMethodSignature, argument),
					               conversionService.ConvertArgument(this.ActionMethodSignature, argument));
					index++;
				}
				UrlInfo urlInfo = this.Services.RailsContext.UrlInfo;
				UrlBuilderParameters urlBuilderParameters = new UrlBuilderParameters(this.AreaName, this.ControllerName, this.ActionName);
				urlBuilderParameters.QueryString = parameters;
				return urlBuilder.BuildUrl(urlInfo, urlBuilderParameters);
			}
		}

		public virtual void Redirect(bool useJavascript)
		{
			if (useJavascript)
			{
				string type = "text/javascript";
				string javascript = String.Format("<script type=\"{0}\">window.location = \"{1}\";</script>", type, this.Url);
				this.Services.Controller.CancelView();
				this.Services.Controller.RenderText(javascript);
			}
			else
			{
				this.Services.RailsContext.Response.RedirectToUrl(this.Url, false);
			}
		}

		public virtual void Redirect()
		{
			Redirect(false);
		}

		#endregion
	}
}
