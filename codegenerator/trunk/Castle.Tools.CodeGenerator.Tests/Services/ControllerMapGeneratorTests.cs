using System;
using System.CodeDom;
using System.Collections.Generic;

using Rhino.Mocks;
using NUnit.Framework;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class ControllerMapGeneratorTests
  {
    #region Member Data
  	protected MockRepository _mocks;
    protected ILogger _logging;
    protected INamingService _naming;
    protected ISourceGenerator _source;
    protected ControllerMapGenerator _generator;
  	#endregion
  	
  	#region Test Setup and Teardown Methods
  	[SetUp]
  	public virtual void Setup()
  	{
  		_mocks = new MockRepository();
      _naming = new DefaultNamingService();
      _source = new DefaultSourceGenerator();
      _logging = new NullLogger();
      _generator = new ControllerMapGenerator(_logging, _source, _naming, "TargetNamespace", typeof(IServiceProvider).FullName);
  	}
  	#endregion
  }

  [TestFixture]
  public class ControllerMapGeneratorNotInAreaTests : ControllerMapGeneratorTests
  {
    #region Test Methods
    [Test]
    public void VisitAreaNode_NoParent_CreatesType()
    {
      AreaTreeNode node = new AreaTreeNode("Root");

      _mocks.ReplayAll();
      _generator.Visit(node);
      _mocks.VerifyAll();

      CodeDomAssert.AssertHasField(_source.Ccu.Namespaces[0].Types[0], "_services");
    }
    #endregion
  }

  [TestFixture]
  public class ControllerMapGeneratorInAreaTests : ControllerMapGeneratorTests
  {
    #region Member Data
    private AreaTreeNode _root;
    private CodeTypeDeclaration _rootType;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      base.Setup();
      _root = new AreaTreeNode("Root");
      _rootType = new CodeTypeDeclaration();
    }
    #endregion

    #region Test Methods
    [Test]
    public void VisitAreaNode_AreaAsParent_CreatesType()
    {
      AreaTreeNode node = new AreaTreeNode("Area");
      _root.AddChild(node);

      _mocks.ReplayAll();
      _generator.Visit(_root);
      _mocks.VerifyAll();

      CodeDomAssert.AssertHasField(_source.Ccu.Namespaces[0].Types[0], "_services");
    }

    [Test]
    public void VisitControllerTreeNode_Always_CreatesControllerType()
    {
      ControllerTreeNode node = new ControllerTreeNode("HomeController", "ControllerNamespace");
      _root.AddChild(node);

      _mocks.ReplayAll();
      _generator.Visit(_root);
      _mocks.VerifyAll();

      CodeTypeDeclaration type = CodeDomAssert.AssertHasType(_source.Ccu, "RootHomeControllerNode");
      CodeDomAssert.AssertNotHasField(type, "_services");
    }
    #endregion
  }
}
