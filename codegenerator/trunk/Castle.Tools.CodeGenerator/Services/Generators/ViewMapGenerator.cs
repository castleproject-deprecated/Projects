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
	using External;
	using Model.TreeNodes;
	using Generators;

	public class ViewMapGenerator : AbstractGenerator
	{
		public ViewMapGenerator(ILogger logger, ISourceGenerator source, INamingService naming, string targetNamespace,
		                        string serviceType)
			: base(logger, source, naming, targetNamespace, serviceType)
		{
		}

		public override void Visit(ControllerTreeNode node)
		{
			var type = GenerateTypeDeclaration(@namespace, node.PathNoSlashes + naming.ToViewWrapperName(node.Name));

			typeStack.Push(type);
			base.Visit(node);
			typeStack.Pop();
		}

		public override void Visit(ViewTreeNode node)
		{
			if (typeStack.Count == 0) return;

			var constructionArguments = new CodeExpression[]
			{
				new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), naming.ToMemberVariableName(serviceIdentifier)),
				new CodeTypeOfExpression(node.Controller.FullName),
				new CodePrimitiveExpression(node.Controller.Area),
				new CodePrimitiveExpression(naming.ToControllerName(node.Controller.Name)),
				new CodePrimitiveExpression(node.Name)
			};

			CodeExpression returnExpression =
				new CodeMethodInvokeExpression(
					new CodeMethodReferenceExpression(
						new CodePropertyReferenceExpression(
							new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), naming.ToMemberVariableName(serviceIdentifier)),
							"ControllerReferenceFactory"),
						"CreateViewReference"),
					constructionArguments);

			var propertyType = new CodeTypeReference(typeof (IControllerViewReference));
			typeStack.Peek().Members.Add(source.CreateReadOnlyProperty(node.Name, propertyType, returnExpression));

			base.Visit(node);
		}

		public override void Visit(WizardControllerTreeNode node)
		{
			Visit((ControllerTreeNode) node);
		}
	}
}