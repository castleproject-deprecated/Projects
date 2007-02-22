using System;
using System.Collections.Generic;

namespace Castle.Tools.CodeGenerator.Services
{
  public class NullLogger : ILogger
  {
    #region ILogger Members
    public void LogInfo(string message, params object[] args)
    {
    }
    #endregion
  }
}
