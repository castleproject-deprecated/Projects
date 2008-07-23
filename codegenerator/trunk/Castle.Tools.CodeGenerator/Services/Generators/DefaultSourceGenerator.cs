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
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using CodeDom;

	public class DefaultSourceGenerator : ISourceGenerator
	{
		private readonly Dictionary<string, CodeTypeReference> typeReferences = new Dictionary<string, CodeTypeReference>();
		private readonly Dictionary<string, CodeNamespace> namespaces = new Dictionary<string, CodeNamespace>();
		private readonly List<string> defaultUsings = new List<string>();
		private readonly INamingService naming;

		public DefaultSourceGenerator()
		{
			naming = new DefaultNamingService();
			Ccu = new CodeCompileUnit();
			defaultUsings.Add("System");
		}

		public CodeCompileUnit Ccu { get; private set; }

		public CodeNamespace LookupNamespace(string path)
		{
			if (namespaces.ContainsKey(path)) return namespaces[path];

			namespaces[path] = new CodeNamespace(path);
			
			foreach (var defaultNamespace in defaultUsings)
				namespaces[path].Imports.Add(new CodeNamespaceImport(defaultNamespace));
			
			Ccu.Namespaces.Add(namespaces[path]);
			
			return namespaces[path];
		}

		public virtual CodeTypeDeclaration GenerateTypeDeclaration(string targetNamespace, string name, params string[] parents)
		{
			var type = CreateTypeDeclaration.Called(name)
				.AsPartial
				.WithAttributes(TypeAttributes.Public)
				.WithCustomAttributes(CodeGeneratorAttribute)
				.Type;

			var ns = LookupNamespace(targetNamespace);

			if (parents.Length == 0)
				ns.Types.Add(type);
			else
			{
				var members = GetMemberCollectionForTypeDeclaration(ns.Types, parents[0]);

				for (var i = 1; i < parents.Length; i++)
					members = GetMemberCollectionForTypeMember(members, parents[i]);

				members.Add(type);
			}

			return type;
		}

		private static CodeTypeMemberCollection GetMemberCollectionForTypeDeclaration(CodeTypeDeclarationCollection collection, string typeName)
		{
			foreach (CodeTypeDeclaration typeDeclaration in collection)
				if (typeDeclaration.Name == typeName)
					return typeDeclaration.Members;
			
			throw new Exception("Couldn't find type " + typeName + ".");
		}

		private static CodeTypeMemberCollection GetMemberCollectionForTypeMember(CodeTypeMemberCollection collection, string typeName)
		{
			for (var i = 0; i < collection.Count; i++)
			{
				if (!(collection[i] is CodeTypeDeclaration)) continue;

				var codeTypeDeclaration = (CodeTypeDeclaration) collection[i];

				if (codeTypeDeclaration.Name == typeName)
					return codeTypeDeclaration.Members;
			}

			throw new Exception("Couldn't find type " + typeName + ".");
		}

		public virtual CodeMemberProperty CreateReadOnlyProperty(string name, CodeTypeReference type, CodeExpression returnExpression)
		{
			return CreateMemberProperty.OfType(type)
				.Called(name)
				.WithAttributes(MemberAttributes.Public)
				.WithCustomAttributes(DebuggerAttribute)
				.WithGetter(new CodeMethodReturnStatement(returnExpression))
				.Property;
		}

		public CodeMemberProperty NewGetFieldProperty(string name, CodeMemberField field)
		{
			return CreateMemberProperty.OfType(field.Type)
				.Called(naming.ToPropertyName(name))
				.WithAttributes(MemberAttributes.Public)
				.WithCustomAttributes(DebuggerAttribute)
				.WithGetter(
				new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)))
				.Property;
		}

		public CodeMemberField NewField(string name, string type)
		{
			return CreateMemberField.WithNameAndType(naming.ToMemberVariableName(name), type)
				.WithAttributes(MemberAttributes.Private)
				.Field;
		}

		public CodeMemberField NewPublicConstant<T>(string name, T value)
		{
			return CreateMemberField.WithNameAndType<T>(name)
				.WithAttributes(MemberAttributes.Public | MemberAttributes.Const)
				.WithInitialValue(value)
				.Field;
		}

		public CodeThisReferenceExpression This
		{
			get { return new CodeThisReferenceExpression(); }
		}

		public CodeAttributeDeclaration DebuggerAttribute
		{
			get { return CreateAttributeDeclaration.ForAttributeType<DebuggerNonUserCodeAttribute>().Attribute; }
		}

		public CodeAttributeDeclaration CodeGeneratorAttribute
		{
			get
			{
				return CreateAttributeDeclaration.ForAttributeType<GeneratedCodeAttribute>()
					.WithArgument("Castle.Tools.CodeGenerator")
					.WithArgument("0.2")
					.Attribute;
			}
		}

		public void AddFieldPropertyConstructorInitialize(CodeTypeDeclaration type, string name, string fieldType)
		{
			// Probably not the best assumption? --jlewalle
			// Probably....but it works for now ;) -- lee
			var constructor = (CodeConstructor) type.Members[1];

			var field = NewField(name, fieldType);
			var property = NewGetFieldProperty(name, field);

			for (var i = 0; i < type.Members.Count; i++)
			{
				if (!(type.Members[i] is CodeMemberField)) continue;

				var fieldMember = (CodeMemberField) type.Members[i];

				if (fieldMember.Name != field.Name) continue;

				// Another field already exists with the name we are about to create the new field with.
				// This occurs during the use of wizardsteps.

				if (!fieldMember.Type.BaseType.EndsWith("AreaNode") && !fieldMember.Type.BaseType.EndsWith("ControllerNode"))
					continue;

				var areaNodeSuffix = "Area";
				var controllerNodeSuffix = "Controller";
				var fieldMemberIsAreaNode = fieldMember.Type.BaseType.EndsWith("AreaNode");

				foreach (CodeStatement codeStatement in constructor.Statements)
				{
					if (!(codeStatement is CodeAssignStatement)) continue;
					
					var codeAssignStatement = (CodeAssignStatement) codeStatement;

					if (!(codeAssignStatement.Left is CodeFieldReferenceExpression)) continue;
					
					var codeFieldReferenceExpression = (CodeFieldReferenceExpression) codeAssignStatement.Left;

					if (!(codeFieldReferenceExpression.TargetObject is CodeThisReferenceExpression)) continue;

					if (codeFieldReferenceExpression.FieldName == fieldMember.Name)
						codeFieldReferenceExpression.FieldName += fieldMemberIsAreaNode ? areaNodeSuffix : controllerNodeSuffix;
				}

				fieldMember.Name += fieldMemberIsAreaNode ? areaNodeSuffix : controllerNodeSuffix;

				var fieldMemberProperty = (CodeMemberProperty) type.Members[i - 1];
				fieldMemberProperty.Name += fieldMemberIsAreaNode ? areaNodeSuffix : controllerNodeSuffix;
				fieldMemberProperty.GetStatements.RemoveAt(0);
				fieldMemberProperty.GetStatements.Insert(0, 
					new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldMember.Name)));

				field.Name += fieldMemberIsAreaNode ? controllerNodeSuffix : areaNodeSuffix;

				property.Name += fieldMemberIsAreaNode ? controllerNodeSuffix : areaNodeSuffix;
				property.GetStatements.RemoveAt(0);
				property.GetStatements.Insert(0,
					new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)));
			}

			type.Members.Add(property);
			type.Members.Add(field);

			constructor.Statements.Add(
				new CodeAssignStatement(
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name),
					new CodeObjectCreateExpression(
						field.Type, 
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), naming.ToMemberVariableName("services")))));
		}

		public CodeTypeReference this[string name]
		{
			get
			{
				if (!typeReferences.ContainsKey(name))
					typeReferences[name] = new CodeTypeReference(name);
				
				return typeReferences[name];
			}
		}

		public CodeTypeReference this[Type type]
		{
			get { return this[type.FullName]; }
		}
	}
}