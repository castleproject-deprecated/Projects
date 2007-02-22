using System;
using System.Collections.Generic;

namespace Castle.Tools.CodeGenerator.Services
{
  public class DefaultArgumentConversionService : IArgumentConversionService
  {
    #region IArgumentConversionService Members
    public object ConvertArgument(string name, object value)
    {
      return value;
    }
    #endregion
  }
}
