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
	using System.CodeDom;
	using Model.TreeNodes;
	using Generators;

	public class ControllerMapGenerator : AbstractGenerator
	{
		public ControllerMapGenerator(ILogger logger, ISourceGenerator source, INamingService naming, string targetNamespace,
		                              string serviceType)
			: base(logger, source, naming, targetNamespace, serviceType)
		{
		}

		public override void Visit(AreaTreeNode node)
		{
			var type = GenerateTypeDeclaration(@namespace, node.PathNoSlashes + naming.ToAreaWrapperName(node.Name));

			if (typeStack.Count > 0)
			{
				var parent = typeStack.Peek();
				source.AddFieldPropertyConstructorInitialize(parent, node.Name.Replace("/", ""), type.Name);
			}

			typeStack.Push(type);
			base.Visit(node);
			typeStack.Pop();
		}

		public override void Visit(ControllerTreeNode node)
		{
			var type = source.GenerateTypeDeclaration(@namespace, node.PathNoSlashes + naming.ToControllerWrapperName(node.Name));
			var constructor = CreateServicesConstructor();
			constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression(naming.ToVariableName(serviceIdentifier)));
			
			type.BaseTypes.Add(source[node.PathNoSlashes + naming.ToActionWrapperName(node.Name)]);
			type.Members.Add(constructor);

			var parent = typeStack.Peek();
			source.AddFieldPropertyConstructorInitialize(parent, naming.ToControllerName(node.Name), type.Name);

			type.Members.Add(
				source.CreateReadOnlyProperty(
					"Views",
					new CodeTypeReference(node.PathNoSlashes + naming.ToViewWrapperName(node.Name)),
					new CodeObjectCreateExpression(
						new CodeTypeReference(node.PathNoSlashes + naming.ToViewWrapperName(node.Name)),
						new CodeFieldReferenceExpression(source.This, naming.ToMemberVariableName(serviceIdentifier)))));
			
			type.Members.Add(
				source.CreateReadOnlyProperty("Actions", 
												new CodeTypeReference(node.PathNoSlashes + naming.ToActionWrapperName(node.Name)), source.This));
		}

		public override void Visit(WizardControllerTreeNode node)
		{
			Visit((ControllerTreeNode) node);

			var codeNamespace = source.LookupNamespace(@namespace);

			foreach (CodeTypeDeclaration type in codeNamespace.Types)
			{
				if (type.Name != (node.PathNoSlashes + naming.ToControllerWrapperName(node.Name))) continue;

				type.Members.Add(
					source.CreateReadOnlyProperty(
						"Steps",
						new CodeTypeReference(node.PathNoSlashes + naming.ToWizardStepWrapperName(node.Name)),
						new CodeObjectCreateExpression(
							new CodeTypeReference(node.PathNoSlashes + naming.ToWizardStepWrapperName(node.Name)),
							new CodeFieldReferenceExpression(source.This, naming.ToMemberVariableName(serviceIdentifier)))));

				break;
			}
		}
	}
}