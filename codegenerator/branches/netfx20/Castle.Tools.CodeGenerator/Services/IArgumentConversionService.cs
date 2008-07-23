using System;
using System.Collections;
using System.Collections.Generic;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface IArgumentConversionService
  {
    IDictionary CreateParameters();
    bool ConvertArgument(MethodSignature signature, ActionArgument argument, IDictionary parameters);
  }
}
