using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory.Parser.AST;
using Attribute = ICSharpCode.NRefactory.Parser.AST.Attribute;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class ControllerVisitor : TypeResolvingVisitor
  {
    #region Member Data
    private ITreeCreationService _treeService;
    private ILogger _logger;
    #endregion

    #region ControllerVisitor()
    public ControllerVisitor(ILogger logger, ITypeResolver typeResolver, ITreeCreationService treeService)
      : base(typeResolver)
    {
      _logger = logger;
      _treeService = treeService;
    }
    #endregion

    #region AbstractAstVisitor Members
    public override object Visit(MethodDeclaration methodDeclaration, object data)
    {
      if (methodDeclaration.Modifier != Modifier.Public)
      {
        return null;
      }

      ControllerTreeNode controllerNode = (ControllerTreeNode)_treeService.Peek;

      ActionTreeNode action = new ActionTreeNode(methodDeclaration.Name);

      foreach (ParameterDeclarationExpression parameter in methodDeclaration.Parameters)
      {
        string typeName = this.TypeResolver.Resolve(parameter.TypeReference);
        action.AddChild(new ParameterTreeNode(parameter.ParameterName, typeName));
      }
      controllerNode.AddChild(action, true);

      return base.Visit(methodDeclaration, data);
    }

    public override object Visit(TypeDeclaration typeDeclaration, object data)
    {
      if (!IsController(typeDeclaration))
      {
        return null;
      }

      string area = GetArea(typeDeclaration);
      string typeNamespace = GetNamespace(typeDeclaration);

      if (!String.IsNullOrEmpty(area))
      {
      	string[] areas = area.Split('/');
        for (int i = 0; i < areas.Length; i++)
        {
          TreeNode areaNode = _treeService.FindNode(areas[i]);
          if (areaNode == null)
          {
            areaNode = new AreaTreeNode(areas[i]);
          }
          _treeService.PushNode(areaNode);
        }
      }
      
      ControllerTreeNode node = new ControllerTreeNode(typeDeclaration.Name, typeNamespace);
      _treeService.PushNode(node);

      object r = base.Visit(typeDeclaration, data);

      if (!String.IsNullOrEmpty(area))
      {
        string[] areas = area.Split('/');
        for (int i = 0; i < areas.Length; i++)
        {
          _treeService.PopNode();
        }
      }
      _treeService.PopNode();

      return r;
    }
    #endregion

    #region Protected Methods
    protected virtual bool IsController(TypeDeclaration typeDeclaration)
    {
      bool isController = 
        typeDeclaration.Name.EndsWith("Controller") && (typeDeclaration.Modifier & Modifier.Partial) == Modifier.Partial;

        if(!isController)
        {
            if( (typeDeclaration.Modifier & Modifier.Partial) != Modifier.Partial)
            {
                _logger.LogInfo("Controller Source for " + typeDeclaration.Name +
                                " will not be included in the generated sitemap because it is not a partial type");
            }
            if( !typeDeclaration.Name.EndsWith("Controller"))
            {
                _logger.LogInfo("Controller source for " + typeDeclaration.Name +
                                " will not be included in the generated site map because its type name does not end with controller");

            }
        }

        return isController;
    }

    protected virtual string GetArea(TypeDeclaration typeDeclaration)
    {
      foreach (AttributeSection attributeSection in typeDeclaration.Attributes)
      {
        foreach (Attribute attribute in attributeSection.Attributes)
        {
          if (attribute.Name == "ControllerDetails")
          {
            foreach (NamedArgumentExpression namedArgument in attribute.NamedArguments)
            {
              if (namedArgument.Name == "Area")
              {
                return ResolvePrimitiveValue(namedArgument.Expression);
              }
            }
          }
        }
      }
      return null;
    }

    protected virtual string ResolvePrimitiveValue(Expression expression)
    {
      if (expression is PrimitiveExpression)
        return ((PrimitiveExpression)expression).Value.ToString();
      return null;
    }
    #endregion
  }
}
