using System;
using System.Collections.Generic;
using System.CodeDom;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class ControllerMapGenerator : AbstractGenerator
  {
    #region ControllerMapGenerator()
    public ControllerMapGenerator(ILogger logger, ISourceGenerator source, INamingService naming, string targetNamespace, string serviceType)
     : base(logger, source, naming, targetNamespace, serviceType)
    {
    }
    #endregion

    #region Visitor Methods
    public override void Visit(AreaTreeNode node)
    {
      CodeTypeDeclaration type = GenerateTypeDeclaration(_namespace, node.PathNoSlashes + _naming.ToAreaWrapperName(node.Name));

      if (_typeStack.Count > 0)
      {
        CodeTypeDeclaration parent = _typeStack.Peek();
        _source.AddFieldPropertyConstructorInitialize(parent, node.Name.Replace("/", ""), type.Name);
      }

      _typeStack.Push(type);
      base.Visit(node);
      _typeStack.Pop();
    }

    public override void Visit(ControllerTreeNode node)
    {
      // CodeTypeDeclaration type = GenerateTypeDeclaration(_namespace, _naming.ToControllerWrapperName(node.Name));
      CodeTypeDeclaration type = _source.GenerateTypeDeclaration(_namespace, node.PathNoSlashes + _naming.ToControllerWrapperName(node.Name));
      CodeConstructor constructor = CreateServicesConstructor();
      constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression(_naming.ToVariableName(_serviceIdentifier)));
      type.BaseTypes.Add(_source[node.PathNoSlashes + _naming.ToActionWrapperName(node.Name)]);
      type.Members.Add(constructor);

      CodeTypeDeclaration parent = _typeStack.Peek();
      _source.AddFieldPropertyConstructorInitialize(parent, _naming.ToControllerName(node.Name), type.Name);

      type.Members.Add(
        _source.CreateReadOnlyProperty("Views", new CodeTypeReference(node.PathNoSlashes + _naming.ToViewWrapperName(node.Name)),
                               new CodeObjectCreateExpression(
                                 new CodeTypeReference(node.PathNoSlashes + _naming.ToViewWrapperName(node.Name)),
                                 new CodeFieldReferenceExpression(_source.This, _naming.ToMemberVariableName(_serviceIdentifier)))));
      /*
      type.Members.Add(
        _source.CreateReadOnlyProperty("Actions", new CodeTypeReference(node.PathNoSlashes + _naming.ToActionWrapperName(node.Name)),
                               new CodeObjectCreateExpression(
                                 new CodeTypeReference( _naming.ToActionWrapperName(node.Name)),
                                 new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), _naming.ToMemberVariableName(_serviceIdentifier)))));
      */
      type.Members.Add(
        _source.CreateReadOnlyProperty("Actions", new CodeTypeReference(node.PathNoSlashes + _naming.ToActionWrapperName(node.Name)), _source.This));
    }
    #endregion
  }
}