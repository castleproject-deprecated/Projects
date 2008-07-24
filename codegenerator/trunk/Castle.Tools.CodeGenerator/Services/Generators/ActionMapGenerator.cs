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
	using System.Collections.Generic;
	using External;
	using Model;
	using Model.TreeNodes;
	using Generators;

	public class ActionMapGenerator : AbstractGenerator
	{
		private Dictionary<string, short> occurences;

		public ActionMapGenerator(ILogger logger, ISourceGenerator source, INamingService naming, string targetNamespace,
		                          string serviceType)
			: base(logger, source, naming, targetNamespace, serviceType)
		{
		}

		public override void Visit(ControllerTreeNode node)
		{
			var type = GenerateTypeDeclaration(@namespace, node.PathNoSlashes + naming.ToActionWrapperName(node.Name));

			occurences = new Dictionary<string, short>();

			// We can only generate empty argument methods for actions that appear once and only once.
			foreach (var child in node.Children)
			{
				if (!(child is ActionTreeNode)) continue;

				if (!occurences.ContainsKey(child.Name))
					occurences[child.Name] = 0;
				
				occurences[child.Name]++;
			}

			typeStack.Push(type);
			base.Visit(node);
			typeStack.Pop();
		}

		public override void Visit(ActionTreeNode node)
		{
			var type = typeStack.Peek();
			var actionArgumentTypes = new List<string>();

			var method = new CodeMemberMethod
			{
				Name = node.Name,
				ReturnType = source[typeof (IControllerActionReference)],
				Attributes = MemberAttributes.Public
			};

			method.CustomAttributes.Add(source.DebuggerAttribute);
			
			var actionArguments = CreateActionArgumentsAndAddParameters(method, node, actionArgumentTypes);
			
			method.Statements.Add(new CodeMethodReturnStatement(CreateNewActionReference(node, actionArguments, actionArgumentTypes)));
			type.Members.Add(method);

			if (actionArguments.Count > 0 && occurences[node.Name] == 1)
			{
				method = new CodeMemberMethod
				{
					Name = node.Name,
					ReturnType = source[typeof (IArgumentlessControllerActionReference)],
					Attributes = MemberAttributes.Public
				};

				method.CustomAttributes.Add(source.DebuggerAttribute);
				method.Comments.Add(new CodeCommentStatement("Empty argument Action... Not sure if we want to pass MethodInformation to these..."));
				method.Statements.Add(new CodeMethodReturnStatement(CreateNewActionReference(node, new List<CodeExpression>(), actionArgumentTypes)));
				
				type.Members.Add(method);
			}

			base.Visit(node);
		}

		public override void Visit(WizardControllerTreeNode node)
		{
			Visit((ControllerTreeNode) node);
		}

		protected CodeExpression CreateNewActionReference(ActionTreeNode node, List<CodeExpression> actionArguments, List<string> actionArgumentTypes)
		{
			var actionArgumentRuntimeTypes = new List<CodeExpression>();
			foreach (var type in actionArgumentTypes)
				actionArgumentRuntimeTypes.Add(new CodeTypeOfExpression(source[type]));
			
			CodeExpression createMethodSignature = new CodeObjectCreateExpression(
				source[typeof (MethodSignature)],
				new CodeExpression[]
				{
					new CodeTypeOfExpression(node.Controller.FullName),
					new CodePrimitiveExpression(node.Name),
					new CodeArrayCreateExpression(source[typeof (Type)], actionArgumentRuntimeTypes.ToArray())
				});

			var constructionArguments = new[]
			{
				new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), naming.ToMemberVariableName(serviceIdentifier)),
				new CodeTypeOfExpression(node.Controller.FullName),
				new CodePrimitiveExpression(node.Controller.Area),
				new CodePrimitiveExpression(naming.ToControllerName(node.Controller.Name)),
				new CodePrimitiveExpression(node.Name),
				createMethodSignature,
				new CodeArrayCreateExpression(source[typeof (ActionArgument)], actionArguments.ToArray())
			};

			return new CodeMethodInvokeExpression(
				new CodeMethodReferenceExpression(
					new CodePropertyReferenceExpression(
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), naming.ToMemberVariableName(serviceIdentifier)),
						"ControllerReferenceFactory"),
					"CreateActionReference"),
				constructionArguments);
		}

		protected List<CodeExpression> CreateActionArgumentsAndAddParameters(CodeMemberMethod method, ActionTreeNode node, List<string> actionArgumentTypes)
		{
			var actionArguments = new List<CodeExpression>();
			var index = 0;
			var parameters = node.Children.FindAll(t => t is ParameterTreeNode);

			foreach (ParameterTreeNode parameterInfo in parameters)
			{
				var newParameter = new CodeParameterDeclarationExpression
				{
					Name = parameterInfo.Name,
					Type = source[parameterInfo.Type]
				};

				method.Parameters.Add(newParameter);
				actionArgumentTypes.Add(parameterInfo.Type);

				var argumentCreate =
					new CodeObjectCreateExpression(source[typeof (ActionArgument)], new CodeExpression[]
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

		/*
		I opted to only get this information when it was necessary, but decided to check this in at least once because it might be useful. -jlewalle
		protected string CreateMethodInformation(ActionTreeNode node, CodeTypeDeclaration type, List<string> actionArgumentTypes)
		{
		  string methodInfoName = naming.ToMethodSignatureName(node.Name, actionArgumentTypes.ToArray());
		  string memberName = naming.ToMemberVariableName(methodInfoName);
		  CodeMemberField field = new CodeMemberField(source[typeof(MethodInformation)], memberName);
		  field.Attributes = MemberAttributes.Family;
		  type.Members.Add(field);

		  List<CodeExpression> actionArgumentRuntimeTypes = new List<CodeExpression>();
		  foreach (string typeName in actionArgumentTypes)
		  {
			actionArgumentRuntimeTypes.Add(new CodeTypeOfExpression(source[typeName]));
		  }
		  constructor.Statements.Add(
			new CodeAssignStatement(
			  new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), memberName),
			  new CodeMethodInvokeExpression(
				new CodeMethodReferenceExpression(
				  new CodePropertyReferenceExpression(
					new CodeArgumentReferenceExpression(naming.ToVariableName(serviceIdentifier)), "RuntimeInformationService"
				  ),
				  "ResolveMethodInformation"
				),
				new CodeExpression[]
				{
				  new CodeTypeOfExpression(node.Controller.FullName),
				  new CodePrimitiveExpression(node.Name),
				  new CodeArrayCreateExpression(source[typeof(Type)], actionArgumentRuntimeTypes.ToArray())
				}
			  )
			)
		  );
		  return memberName;
		}
		*/
	}
}