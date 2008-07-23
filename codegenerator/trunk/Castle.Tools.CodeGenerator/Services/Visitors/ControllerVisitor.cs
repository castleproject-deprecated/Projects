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

namespace Castle.Tools.CodeGenerator.Services.Visitors
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using MonoRail.Framework;
	using Model.TreeNodes;
	using ICSharpCode.NRefactory.Ast;
	
	public class ControllerVisitor : TypeResolvingVisitor
	{
		private readonly ILogger logger;
		private readonly ITreeCreationService treeService;

		public ControllerVisitor(ILogger logger, ITypeResolver typeResolver, ITreeCreationService treeService)
			: base(typeResolver)
		{
			this.logger = logger;
			this.treeService = treeService;
		}

		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if ((methodDeclaration.Modifier & Modifiers.Public) != Modifiers.Public)
				return null;
			
			var controllerNode = (ControllerTreeNode) treeService.Peek;

			if (controllerNode is WizardControllerTreeNode)
			{
				var wizardControllerInterface = typeof (IWizardController);
				var methodInfos = wizardControllerInterface.GetMethods(BindingFlags.Public | BindingFlags.Instance);

				if ((methodDeclaration.Name == "GetSteps") && (methodDeclaration.Body.Children.Count > 0))
				{
					(controllerNode as WizardControllerTreeNode).WizardStepPages = GetWizardStepPages(methodDeclaration.Body);
					return null;
				}
				
				if (Array.Exists(methodInfos, methodInfo => methodInfo.Name == methodDeclaration.Name))
					return null;
			}

			var action = new ActionTreeNode(methodDeclaration.Name);

			foreach (var parameter in methodDeclaration.Parameters)
			{
				var type = TypeResolver.Resolve(parameter.TypeReference, true);
				action.AddChild(new ParameterTreeNode(parameter.ParameterName, type));
			}

			foreach (var attributeSection in methodDeclaration.Attributes)
			{
				var attributes = attributeSection.Attributes.FindAll(attribute => attribute.Name == "StaticRoute");

				foreach (var attribute in attributes)
				{
					var name = (PrimitiveExpression) attribute.PositionalArguments[0];
					var pattern = (PrimitiveExpression) attribute.PositionalArguments[1];
					var routeTreeNode = new StaticRouteTreeNode((string) name.Value, (string) pattern.Value);

					action.AddChild(routeTreeNode);
				}

				attributes = attributeSection.Attributes.FindAll(attribute => attribute.Name == "PatternRoute");

				foreach (var attribute in attributes)
				{
					var name = (PrimitiveExpression) attribute.PositionalArguments[0];
					var pattern = (PrimitiveExpression) attribute.PositionalArguments[1];
					var defaults = new string[attribute.PositionalArguments.Count - 2];

					for (var i = 0; i < defaults.Length; i++)
						defaults[i] = (string) ((PrimitiveExpression) attribute.PositionalArguments[2 + i]).Value;

					var routeTreeNode = new PatternRouteTreeNode((string) name.Value, (string) pattern.Value, defaults);
					
					action.AddChild(routeTreeNode);
				}
			}

			controllerNode.AddChild(action, true);

			return base.VisitMethodDeclaration(methodDeclaration, data);
		}


		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (!IsController(typeDeclaration))
			{
				return null;
			}

			var area = GetArea(typeDeclaration);
			var typeNamespace = GetNamespace(typeDeclaration);

			if (!String.IsNullOrEmpty(area))
			{
				var areas = area.Split('/');
				
				for (var i = 0; i < areas.Length; i++)
				{
					var areaNode = treeService.FindNode(areas[i]) ?? new AreaTreeNode(areas[i]);

					treeService.PushNode(areaNode);
				}
			}

			var node = IsWizardController(typeDeclaration)
              	? new WizardControllerTreeNode(typeDeclaration.Name, typeNamespace, new string[0])
              	: new ControllerTreeNode(typeDeclaration.Name, typeNamespace);

			treeService.PushNode(node);

			var r = base.VisitTypeDeclaration(typeDeclaration, data);

			if (!String.IsNullOrEmpty(area))
			{
				var areas = area.Split('/');
				
				for (var i = 0; i < areas.Length; i++)
					treeService.PopNode();
			}

			treeService.PopNode();

			return r;
		}

		protected virtual string GetArea(TypeDeclaration typeDeclaration)
		{
			foreach (var attributeSection in typeDeclaration.Attributes)
				foreach (var attribute in attributeSection.Attributes)
				{
					if (attribute.Name != "ControllerDetails") continue;

					foreach (var namedArgument in attribute.NamedArguments)
						if (namedArgument.Name == "Area")
							return ResolvePrimitiveValue(namedArgument.Expression);
				}
			
			return null;
		}

		protected virtual string[] GetWizardStepPages(BlockStatement getStepsBody)
		{
			var returnStatement = (ReturnStatement) getStepsBody.Children[getStepsBody.Children.Count - 1];
			var arrayCreateExpression = (ArrayCreateExpression) returnStatement.Expression;
			var types = new List<string>();

			arrayCreateExpression.ArrayInitializer.CreateExpressions.ForEach(expression =>
			{
				if (expression is ObjectCreateExpression)
				{
					var objectCreateExpression = (ObjectCreateExpression) expression;

					types.Add(objectCreateExpression.CreateType.Type);
				}
				else if (expression is InvocationExpression)
				{
					var invocationExpression = (InvocationExpression) expression;

					if (invocationExpression.TypeArguments.Count == 1)
						types.Add(invocationExpression.TypeArguments[0].Type);
				}
			});

			return types.ToArray();
		}

		protected virtual bool IsController(TypeDeclaration typeDeclaration)
		{
			var isController = typeDeclaration.Name.EndsWith("Controller") && (typeDeclaration.Modifier & Modifiers.Partial) == Modifiers.Partial;

			if (!isController)
			{
				if ((typeDeclaration.Modifier & Modifiers.Partial) != Modifiers.Partial)
				{
					logger.LogInfo("Controller Source for " + typeDeclaration.Name +
					                " will not be included in the generated sitemap because it is not a partial type");
				}
				
				if (!typeDeclaration.Name.EndsWith("Controller"))
				{
					logger.LogInfo("Controller source for " + typeDeclaration.Name +
					                " will not be included in the generated site map because its type name does not end with controller");
				}
			}

			return isController;
		}

		protected virtual bool IsWizardController(TypeDeclaration typeDeclaration)
		{
			return typeDeclaration.BaseTypes.Exists(typeReference => typeReference.ToString() == "IWizardController");
		}

		protected virtual string ResolvePrimitiveValue(Expression expression)
		{
			return expression is PrimitiveExpression ? ((PrimitiveExpression) expression).Value.ToString() : null;
		}
	}
}