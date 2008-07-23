using System;
using System.CodeDom;

namespace Castle.Tools.CodeGenerator.Services.Generators
{
	public interface ISourceGenerator
	{
		CodeCompileUnit Ccu { get; }
		CodeNamespace LookupNamespace(string path);
		CodeTypeDeclaration GenerateTypeDeclaration(string targetNamespace, string name, params string[] parents);
		CodeMemberProperty CreateReadOnlyProperty(string name, CodeTypeReference type, CodeExpression returnExpression);
		CodeMemberField NewField(string name, string type);
		CodeMemberField NewPublicConstant<T>(string name, T value);
		CodeMemberProperty NewGetFieldProperty(string name, CodeMemberField field);
		CodeThisReferenceExpression This { get; }
		CodeAttributeDeclaration DebuggerAttribute { get; }
		CodeAttributeDeclaration CodeGeneratorAttribute { get; }
		void AddFieldPropertyConstructorInitialize(CodeTypeDeclaration type, string name, string fieldType);
		CodeTypeReference this[string name] { get; }
		CodeTypeReference this[Type type] { get; }
	}
}