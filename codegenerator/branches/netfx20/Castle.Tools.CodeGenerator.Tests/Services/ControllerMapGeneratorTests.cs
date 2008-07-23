using System;
using System.CodeDom;
using System.Collections.Generic;
using Castle.Tools.CodeGenerator.Model.TreeNodes;
using Castle.Tools.CodeGenerator.Services.Generators;
using Rhino.Mocks;
using NUnit.Framework;

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

	[Test]
	public void VisitControllerTreeNode_AreaExistsWithSameNameAsController_AppendsSuffixToAreaNodeField()
	{
		BuildTestTree();

		_mocks.ReplayAll();
		_generator.Visit(_root);
		_mocks.VerifyAll();

		CodeTypeDeclaration type = CodeDomAssert.AssertHasType(_source.Ccu, "RootParentAreaNode");
		CodeMemberField areaField = (CodeMemberField) type.Members[3];
		Assert.AreEqual("_childArea", areaField.Name);
	}

	[Test]
	public void VisitControllerTreeNode_AreaExistsWithSameNameAsController_AppendsSuffixToAreaNodeProperty()
	{
		BuildTestTree();

		_mocks.ReplayAll();
		_generator.Visit(_root);
		_mocks.VerifyAll();

		CodeTypeDeclaration type = CodeDomAssert.AssertHasType(_source.Ccu, "RootParentAreaNode");
		CodeMemberProperty areaProperty = (CodeMemberProperty)type.Members[2];
		Assert.AreEqual("ChildArea", areaProperty.Name);
		CodeMethodReturnStatement statement = (CodeMethodReturnStatement)areaProperty.GetStatements[0];
		Assert.AreEqual("_childArea", ((CodeFieldReferenceExpression)statement.Expression).FieldName);
	}

  	[Test]
	  public void VisitControllerTreeNode_AreaExistsWithSameNameAsController_AppendsSuffixToControllerNodeField()
	  {
		  BuildTestTree();

		  _mocks.ReplayAll();
		  _generator.Visit(_root);
		  _mocks.VerifyAll();

		  CodeTypeDeclaration type = CodeDomAssert.AssertHasType(_source.Ccu, "RootParentAreaNode");
		  CodeMemberField controllerField = (CodeMemberField)type.Members[5];
		  Assert.AreEqual("_childController", controllerField.Name);
	  }

	  [Test]
	  public void VisitControllerTreeNode_AreaExistsWithSameNameAsController_AppendsSuffixToControllerNodeProperty()
	  {
		  BuildTestTree();

		  _mocks.ReplayAll();
		  _generator.Visit(_root);
		  _mocks.VerifyAll();

		  CodeTypeDeclaration type = CodeDomAssert.AssertHasType(_source.Ccu, "RootParentAreaNode");
		  CodeMemberProperty controllerProperty = (CodeMemberProperty)type.Members[4];
		  Assert.AreEqual("ChildController", controllerProperty.Name);
	  	CodeMethodReturnStatement statement = (CodeMethodReturnStatement) controllerProperty.GetStatements[0];
	  	Assert.AreEqual("_childController", ((CodeFieldReferenceExpression) statement.Expression).FieldName);
	  }

	  [Test]
	  public void VisitControllerTreeNode_AreaExistsWithSameNameAsController_AppendsSuffixToConstructorFieldReferences()
	  {
		  BuildTestTree();

		  _mocks.ReplayAll();
		  _generator.Visit(_root);
		  _mocks.VerifyAll();

		  CodeTypeDeclaration type = CodeDomAssert.AssertHasType(_source.Ccu, "RootParentAreaNode");
		  CodeConstructor constructor = (CodeConstructor)type.Members[1];
	  	  Assert.AreEqual(3, constructor.Statements.Count);
	  	  
		  CodeAssignStatement areaAssignment = (CodeAssignStatement) constructor.Statements[1];
	  	  CodeFieldReferenceExpression areaFieldReference = (CodeFieldReferenceExpression) areaAssignment.Left;
		  Assert.AreEqual("_childArea", areaFieldReference.FieldName);
	  	  
	      CodeAssignStatement controllerAssignment = (CodeAssignStatement) constructor.Statements[2];
		  CodeFieldReferenceExpression controllerFieldReference = (CodeFieldReferenceExpression)controllerAssignment.Left;
		  Assert.AreEqual("_childController", controllerFieldReference.FieldName);
	  }
	#endregion

  	#region Supporting methods
  	private void BuildTestTree()
  	{
  		AreaTreeNode parentAreaNode = new AreaTreeNode("Parent");
  		_root.AddChild(parentAreaNode);

  		AreaTreeNode childAreaNode = new AreaTreeNode("Child");
  		parentAreaNode.AddChild(childAreaNode);

  		ControllerTreeNode controllerNode = new ControllerTreeNode("ChildController", "ControllerNamespace");
  		parentAreaNode.AddChild(controllerNode);
  	}
  	#endregion
  }
}
