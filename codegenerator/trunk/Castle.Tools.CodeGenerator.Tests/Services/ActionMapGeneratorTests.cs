using System;
using System.CodeDom;
using System.Collections.Generic;

using Rhino.Mocks;
using NUnit.Framework;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class ActionMapGeneratorTests
  {
    #region Member Data
  	protected MockRepository _mocks;
    protected ILogger _logging;
    protected INamingService _naming;
    protected ISourceGenerator _source;
    protected ActionMapGenerator _generator;
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
      _generator = new ActionMapGenerator(_logging, _source, _naming, "TargetNamespace", typeof(IServiceProvider).FullName);
  	}
  	#endregion
  }

  [TestFixture]
  public class ActionMapGeneratorControllerTests : ActionMapGeneratorTests
  {
    #region Test Methods
    [Test]
    public void VisitControllerNode_Always_CreatesType()
    {
      ControllerTreeNode node = new ControllerTreeNode("HomeController", "ControllerNamespace");

      _mocks.ReplayAll();
      _generator.Visit(node);
      _mocks.VerifyAll();

      CodeDomAssert.AssertHasField(_source.Ccu.Namespaces[0].Types[0], "_services");
    }
    #endregion
  }

  [TestFixture]
  public class ActionMapGeneratorActionTests : ActionMapGeneratorTests
  {
    #region Member Data
    private AreaTreeNode _root;
    private ControllerTreeNode _controller;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      base.Setup();
      _root = new AreaTreeNode("Root");
      _controller = new ControllerTreeNode("HomeController", "ControllerNamespace");
      _root.AddChild(_controller);
    }
    #endregion

    #region Test Methods
    [Test]
    public void VisitActionNode_NoParameters_CreatesMethod()
    {
      ActionTreeNode node = new ActionTreeNode("Index");
      _controller.AddChild(node);

      _mocks.ReplayAll();
      _generator.Visit(_controller);
      _mocks.VerifyAll();

      CodeDomAssert.AssertHasField(_source.Ccu.Namespaces[0].Types[0], "_services");
      CodeDomAssert.AssertHasMethod(_source.Ccu.Namespaces[0].Types[0], "Index");
    }

    [Test]
    public void VisitActionNode_OneParameters_CreatesMethod()
    {
      ActionTreeNode node = new ActionTreeNode("Index");
      _controller.AddChild(node);
      node.AddChild(new ParameterTreeNode("id", "System.Int32"));

      using (_mocks.Unordered())
      {
      }

      _mocks.ReplayAll();
      _generator.Visit(_controller);
      _mocks.VerifyAll();

      CodeDomAssert.AssertHasField(_source.Ccu.Namespaces[0].Types[0], "_services");
      CodeDomAssert.AssertHasMethod(_source.Ccu.Namespaces[0].Types[0], "Index");
    }
    #endregion
  }
}
