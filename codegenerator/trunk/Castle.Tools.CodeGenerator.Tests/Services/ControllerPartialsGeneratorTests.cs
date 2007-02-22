using System;
using System.Collections.Generic;
using Castle.Tools.CodeGenerator.Model;
using Rhino.Mocks;
using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  public class ControllerPartialsGeneratorTests
  {
    #region Member Data
  	protected MockRepository _mocks;
    protected ILogger _logging;
    protected INamingService _naming;
    protected ISourceGenerator _source;
    protected ControllerPartialsGenerator _generator;
  	#endregion
  	
  	#region Test Setup and Teardown Methods
  	[SetUp]
  	public virtual void Setup()
  	{
  		_mocks = new MockRepository();
      _naming = new DefaultNamingService();
      _source = new DefaultSourceGenerator(); // I found a more integration style of testing was better, I started off
      // mocking calls to ISourceGenerator, and that was just stupid, we want the classes and types and members.
      // and the assertions here ensure that.
      _logging = new NullLogger();
      _generator = new ControllerPartialsGenerator(_logging, _source, _naming, "TargetNamespace", typeof(IServiceProvider).FullName);
  	}
  	#endregion
  }

  [TestFixture]
  public class ControllerPartialsGeneratorControllerTests : ControllerPartialsGeneratorTests
  {
    #region Test Methods
    [Test]
    public void VisitControllerNode_Always_CreatesType()
    {
      ControllerTreeNode node = new ControllerTreeNode("HomeController", "ControllerNamespace");

      _mocks.ReplayAll();
      _generator.Visit(node);
      _mocks.VerifyAll();

      CodeDomAssert.AssertHasProperty(_source.Ccu.Namespaces[0].Types[0], "MyActions");
      CodeDomAssert.AssertHasProperty(_source.Ccu.Namespaces[0].Types[0], "MyViews");
      CodeDomAssert.AssertHasMethod(_source.Ccu.Namespaces[0].Types[0], "PerformGeneratedInitialize");
    }
    #endregion
  }
}
