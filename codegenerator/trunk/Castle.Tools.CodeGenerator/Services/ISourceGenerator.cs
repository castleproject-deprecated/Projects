using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface ISourceGenerator
  {
    CodeCompileUnit Ccu { get;}
    CodeNamespace LookupNamespace(string path);
    CodeTypeDeclaration GenerateTypeDeclaration(string targetNamespace, string name);
    CodeMemberProperty CreateReadOnlyProperty(string name, CodeTypeReference type, CodeExpression returnExpression);
    CodeMemberField NewField(string name, string type);
    CodeMemberProperty NewGetFieldProperty(string name, CodeMemberField field);
    // CodeMemberProperty NewGetExpressionProperty(string name, CodeTypeReference type, CodeExpression expression);
    CodeThisReferenceExpression This { get;}
    CodeAttributeDeclaration DebuggerAttribute { get;}
    void AddFieldPropertyConstructorInitialize(CodeTypeDeclaration type, string name, string fieldType);
    CodeTypeReference this[string name]
    {
      get;
    }
    CodeTypeReference this[Type type]
    {
      get;
    }
  }
}
