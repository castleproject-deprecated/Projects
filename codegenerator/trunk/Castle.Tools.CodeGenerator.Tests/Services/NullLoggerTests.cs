using System;

using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class NullLoggerTests
  {
    #region Member Data
  	private ILogger _logger;
  	#endregion
  	
  	#region Test Setup and Teardown Methods
  	[SetUp]
  	public void Setup()
  	{
      _logger = new NullLogger();
  	}
  	#endregion
  	
  	#region Test Methods
    [Test]
    public void LogInfo_Always_DoesNothing()
    {
      _logger.LogInfo("Hello {0}!", "World");
    }
  	#endregion	
  }
}
