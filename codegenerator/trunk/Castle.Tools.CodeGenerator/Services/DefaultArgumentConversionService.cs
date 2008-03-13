using System;
using System.Collections;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class DefaultArgumentConversionService : IArgumentConversionService
  {
    #region IArgumentConversionService Members
    public IDictionary CreateParameters()
    {
      return new Hashtable();
    }

    public bool ConvertArgument(MethodSignature signature, ActionArgument argument, IDictionary parameters)
    {
      parameters.Add(argument.Name, argument.Value);
      return true;
    }
  	#endregion
  }
}
