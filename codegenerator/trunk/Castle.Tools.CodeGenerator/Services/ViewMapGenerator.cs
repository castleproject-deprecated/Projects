using System;
using System.CodeDom;
using System.Collections.Generic;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class ViewMapGenerator : AbstractGenerator
  {
    #region ViewMapGenerator()
    public ViewMapGenerator(ILogger logger, ISourceGenerator source, INamingService naming, string targetNamespace, string serviceType)
     : base(logger, source, naming, targetNamespace, serviceType)
    {
    }
    #endregion

    #region Methods
    public override void Visit(ControllerTreeNode node)
    {
      CodeTypeDeclaration type = GenerateTypeDeclaration(_namespace, node.PathNoSlashes + _naming.ToViewWrapperName(node.Name));

      _typeStack.Push(type);
      base.Visit(node);
      _typeStack.Pop();
    }

    public override void Visit(ViewTreeNode node)
    {
      if (_typeStack.Count == 0) return;

      CodeExpression[] constructionArguments = new CodeExpression[]
          {
            new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), _naming.ToMemberVariableName(_serviceIdentifier)),
            new CodeTypeOfExpression(node.Controller.FullName),
            new CodePrimitiveExpression(node.Controller.Area),
            new CodePrimitiveExpression(_naming.ToControllerName(node.Controller.Name)),
            new CodePrimitiveExpression(node.Name)
          };

      CodeExpression returnExpression =
        new CodeMethodInvokeExpression(
          new CodeMethodReferenceExpression(
            new CodePropertyReferenceExpression(
              new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), _naming.ToMemberVariableName(_serviceIdentifier)),
              "ControllerReferenceFactory"),
            "CreateViewReference"),
          constructionArguments
          );

      CodeTypeReference propertyType = new CodeTypeReference(typeof(ControllerViewReference));
      _typeStack.Peek().Members.Add(_source.CreateReadOnlyProperty(node.Name, propertyType, returnExpression));

      base.Visit(node);
    }
    #endregion
  }
}
