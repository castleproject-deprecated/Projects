using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Castle.Tools.CodeGenerator.CodeDom;

namespace Castle.Tools.CodeGenerator.Services.Generators
{
	public class DefaultSourceGenerator : ISourceGenerator
	{
		private Dictionary<string, CodeTypeReference> _typeReferences = new Dictionary<string, CodeTypeReference>();
		private Dictionary<string, CodeNamespace> _namespaces = new Dictionary<string, CodeNamespace>();
		private List<string> _defaultUsings = new List<string>();
		private CodeCompileUnit _ccu;
		private INamingService _naming;

		public CodeCompileUnit Ccu
		{
			get { return _ccu; }
		}

		public DefaultSourceGenerator()
		{
			_naming = new DefaultNamingService();
			_ccu = new CodeCompileUnit();
			_defaultUsings.Add("System");
		}

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

		public virtual CodeTypeDeclaration GenerateTypeDeclaration(string targetNamespace, string name, params string[] parents)
		{
			CodeTypeDeclaration type = 
				CreateTypeDeclaration.Called(name)
					.AsPartial
					.WithAttributes(TypeAttributes.Public)
					.WithCustomAttributes(CodeGeneratorAttribute)
					.Type;

			CodeNamespace ns = LookupNamespace(targetNamespace);

			if (parents.Length == 0)
				ns.Types.Add(type);
			else
			{
				CodeTypeMemberCollection members = GetMemberCollectionForTypeDeclaration(ns.Types, parents[0]);

				for (int i = 1; i < parents.Length; i++)
					members = GetMemberCollectionForTypeMember(members, parents[i]);
				
				members.Add(type);
			}

			return type;
		}

		private static CodeTypeMemberCollection GetMemberCollectionForTypeDeclaration(CodeTypeDeclarationCollection collection, string typeName)
		{
			foreach (CodeTypeDeclaration typeDeclaration in collection)
			{
				if (typeDeclaration.Name == typeName)
					return typeDeclaration.Members;				
			}

			throw new Exception("Couldn't find type " + typeName + ".");
		}

		private static CodeTypeMemberCollection GetMemberCollectionForTypeMember(CodeTypeMemberCollection collection, string typeName)
		{
			for (int i = 0; i < collection.Count; i++)
			{
				if (collection[i] is CodeTypeDeclaration)
				{
					CodeTypeDeclaration codeTypeDeclaration = (CodeTypeDeclaration)collection[i];

					if (codeTypeDeclaration.Name == typeName)
						return codeTypeDeclaration.Members;					
				}
			}

			throw new Exception("Couldn't find type " + typeName + ".");
		}

		public virtual CodeMemberProperty CreateReadOnlyProperty(string name, CodeTypeReference type,
		                                                         CodeExpression returnExpression)
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
				.Called(_naming.ToPropertyName(name))
				.WithAttributes(MemberAttributes.Public)
				.WithCustomAttributes(DebuggerAttribute)
				.WithGetter(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)))
				.Property;
		}

		public CodeMemberField NewField(string name, string type)
		{
			return CreateMemberField.WithNameAndType(_naming.ToMemberVariableName(name), type)
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
			CodeConstructor constructor = (CodeConstructor) type.Members[1];

			CodeMemberField field = NewField(name, fieldType);
			CodeMemberProperty property = NewGetFieldProperty(name, field);

			for (int i = 0; i < type.Members.Count; i++)
			{
				if (type.Members[i] is CodeMemberField)
				{
					CodeMemberField fieldMember = (CodeMemberField) type.Members[i];

					if (fieldMember.Name == field.Name)
					{
						// Another field already exists with the name we are about to create the new field with.
						// This occurs during the use of wizardsteps.

						if (fieldMember.Type.BaseType.EndsWith("AreaNode") || fieldMember.Type.BaseType.EndsWith("ControllerNode"))
						{
							string areaNodeSuffix = "Area";
							string controllerNodeSuffix = "Controller";
							bool fieldMemberIsAreaNode = fieldMember.Type.BaseType.EndsWith("AreaNode");

							foreach (CodeStatement codeStatement in constructor.Statements)
							{
								if (codeStatement is CodeAssignStatement)
								{
									CodeAssignStatement codeAssignStatement = (CodeAssignStatement) codeStatement;

									if (codeAssignStatement.Left is CodeFieldReferenceExpression)
									{
										CodeFieldReferenceExpression codeFieldReferenceExpression =
											(CodeFieldReferenceExpression) codeAssignStatement.Left;

										if (codeFieldReferenceExpression.TargetObject is CodeThisReferenceExpression)
										{
											if (codeFieldReferenceExpression.FieldName == fieldMember.Name)
												codeFieldReferenceExpression.FieldName += fieldMemberIsAreaNode ? areaNodeSuffix : controllerNodeSuffix;
										}
									}
								}
							}

							fieldMember.Name += fieldMemberIsAreaNode ? areaNodeSuffix : controllerNodeSuffix;

							CodeMemberProperty fieldMemberProperty = (CodeMemberProperty) type.Members[i - 1];
							fieldMemberProperty.Name += fieldMemberIsAreaNode ? areaNodeSuffix : controllerNodeSuffix;
							fieldMemberProperty.GetStatements.RemoveAt(0);
							fieldMemberProperty.GetStatements.Insert(0,
							                                         new CodeMethodReturnStatement(
							                                         	new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
							                                         	                                 fieldMember.Name)));

							field.Name += fieldMemberIsAreaNode ? controllerNodeSuffix : areaNodeSuffix;

							property.Name += fieldMemberIsAreaNode ? controllerNodeSuffix : areaNodeSuffix;
							property.GetStatements.RemoveAt(0);
							property.GetStatements.Insert(0,
							                              new CodeMethodReturnStatement(
							                              	new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)));
						}
					}
				}
			}

			type.Members.Add(property);
			type.Members.Add(field);

			constructor.Statements.Add(
				new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name),
				                        new CodeObjectCreateExpression(field.Type,
				                                                       new CodeFieldReferenceExpression(
				                                                       	new CodeThisReferenceExpression(),
				                                                       	_naming.ToMemberVariableName("services")))));
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
	}
}