using System;
using System.Collections.Generic;

using Microsoft.Build.Utilities;

using Castle.Tools.CodeGenerator.Services;

namespace Castle.Tools.CodeGenerator.MsBuild
{
  public class MsBuildLogger : ILogger
  {
    #region Member Data
    private TaskLoggingHelper _helper;
    #endregion

    #region MsBuildLogger()
    public MsBuildLogger(TaskLoggingHelper helper)
    {
      _helper = helper;
    }
    #endregion

    #region ILogger Members
    public void LogInfo(string message, params object[] args)
    {
      _helper.LogMessage(message, args);
    }
    #endregion
  }
}
