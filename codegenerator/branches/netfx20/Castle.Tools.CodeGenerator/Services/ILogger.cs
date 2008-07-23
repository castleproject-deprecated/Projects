using System;
using System.Collections.Generic;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface ILogger
  {
    void LogInfo(string message, params object[] args);
  }
}
