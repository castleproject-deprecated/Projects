using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Services;
using Castle.Tools.CodeGenerator.Services;

namespace Castle.Tools.CodeGenerator.Model
{
  public class ActionArgument
  {
    #region Member Data
    private Type _type;
    private string _name;
    private object _value;
    #endregion

    #region Properties
    public Type Type
    {
      get { return _type; }
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
    public ActionArgument(string name, object value)
      : this(name, value.GetType(), value)
    {
    }

    public ActionArgument(string name, Type type, object value)
    {
      _name = name;
      _type = type;
      _value = value;
    }
    #endregion
  }

  public class ControllerViewReference
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
    public ControllerViewReference(ICodeGeneratorServices services, Type controllerType, string areaName, string controllerName, string actionName)
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

  public class ControllerActionReference : ControllerViewReference
  {
    #region Member Data
    private ActionArgument[] _arguments;
    #endregion

    #region Properties
    public ActionArgument[] Arguments
    {
      get { return _arguments; }
    }
    #endregion

    #region ControllerActionReference()
    public ControllerActionReference(ICodeGeneratorServices services, Type controllerType, string areaName, string controllerName, string actionName, params ActionArgument[] arguments)
      : base(services, controllerType, areaName, controllerName, actionName)
    {
      _arguments = arguments;
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
        DefaultUrlBuilder urlBuilder = new DefaultUrlBuilder();
        urlBuilder.ServerUtil = this.Services.RailsContext.Server;
        urlBuilder.UseExtensions = true;
        IDictionary parameters = new Hashtable();
        IArgumentConversionService conversionService = this.Services.ArgumentConversionService;
        foreach (ActionArgument argument in this.Arguments)
        {
          parameters.Add(argument.Name, conversionService.ConvertArgument(argument.Name, argument.Value));
        }
        UrlInfo urlInfo = this.Services.RailsContext.UrlInfo;
        return urlBuilder.BuildUrl(urlInfo, this.AreaName, this.ControllerName, this.ActionName, parameters);
      }
    }

    public virtual void Redirect()
    {
      this.Services.RedirectService.Redirect(this.Url);
    }
    #endregion
  }
}
