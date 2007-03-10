using System;
using System.CodeDom;
using System.Collections.Generic;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public abstract class AbstractGenerator : TreeWalker, IGenerator
  {
    #region Member Data
    protected Stack<CodeTypeDeclaration> _typeStack = new Stack<CodeTypeDeclaration>();
    protected ILogger _logger;
    protected ISourceGenerator _source;
    protected INamingService _naming;
    protected string _namespace;
    protected string _serviceType;
    protected string _serviceIdentifier;
    protected CodeConstructor _constructor;
    #endregion

    #region AbstractGenerator()
    public AbstractGenerator(ILogger logger, ISourceGenerator source, INamingService naming, string targetNamespace, string serviceType)
    {
      _logger = logger;
      _source = source;
      _naming = naming;
      _namespace = targetNamespace;
      _serviceType = serviceType;
      _serviceIdentifier = "services";
    }
    #endregion

    #region IGenerator Members
    public void Generate(TreeNode root)
    {
      Accept(root);
    }
    #endregion

    #region Protected Methods
    protected virtual CodeTypeDeclaration GenerateTypeDeclaration(string ns, string name)
    {
      CodeTypeDeclaration type = _source.GenerateTypeDeclaration(ns, name);

      CreateMemberFields(type);

      CodeConstructor constructor = _constructor = CreateServicesConstructor();
      constructor.Statements.Add(
        new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), _naming.ToMemberVariableName(_serviceIdentifier)),
                                new CodeArgumentReferenceExpression(_naming.ToVariableName(_serviceIdentifier))));
      type.Members.Add(constructor);
      return type;
    }

    protected virtual void CreateMemberFields(CodeTypeDeclaration type)
    {
      CodeMemberField field = new CodeMemberField(_source[_serviceType], _naming.ToMemberVariableName(_serviceIdentifier));
      field.Attributes = MemberAttributes.Family;
      type.Members.Add(field);
    }

    protected virtual CodeConstructor CreateServicesConstructor()
    {
      CodeConstructor constructor = new CodeConstructor();
      constructor.Attributes = MemberAttributes.Public;
      constructor.Parameters.Add(new CodeParameterDeclarationExpression(_serviceType, _naming.ToVariableName(_serviceIdentifier)));
      return constructor;
    }
    #endregion
  }
}
