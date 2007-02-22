using System;
using System.Collections.Generic;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface IArgumentConversionService
  {
    object ConvertArgument(string name, object value);
  }
}
