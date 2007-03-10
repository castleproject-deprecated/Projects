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
      List<string> actionArgumentTypes = new List<string>();

      CodeMemberMethod method = new CodeMemberMethod();
      method.Name = node.Name;
      method.ReturnType = _source[typeof(IControllerActionReference)];
      method.Attributes = MemberAttributes.Public;
      method.CustomAttributes.Add(_source.DebuggerAttribute);
      List<CodeExpression> actionArguments = CreateActionArgumentsAndAddParameters(method, node, actionArgumentTypes);
      method.Statements.Add(new CodeMethodReturnStatement(CreateNewActionReference(node, actionArguments, actionArgumentTypes)));
      type.Members.Add(method);


      if (actionArguments.Count > 0 && _occurences[node.Name] == 1)
      {
        method = new CodeMemberMethod();
        method.Comments.Add(new CodeCommentStatement("Empty argument Action... Not sure if we want to pass MethodInformation to these..."));
        method.Name = node.Name;
        method.ReturnType = _source[typeof(IArgumentlessControllerActionReference)];
        method.Attributes = MemberAttributes.Public;
        method.CustomAttributes.Add(_source.DebuggerAttribute);
        method.Statements.Add(new CodeMethodReturnStatement(CreateNewActionReference(node, new List<CodeExpression>(), actionArgumentTypes)));
        type.Members.Add(method);
      }

      base.Visit(node);
    }
    #endregion

    #region Methods
    /*
    I opted to only get this information when it was necessary, but decided to check this in at least once because it might be useful. -jlewalle
    protected string CreateMethodInformation(ActionTreeNode node, CodeTypeDeclaration type, List<string> actionArgumentTypes)
    {
      string methodInfoName = _naming.ToMethodSignatureName(node.Name, actionArgumentTypes.ToArray());
      string memberName = _naming.ToMemberVariableName(methodInfoName);
      CodeMemberField field = new CodeMemberField(_source[typeof(MethodInformation)], memberName);
      field.Attributes = MemberAttributes.Family;
      type.Members.Add(field);

      List<CodeExpression> actionArgumentRuntimeTypes = new List<CodeExpression>();
      foreach (string typeName in actionArgumentTypes)
      {
        actionArgumentRuntimeTypes.Add(new CodeTypeOfExpression(_source[typeName]));
      }
      _constructor.Statements.Add(
        new CodeAssignStatement(
          new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), memberName),
          new CodeMethodInvokeExpression(
            new CodeMethodReferenceExpression(
              new CodePropertyReferenceExpression(
                new CodeArgumentReferenceExpression(_naming.ToVariableName(_serviceIdentifier)), "RuntimeInformationService"
              ),
              "ResolveMethodInformation"
            ),
            new CodeExpression[]
            {
              new CodeTypeOfExpression(node.Controller.FullName),
              new CodePrimitiveExpression(node.Name),
              new CodeArrayCreateExpression(_source[typeof(Type)], actionArgumentRuntimeTypes.ToArray())
            }
          )
        )
      );
      return memberName;
    }
    */

    protected CodeExpression CreateNewActionReference(ActionTreeNode node, List<CodeExpression> actionArguments, List<string> actionArgumentTypes)
    {
      List<CodeExpression> actionArgumentRuntimeTypes = new List<CodeExpression>();
      foreach (string typeName in actionArgumentTypes)
      {
        actionArgumentRuntimeTypes.Add(new CodeTypeOfExpression(_source[typeName]));
      }

      CodeExpression createMethodSignature = new CodeObjectCreateExpression(
        _source[typeof(MethodSignature)],
        new CodeExpression[]
        {
          new CodeTypeOfExpression(node.Controller.FullName),
          new CodePrimitiveExpression(node.Name),
          new CodeArrayCreateExpression(_source[typeof(Type)], actionArgumentRuntimeTypes.ToArray())
        }
      );

      CodeExpression[] constructionArguments = new CodeExpression[]
        {
          new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), _naming.ToMemberVariableName(_serviceIdentifier)),
          new CodeTypeOfExpression(node.Controller.FullName),
          new CodePrimitiveExpression(node.Controller.Area),
          new CodePrimitiveExpression(_naming.ToControllerName(node.Controller.Name)),
          new CodePrimitiveExpression(node.Name),
          createMethodSignature,
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

    protected List<CodeExpression> CreateActionArgumentsAndAddParameters(CodeMemberMethod method, ActionTreeNode node, List<string> actionArgumentTypes)
    {
      List<CodeExpression> actionArguments = new List<CodeExpression>();

      int index = 0;
      foreach (ParameterTreeNode parameterInfo in node.Children)
      {
        CodeParameterDeclarationExpression newParameter = new CodeParameterDeclarationExpression();
        newParameter.Name = parameterInfo.Name;
        newParameter.Type = _source[parameterInfo.Type];
        method.Parameters.Add(newParameter);

        actionArgumentTypes.Add(parameterInfo.Type);

        CodeObjectCreateExpression argumentCreate =
          new CodeObjectCreateExpression(_source[typeof(ActionArgument)], new CodeExpression[]
                                                                                          {
                                                                                            new CodePrimitiveExpression(index++),
                                                                                            new CodePrimitiveExpression(parameterInfo.Name),
                                                                                            new CodeTypeOfExpression(newParameter.Type),
                                                                                            new CodeArgumentReferenceExpression(parameterInfo.Name)
                                                                                          });

        actionArguments.Add(argumentCreate);
      }
      return actionArguments;
    }
    #endregion
  }
}
