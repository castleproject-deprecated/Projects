using System.Collections.Generic;
using Castle.Tools.CodeGenerator.Model.TreeNodes;
using Castle.Tools.CodeGenerator.Services.Visitors;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;
using Rhino.Mocks;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class ViewComponentVisitorTests
  {
    #region Member Data
    private MockRepository _mocks;
    private ITreeCreationService _treeService;
    private ILogger _logger;
    private ITypeResolver _typeResolver;
    private ViewComponentVisitor _visitor;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _mocks = new MockRepository();
      _logger = new NullLogger();
      _typeResolver = _mocks.CreateMock<ITypeResolver>();
      _treeService = _mocks.CreateMock<ITreeCreationService>();
      _visitor = new ViewComponentVisitor(_logger, _typeResolver, _treeService);
    }
    #endregion

    #region Test Methods
    [Test]
    public void VisitCompileUnit_Always_PushesComponentsArea()
    {
      CompilationUnit cu = new CompilationUnit();

      using (_mocks.Unordered())
      {
        _treeService.PushArea("Components");
        _treeService.PopNode();
      }

      _mocks.ReplayAll();
      _visitor.VisitCompilationUnit(cu, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_NotViewComponent_DoesNothing()
    {
      TypeDeclaration type = new TypeDeclaration(Modifiers.Public, new List<AttributeSection>());
      type.Name = "SomeRandomType";

      _mocks.ReplayAll();
	  _visitor.VisitTypeDeclaration(type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_AViewComponentNoChildren_PushesAndPops()
    {
      TypeDeclaration type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>());
      type.Name = "SomeRandomComponent";

      using (_mocks.Unordered())
      {
        _treeService.PushNode(new ViewComponentTreeNode("SomeRandomComponent", "SomeNamespace"));
        _treeService.PopNode();
      }

      _mocks.ReplayAll();
      _visitor.VisitTypeDeclaration(type, null);
      _mocks.VerifyAll();
    }
    #endregion
  }
}
