using System;
using System.CodeDom;
using System.Collections.Generic;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class ActionMapGenerator : AbstractGenerator
  {
    #region Member Data
    private Dictionary<string, short> _occurences;
    #endregion

    #region ActionMapGenerator()
    public ActionMapGenerator(ILogger logger, ISourceGenerator source, INamingService naming, string targetNamespace, string serviceType)
      : base(logger, source, naming, targetNamespace, serviceType)
    {
    }
    #endregion

    #region Methods
    public override void Visit(ControllerTreeNode node)
    {
      CodeTypeDeclaration type = GenerateTypeDeclaration(_namespace, node.PathNoSlashes + _naming.ToActionWrapperName(node.Name));

      _occurences = new Dictionary<string, short>();

      // We can only generate empty argument methods for actions that appear once and only once.
      foreach (TreeNode child in node.Children)
      {
        if (child is ActionTreeNode)
        {
          if (!_occurences.ContainsKey(child.Name))
          {
            _occurences[child.Name] = 0;
          }
          _occurences[child.Name]++;
        }
      }

      _typeStack.Push(type);
      base.Visit(node);
      _typeStack.Pop();
    }

    public override void Visit(ActionTreeNode node)
    {
      CodeTypeDeclaration type = _typeStack.Peek();

      CodeMemberMethod method = new CodeMemberMethod();
      method.Name = node.Name;
      method.ReturnType = _source[typeof(ControllerActionReference)];
      method.Attributes = MemberAttributes.Public;
      method.CustomAttributes.Add(_source.DebuggerAttribute);
      List<CodeExpression> actionArguments = CreateActionArgumentsAndAddParameters(method, node);
      method.Statements.Add(new CodeMethodReturnStatement(CreateNewActionReference(node, actionArguments)));
      type.Members.Add(method);

      if (actionArguments.Count > 0 && _occurences[node.Name] == 1)
      {
        method = new CodeMemberMethod();
        method.Comments.Add(new CodeCommentStatement("Empty argument Action..."));
        method.Name = node.Name;
        method.ReturnType = _source[typeof(ControllerActionReference)];
        method.Attributes = MemberAttributes.Public;
        method.CustomAttributes.Add(_source.DebuggerAttribute);
        method.Statements.Add(new CodeMethodReturnStatement(CreateNewActionReference(node, new List<CodeExpression>())));
        type.Members.Add(method);
      }

      base.Visit(node);
    }
    #endregion

    #region Methods
    protected CodeExpression CreateNewActionReference(ActionTreeNode node, List<CodeExpression> actionArguments)
    {
      CodeExpression[] constructionArguments = new CodeExpression[]
        {
          new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), _naming.ToMemberVariableName(_serviceIdentifier)),
          new CodeTypeOfExpression(node.Controller.FullName),
          new CodePrimitiveExpression(node.Controller.Area),
          new CodePrimitiveExpression(_naming.ToControllerName(node.Controller.Name)),
          new CodePrimitiveExpression(node.Name),
          new CodeArrayCreateExpression(_source[typeof(ActionArgument)], actionArguments.ToArray())
        };

      return new CodeMethodInvokeExpression(
        new CodeMethodReferenceExpression(
          new CodePropertyReferenceExpression(
            new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), _naming.ToMemberVariableName(_serviceIdentifier)),
            "ControllerReferenceFactory"),
          "CreateActionReference"),
        constructionArguments
        );
    }

    protected List<CodeExpression> CreateActionArgumentsAndAddParameters(CodeMemberMethod method, ActionTreeNode node)
    {
      List<CodeExpression> actionArguments = new List<CodeExpression>();

      foreach (ParameterTreeNode parameterInfo in node.Children)
      {
        CodeParameterDeclarationExpression newParameter = new CodeParameterDeclarationExpression();
        newParameter.Name = parameterInfo.Name;
        newParameter.Type = _source[parameterInfo.Type];
        method.Parameters.Add(newParameter);

        CodeObjectCreateExpression argumentCreate =
          new CodeObjectCreateExpression(_source[typeof(ActionArgument)], new CodeExpression[]
                                                                                          {
                                                                                            new CodePrimitiveExpression(
                                                                                              parameterInfo.Name),
                                                                                            new CodeTypeOfExpression(
                                                                                              newParameter.Type),
                                                                                            new CodeArgumentReferenceExpression
                                                                                              (parameterInfo.Name)
                                                                                          });

        actionArguments.Add(argumentCreate);
      }
      return actionArguments;
    }
    #endregion
  }
}
