using System;
using System.Collections.Generic;
using Castle.Tools.CodeGenerator.Model;
using ICSharpCode.NRefactory.Parser.AST;
using Rhino.Mocks;
using NUnit.Framework;
using Attribute=ICSharpCode.NRefactory.Parser.AST.Attribute;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class ControllerVisitorTests
  {
    #region Member Data
    private MockRepository _mocks;
    private ITreeCreationService _treeService;
    private ILogger _logger;
    private ITypeResolver _typeResolver;
    private ControllerVisitor _visitor;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _mocks = new MockRepository();
      _logger = new NullLogger();
      _typeResolver = _mocks.CreateMock<ITypeResolver>();
      _treeService = _mocks.CreateMock<ITreeCreationService>();
      _visitor = new ControllerVisitor(_logger, _typeResolver, _treeService);
    }
    #endregion

    #region Test Methods
    [Test]
    public void VisitTypeDeclaration_NotController_DoesNothing()
    {
      TypeDeclaration type = new TypeDeclaration(Modifier.Public, new List<AttributeSection>());
      type.Name = "SomeRandomType";

      _mocks.ReplayAll();
      _visitor.Visit(type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_AControllerNotPartial_DoesNothing()
    {
      TypeDeclaration type = new TypeDeclaration(Modifier.Public, new List<AttributeSection>());
      type.Name = "SomeRandomController";

      _mocks.ReplayAll();
      _visitor.Visit(type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_AControllerNoChildren_PushesAndPops()
    {
      TypeDeclaration type = new TypeDeclaration(Modifier.Public | Modifier.Partial, new List<AttributeSection>());
      type.Name = "SomeRandomController";

      using (_mocks.Unordered())
      {
        _treeService.PushNode(new ControllerTreeNode("SomeRandomController", "SomeNamespace"));
        _treeService.PopNode();
      }

      _mocks.ReplayAll();
      _visitor.Visit(type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_AControllerNoChildrenWithArea_PushesAndPops()
    {
      TypeDeclaration type = new TypeDeclaration(Modifier.Public | Modifier.Partial, new List<AttributeSection>());
      type.Name = "SomeRandomController";
      type.Attributes.Add(CreateAreaAttributeCode("ControllerDetails", "Area", new PrimitiveExpression("AnArea", "AnArea")));

      using (_mocks.Unordered())
      {
        Expect.Call(_treeService.FindNode("AnArea")).Return(null);
        _treeService.PushNode(new AreaTreeNode("AnArea"));
        _treeService.PushNode(new ControllerTreeNode("SomeRandomController", "SomeNamespace"));
        _treeService.PopNode();
        _treeService.PopNode();
      }

      _mocks.ReplayAll();
      _visitor.Visit(type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_AControllerNoChildrenTrickyAreaValue_IgnoresAreaAttribute()
    {
      TypeDeclaration type = new TypeDeclaration(Modifier.Public | Modifier.Partial, new List<AttributeSection>());
      type.Name = "SomeRandomController";
      type.Attributes.Add(CreateAreaAttributeCode("ControllerDetails", "Area", new AddressOfExpression(new PrimitiveExpression("ThisNeverHappens", "Ok?"))));

      using (_mocks.Unordered())
      {
        _treeService.PushNode(new ControllerTreeNode("SomeRandomController", "SomeNamespace"));
        _treeService.PopNode();
      }

      _mocks.ReplayAll();
      _visitor.Visit(type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_AControllerTrickyAttribute_IgnoresAttribute()
    {
      TypeDeclaration type = new TypeDeclaration(Modifier.Public | Modifier.Partial, new List<AttributeSection>());
      type.Name = "SomeRandomController";
      type.Attributes.Add(CreateAreaAttributeCode("NotControllerDetails", "NotArea", new PrimitiveExpression("NotAnArea", "NotAnArea")));

      using (_mocks.Unordered())
      {
        _treeService.PushNode(new ControllerTreeNode("SomeRandomController", "SomeNamespace"));
        _treeService.PopNode();
      }

      _mocks.ReplayAll();
      _visitor.Visit(type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitMethodDeclaration_ProtectedMember_DoesNothing()
    {
      MethodDeclaration method = new MethodDeclaration("Action", Modifier.Protected, null, new List<ParameterDeclarationExpression>(), new List<AttributeSection>());

      using (_mocks.Unordered())
      {
      }

      _mocks.ReplayAll();
      _visitor.Visit(method, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitMethodDeclaration_ActionMemberNoArguments_CreatesEntryInNode()
    {
      MethodDeclaration method = new MethodDeclaration("Action", Modifier.Public, null, new List<ParameterDeclarationExpression>(), new List<AttributeSection>());
      ControllerTreeNode node = new ControllerTreeNode("SomeController", "SomeNamespace");

      using (_mocks.Unordered())
      {
        Expect.Call(_treeService.Peek).Return(node);
      }

      _mocks.ReplayAll();
      _visitor.Visit(method, null);
      _mocks.VerifyAll();
      
      Assert.AreEqual("Action", node.Children[0].Name);
      Assert.AreEqual(0, node.Children[0].Children.Count);
    }

    [Test]
    public void VisitMethodDeclaration_ActionMemberStandardArgument_CreatesEntryInNode()
    {
      MethodDeclaration method = new MethodDeclaration("Action", Modifier.Public, null, new List<ParameterDeclarationExpression>(), new List<AttributeSection>());
      method.Parameters.Add(new ParameterDeclarationExpression(new TypeReference("bool"), "parameter"));
      ControllerTreeNode node = new ControllerTreeNode("SomeController", "SomeNamespace");

      using (_mocks.Unordered())
      {
        Expect.Call(_treeService.Peek).Return(node);
        Expect.Call(_typeResolver.Resolve("System.Boolean")).Return("System.Boolean");
      }

      _mocks.ReplayAll();
      _visitor.Visit(method, null);
      _mocks.VerifyAll();
      
      Assert.AreEqual("Action", node.Children[0].Name);
      Assert.AreEqual("parameter", node.Children[0].Children[0].Name);
    }

    [Test]
    public void VisitMethodDeclaration_ActionMemberTrickyArgumentType_CreatesEntryInNode()
    {
      MethodDeclaration method = new MethodDeclaration("Action", Modifier.Public, null, new List<ParameterDeclarationExpression>(), new List<AttributeSection>());
      method.Parameters.Add(new ParameterDeclarationExpression(new TypeReference("bool"), "parameter"));
      ControllerTreeNode node = new ControllerTreeNode("SomeController", "SomeNamespace");

      using (_mocks.Unordered())
      {
        Expect.Call(_treeService.Peek).Return(node);
        Expect.Call(_typeResolver.Resolve("System.Boolean")).Return(null);
        Expect.Call(_typeResolver.Resolve("System.Boolean", true)).Return(typeof(bool));
      }

      _mocks.ReplayAll();
      _visitor.Visit(method, null);
      _mocks.VerifyAll();
      
      Assert.AreEqual("Action", node.Children[0].Name);
      Assert.AreEqual("parameter", node.Children[0].Children[0].Name);
    }
    #endregion

    #region Methods
    private static AttributeSection CreateAreaAttributeCode(string attributeName, string argumentName, Expression valueExpression)
    {
      NamedArgumentExpression argument = new NamedArgumentExpression(argumentName, valueExpression);
      Attribute attribute = new Attribute(attributeName, new List<Expression>(), new List<NamedArgumentExpression>());
      attribute.NamedArguments.Add(argument);
      AttributeSection attributeSection = new AttributeSection("IDontKnow", new List<Attribute>());
      attributeSection.Attributes.Add(attribute);
      return attributeSection;
    }
    #endregion
  }
}