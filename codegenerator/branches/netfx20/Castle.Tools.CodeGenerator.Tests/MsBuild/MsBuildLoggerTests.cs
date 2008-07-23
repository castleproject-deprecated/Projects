using System;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Rhino.Mocks;
using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.MsBuild
{
  [TestFixture]
  public class MsBuildLoggerTests
  {
    #region Member Data
  	private MockRepository _mocks;
    private MsBuildLogger _logger;
    private ITask _task;
    private IBuildEngine _engine;
  	#endregion
  	
  	#region Test Setup and Teardown Methods
  	[SetUp]
  	public void Setup()
  	{
  		_mocks = new MockRepository();
      _task = _mocks.DynamicMock<ITask>();
      _engine = _mocks.DynamicMock<IBuildEngine>();
      _logger = new MsBuildLogger(new TaskLoggingHelper(_task));
  	}
  	#endregion
  	
  	#region Test Methods
    [Test]
    public void LogInfo_Always_Works()
    {
      using (_mocks.Unordered())
      {
        Expect.Call(_task.BuildEngine).Return(_engine).Repeat.Any();
        _engine.LogMessageEvent(null);
        LastCall.IgnoreArguments();
      }

      _mocks.ReplayAll();
      _logger.LogInfo("Hello {0}!", "World");
      _mocks.VerifyAll();
    }
  	#endregion	
  }
}
