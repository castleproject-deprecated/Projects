using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ILogger=Castle.Tools.CodeGenerator.Services.ILogger;

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
      _helper.LogMessage(MessageImportance.High, message, args);
    }
    #endregion
  }
}
