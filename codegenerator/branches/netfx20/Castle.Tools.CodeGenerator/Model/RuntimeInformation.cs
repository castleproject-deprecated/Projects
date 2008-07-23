using System;
using System.Collections.Generic;
using System.Reflection;

namespace Castle.Tools.CodeGenerator.Model
{
  public class CustomAttributesContainer
  {
    #region Member Data
    private object[] _customAttributes;
    #endregion

    #region Properties
    public object[] CustomAttributes
    {
      get { return _customAttributes; }
    }
    #endregion

    #region CustomAttributesContainer()
    public CustomAttributesContainer(object[] customAttributes)
    {
      _customAttributes = customAttributes;
    }
    #endregion

    #region Methods
    public object[] GetCustomAttributes(Type type)
    {
      List<object> attributes = new List<object>();
      foreach (object attribute in _customAttributes)
      {
        if (type.IsInstanceOfType(attribute))
        {
          attributes.Add(attribute);
        }
      }
      return attributes.ToArray();
    }
    #endregion
  }

  public class MethodInformation : CustomAttributesContainer
  {
    #region Member Data
    private MethodInfo _method;
    private ParameterInformation[] _parameters;
    #endregion

    #region Properties
    public MethodInfo Method
    {
      get { return _method; }
    }

    public ParameterInformation[] Parameters
    {
      get { return _parameters; }
    }
    #endregion

    #region MethodInformation()
    public MethodInformation(MethodInfo method, ParameterInformation[] parameters, object[] customAttributes)
      : base(customAttributes)
    {
      _method = method;
      _parameters = parameters;
    }
    #endregion
  }

  public class ParameterInformation : CustomAttributesContainer
  {
    #region Member Data
    private ParameterInfo _parameter;
    #endregion

    #region Properties
    public ParameterInfo Parameter
    {
      get { return _parameter; }
    }
    #endregion

    #region ParameterInformation()
    public ParameterInformation(ParameterInfo parameter, object[] customAttributes)
      : base(customAttributes)
    {
      _parameter = parameter;
    }
    #endregion
  }

}
