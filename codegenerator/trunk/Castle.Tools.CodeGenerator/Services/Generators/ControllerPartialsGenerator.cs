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

	public class ControllerPartialsGenerator : AbstractGenerator
	{
		public ControllerPartialsGenerator(ILogger logger, ISourceGenerator source, INamingService naming,
		                                   string targetNamespace, string serviceType)
			: base(logger, source, naming, targetNamespace, serviceType)
		{
		}

		public override void Visit(ControllerTreeNode node)
		{
			var type = source.GenerateTypeDeclaration(node.Namespace, node.Name);

			var actionWrapperName = @namespace + "." + node.PathNoSlashes + naming.ToControllerWrapperName(node.Name);
			type.Members.Add(
				source.CreateReadOnlyProperty(
					"MyActions", 
					source[actionWrapperName],
				    new CodeObjectCreateExpression(
						source[actionWrapperName], 
						new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "CodeGeneratorServices"))));

			var viewWrapperName = @namespace + "." + node.PathNoSlashes + naming.ToViewWrapperName(node.Name);
			type.Members.Add(
				source.CreateReadOnlyProperty(
					"MyViews", 
					source[viewWrapperName],
					new CodeObjectCreateExpression(
						source[viewWrapperName],
						new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "CodeGeneratorServices"))));

			var routeWrapperName = @namespace + "." + node.PathNoSlashes + naming.ToRouteWrapperName(node.Name);
			type.Members.Add(
				source.CreateReadOnlyProperty(
					"MyRoutes", 
					source[routeWrapperName],
				    new CodeObjectCreateExpression(
						source[routeWrapperName],
						new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "CodeGeneratorServices"))));

			var initialize = new CodeMemberMethod
			{
				Attributes = (MemberAttributes.Override | MemberAttributes.Family),
				Name = "PerformGeneratedInitialize"
			};

			initialize.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "PerformGeneratedInitialize"));
			initialize.Statements.Add(AddPropertyToPropertyBag("MyViews"));
			initialize.Statements.Add(AddPropertyToPropertyBag("MyActions"));
			initialize.Statements.Add(AddPropertyToPropertyBag("MyRoutes"));

			type.Members.Add(initialize);

			base.Visit(node);
		}

		public override void Visit(WizardControllerTreeNode node)
		{
			Visit((ControllerTreeNode) node);
		}

		protected virtual CodeStatement AddPropertyToPropertyBag(string property)
		{
			return new CodeAssignStatement(
				new CodeIndexerExpression(
					new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "PropertyBag"),
					new CodePrimitiveExpression(property)),
				new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), property));
		}
	}
}