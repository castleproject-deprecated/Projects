using System;
using System.Collections.Generic;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface IRuntimeInformationService
  {
    MethodInformation ResolveMethodInformation(MethodSignature signature);
    MethodInformation ResolveMethodInformation(Type type, string name, Type[] types);
  }
}