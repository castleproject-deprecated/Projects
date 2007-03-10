using System;
using System.Collections.Generic;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class DefaultArgumentConversionService : IArgumentConversionService
  {
    #region IArgumentConversionService Members
    public object ConvertArgument(MethodSignature signature, ActionArgument argument)
    {
      return argument.Value;
    }
    #endregion
  }
}
