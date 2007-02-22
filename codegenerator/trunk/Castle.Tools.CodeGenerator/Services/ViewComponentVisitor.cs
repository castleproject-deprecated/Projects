using System;
using System.Collections.Generic;

using Castle.Tools.CodeGenerator.Model;

using ICSharpCode.NRefactory.Parser.AST;
using Attribute = ICSharpCode.NRefactory.Parser.AST.Attribute;

namespace Castle.Tools.CodeGenerator.Services
{
  public class ViewComponentVisitor : TypeResolvingVisitor
  {
    #region Member Data
    private ITreeCreationService _treeService;
    private ILogger _logger;
    #endregion

    #region ViewComponentVisitor()
    public ViewComponentVisitor(ILogger logger, ITypeResolver typeResolver, ITreeCreationService treeService)
      : base(typeResolver)
    {
      _logger = logger;
      _treeService = treeService;
    }
    #endregion

    #region AbstractAstVisitor Members
    public override object Visit(CompilationUnit compilationUnit, object data)
    {
      _treeService.PushArea("Components");
      object r = base.Visit(compilationUnit, data);
      _treeService.PopNode();
      return r;
    }

    /*
    public override object Visit(MethodDeclaration methodDeclaration, object data)
    {
      return null;
    }
    */

    public override object Visit(TypeDeclaration typeDeclaration, object data)
    {
      if (!IsViewComponent(typeDeclaration))
      {
        return null;
      }

      string typeNamespace = GetNamespace(typeDeclaration);

      ViewComponentTreeNode node = new ViewComponentTreeNode(typeDeclaration.Name, typeNamespace);
      _treeService.PushNode(node);

      object r = base.Visit(typeDeclaration, data);

      _treeService.PopNode();

      return r;
    }
    #endregion

    #region Protected Methods
    protected virtual bool IsViewComponent(TypeDeclaration typeDeclaration)
    {
      return
        typeDeclaration.Name.EndsWith("Component");// && (typeDeclaration.Modifier & Modifier.Partial) == Modifier.Partial;
    }
    #endregion
  }
}
