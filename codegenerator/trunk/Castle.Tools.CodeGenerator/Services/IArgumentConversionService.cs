using System;
using System.Collections.Generic;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface IArgumentConversionService
  {
    object ConvertArgument(MethodSignature signature, ActionArgument argument);
  }
}
