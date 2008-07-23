// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Tools.CodeGenerator.Services.Generators
{
	using System;
	using System.CodeDom;

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