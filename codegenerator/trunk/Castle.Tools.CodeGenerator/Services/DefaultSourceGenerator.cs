using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Castle.Tools.CodeGenerator.Services
{
  public class DefaultSourceGenerator : ISourceGenerator
  {
    #region Member Data
    private Dictionary<string, CodeTypeReference> _typeReferences = new Dictionary<string, CodeTypeReference>();
    private Dictionary<string, CodeNamespace> _namespaces = new Dictionary<string, CodeNamespace>();
    private List<string> _defaultUsings  = new List<string>();
    private CodeCompileUnit _ccu;
    private INamingService _naming;
    #endregion

    #region Properties
    public CodeCompileUnit Ccu
    {
      get { return _ccu; }
    }
    #endregion

    #region DefaultSourceGenerator()
    public DefaultSourceGenerator()
    {
      _naming = new DefaultNamingService();
      _ccu = new CodeCompileUnit();
      _defaultUsings.Add("System");
    }
    #endregion

    #region Methods
    public CodeNamespace LookupNamespace(string path)
    {
      if (_namespaces.ContainsKey(path)) return _namespaces[path];
      _namespaces[path] = new CodeNamespace(path);
      foreach (string defaultNamespace in _defaultUsings)
      {
        _namespaces[path].Imports.Add(new CodeNamespaceImport(defaultNamespace));
      }
      _ccu.Namespaces.Add(_namespaces[path]);
      return _namespaces[path];
    }

    public virtual CodeTypeDeclaration GenerateTypeDeclaration(string targetNamespace, string name)
    {
      CodeTypeDeclaration type = new CodeTypeDeclaration(name);
      type.TypeAttributes = TypeAttributes.Public;
      type.IsPartial = true;
      type.CustomAttributes.Add(
        new CodeAttributeDeclaration(new CodeTypeReference(typeof(GeneratedCodeAttribute)),
                                     new CodeAttributeArgument(new CodePrimitiveExpression("Castle.Tools.CodeGenerator")),
                                     new CodeAttributeArgument(new CodePrimitiveExpression("0.1"))));
      LookupNamespace(targetNamespace).Types.Add(type);
      return type;
    }

    public virtual CodeMemberProperty CreateReadOnlyProperty(string name, CodeTypeReference type, CodeExpression returnExpression)
    {
      CodeMemberProperty property = new CodeMemberProperty();
      property.Attributes = MemberAttributes.Public;
      property.Name = name;
      property.HasGet = true;
      property.HasSet = false;
      property.Type = type;
      property.GetStatements.Add(new CodeMethodReturnStatement(returnExpression));
      property.CustomAttributes.Add(DebuggerAttribute);
      return property;
    }

    public CodeMemberField NewField(string name, string type)
    {
      CodeMemberField field = new CodeMemberField();
      field.Attributes = MemberAttributes.Private;
      field.Name = _naming.ToMemberVariableName(name);
      field.Type = this[type];
      return field;
    }

    public CodeMemberProperty NewGetFieldProperty(string name, CodeMemberField field)
    {
      CodeMemberProperty property = new CodeMemberProperty();
      property.Attributes = MemberAttributes.Public;
      property.Name = _naming.ToPropertyName(name);
      property.HasGet = true;
      property.HasSet = false;
      property.Type = field.Type;
      property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)));
      property.CustomAttributes.Add(DebuggerAttribute);
      return property;
    }

    /*
    public CodeMemberProperty NewGetExpressionProperty(string name, CodeTypeReference type, CodeExpression expression)
    {
      CodeMemberProperty property = new CodeMemberProperty();
      property.Attributes = MemberAttributes.Public;
      property.Name = _naming.ToPropertyName(name);
      property.HasGet = true;
      property.HasSet = false;
      property.Type = type;
      property.GetStatements.Add(new CodeMethodReturnStatement(expression));
      property.CustomAttributes.Add(DebuggerAttribute);
      return property;
    }
    */

    public CodeThisReferenceExpression This
    {
      get { return new CodeThisReferenceExpression(); }
    }

    public CodeAttributeDeclaration DebuggerAttribute
    {
      get { return new CodeAttributeDeclaration(new CodeTypeReference(typeof(DebuggerNonUserCodeAttribute))); }
    }

    public void AddFieldPropertyConstructorInitialize(CodeTypeDeclaration type, string name, string fieldType)
    {
      CodeMemberField field = NewField(name, fieldType);
      CodeMemberProperty property = NewGetFieldProperty(name, field);

      type.Members.Add(property);
      type.Members.Add(field);
      // Probably not the best assumption? --jlewalle
      CodeConstructor constructor = (CodeConstructor)type.Members[1];
      constructor.Statements.Add(
        new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name),
                                new CodeObjectCreateExpression(field.Type,
                                                               new CodeFieldReferenceExpression(
                                                                 new CodeThisReferenceExpression(), _naming.ToMemberVariableName("services")))));
    }

    public CodeTypeReference this[string name]
    {
      get
      {
        if (!_typeReferences.ContainsKey(name))
        {
          _typeReferences[name] = new CodeTypeReference(name);
        }
        return _typeReferences[name];
      }
    }
    public CodeTypeReference this[Type type]
    {
      get { return this[type.FullName]; }
    }
    #endregion
  }
}
