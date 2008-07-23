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
	using External;
	using Model;
	using Model.TreeNodes;
	using Generators;

	public class WizardStepMapGenerator : AbstractGenerator
	{
		public WizardStepMapGenerator(ILogger logger, ISourceGenerator source, INamingService naming, string targetNamespace,
		                              string serviceType) : base(logger, source, naming, targetNamespace, serviceType)
		{
		}

		public override void Visit(WizardControllerTreeNode node)
		{
			var type = GenerateTypeDeclaration(@namespace, node.PathNoSlashes + naming.ToWizardStepWrapperName(node.Name));

			foreach (var wizardStepPage in node.WizardStepPages)
			{
				var method = new CodeMemberMethod
				{
					Name = wizardStepPage,
					ReturnType = source[typeof (IControllerActionReference)],
					Attributes = MemberAttributes.Public
				};

				method.CustomAttributes.Add(source.DebuggerAttribute);
				method.Statements.Add(new CodeMethodReturnStatement(CreateNewWizardStepReference(node, wizardStepPage)));
				type.Members.Add(method);
			}

			base.Visit(node);
		}

		protected CodeExpression CreateNewWizardStepReference(WizardControllerTreeNode node, string wizardStepPage)
		{
			CodeExpression createMethodSignature = new CodeObjectCreateExpression(
				source[typeof (MethodSignature)],
				new CodeExpression[]
				{
					new CodeTypeOfExpression(node.FullName),
					new CodePrimitiveExpression(node.Name),
					new CodeArrayCreateExpression(source[typeof (Type)], 0)
				});

			var constructionArguments = new[]
			{
				new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), naming.ToMemberVariableName(serviceIdentifier)),
				new CodeTypeOfExpression(node.FullName),
				new CodePrimitiveExpression(node.Area),
				new CodePrimitiveExpression(naming.ToControllerName(node.Name)),
				new CodePrimitiveExpression(wizardStepPage),
				createMethodSignature,
				new CodeArrayCreateExpression(source[typeof (ActionArgument)], 0)
			};

			return new CodeMethodInvokeExpression(
				new CodeMethodReferenceExpression(
					new CodePropertyReferenceExpression(
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), naming.ToMemberVariableName(serviceIdentifier)),
						"ControllerReferenceFactory"),
					"CreateActionReference"),
				constructionArguments);
		}
	}
}