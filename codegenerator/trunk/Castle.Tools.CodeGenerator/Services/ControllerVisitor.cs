using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Castle.MonoRail.Framework;
using Castle.Tools.CodeGenerator.Model;
using ICSharpCode.NRefactory.Ast;
using Attribute=ICSharpCode.NRefactory.Ast.Attribute;

namespace Castle.Tools.CodeGenerator.Services
{
	public class ControllerVisitor : TypeResolvingVisitor
	{
		private ILogger _logger;
		private ITreeCreationService _treeService;

		public ControllerVisitor(ILogger logger, ITypeResolver typeResolver, ITreeCreationService treeService)
			: base(typeResolver)
		{
			_logger = logger;
			_treeService = treeService;
		}

		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if ((methodDeclaration.Modifier & Modifiers.Public) != Modifiers.Public)
			{
				return null;
			}

			ControllerTreeNode controllerNode = (ControllerTreeNode) _treeService.Peek;

			if (controllerNode is WizardControllerTreeNode)
			{
				Type wizardControllerInterface = typeof (IWizardController);

				MethodInfo[] methodInfos = wizardControllerInterface.GetMethods(BindingFlags.Public | BindingFlags.Instance);

				if ((methodDeclaration.Name == "GetSteps") && (methodDeclaration.Body.Children.Count > 0))
				{
					(controllerNode as WizardControllerTreeNode).WizardStepPages = GetWizardStepPages(methodDeclaration.Body);

					return null;
				}
				else if (
					Array.Exists(methodInfos, delegate(MethodInfo methodInfo) { return methodInfo.Name == methodDeclaration.Name; }))
					return null;
			}

			ActionTreeNode action = new ActionTreeNode(methodDeclaration.Name);

			foreach (ParameterDeclarationExpression parameter in methodDeclaration.Parameters)
			{
				string typeName = TypeResolver.Resolve(parameter.TypeReference);
				action.AddChild(new ParameterTreeNode(parameter.ParameterName, typeName));
			}

			foreach (AttributeSection attributeSection in methodDeclaration.Attributes)
			{
				List<Attribute> attributes = attributeSection.Attributes.FindAll(
					delegate(Attribute attribute) { return attribute.Name == "Route"; });

				foreach (Attribute attribute in attributes)
				{
					PrimitiveExpression order = (PrimitiveExpression) attribute.PositionalArguments[0];
					PrimitiveExpression name = (PrimitiveExpression) attribute.PositionalArguments[1];
					PrimitiveExpression pattern = (PrimitiveExpression) attribute.PositionalArguments[2];

					RouteTreeNode routeTreeNode = new RouteTreeNode((int) order.Value, (string) name.Value, (string) pattern.Value);
					action.AddChild(routeTreeNode);

					Regex regex = new Regex(@"^((/)*(\w+|(?<parameter>\<\w+:\w+\>)))+(/)*$");
					Match match = regex.Match(routeTreeNode.Pattern);
					Group parameters = match.Groups["parameter"];

					for (int i = 0; i < parameters.Captures.Count; i++)
					{
						Capture capture = parameters.Captures[i];
						string[] parts = capture.Value.Split(':');
						string paramName = parts[0].Substring(1);
						string paramType = parts[1].Substring(0, parts[1].Length - 1);

						if (paramType.ToLowerInvariant() == "string")
							paramType = typeof(string).ToString();
						else if (paramType.ToLowerInvariant() == "int")
							paramType = typeof(int).ToString();

						routeTreeNode.AddChild(new ParameterTreeNode(paramName, paramType));
					}
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

			string area = GetArea(typeDeclaration);
			string typeNamespace = GetNamespace(typeDeclaration);

			if (!String.IsNullOrEmpty(area))
			{
				string[] areas = area.Split('/');
				for (int i = 0; i < areas.Length; i++)
				{
					TreeNode areaNode = _treeService.FindNode(areas[i]);
					if (areaNode == null)
					{
						areaNode = new AreaTreeNode(areas[i]);
					}
					_treeService.PushNode(areaNode);
				}
			}

			ControllerTreeNode node = IsWizardController(typeDeclaration)
			                          	? new WizardControllerTreeNode(typeDeclaration.Name, typeNamespace, new string[0])
			                          	: new ControllerTreeNode(typeDeclaration.Name, typeNamespace);

			_treeService.PushNode(node);

			object r = base.VisitTypeDeclaration(typeDeclaration, data);

			if (!String.IsNullOrEmpty(area))
			{
				string[] areas = area.Split('/');
				for (int i = 0; i < areas.Length; i++)
				{
					_treeService.PopNode();
				}
			}
			_treeService.PopNode();

			return r;
		}

		protected virtual string GetArea(TypeDeclaration typeDeclaration)
		{
			foreach (AttributeSection attributeSection in typeDeclaration.Attributes)
			{
				foreach (Attribute attribute in attributeSection.Attributes)
				{
					if (attribute.Name == "ControllerDetails")
					{
						foreach (NamedArgumentExpression namedArgument in attribute.NamedArguments)
						{
							if (namedArgument.Name == "Area")
							{
								return ResolvePrimitiveValue(namedArgument.Expression);
							}
						}
					}
				}
			}
			return null;
		}

		protected virtual string[] GetWizardStepPages(BlockStatement getStepsBody)
		{
			ReturnStatement returnStatement = (ReturnStatement) getStepsBody.Children[getStepsBody.Children.Count - 1];
			ArrayCreateExpression arrayCreateExpression = (ArrayCreateExpression) returnStatement.Expression;
			List<string> types = new List<string>();

			arrayCreateExpression.ArrayInitializer.CreateExpressions.ForEach(delegate(Expression expression)
			                                                                 	{
			                                                                 		if (expression is ObjectCreateExpression)
			                                                                 		{
			                                                                 			ObjectCreateExpression objectCreateExpression =
			                                                                 				(ObjectCreateExpression) expression;

			                                                                 			types.Add(objectCreateExpression.CreateType.Type);
			                                                                 		}
			                                                                 		else if (expression is InvocationExpression)
			                                                                 		{
			                                                                 			InvocationExpression invocationExpression =
			                                                                 				(InvocationExpression) expression;

			                                                                 			if (invocationExpression.TypeArguments.Count == 1)
			                                                                 				types.Add(
			                                                                 					invocationExpression.TypeArguments[0].Type);
			                                                                 		}
			                                                                 	});

			return types.ToArray();
		}

		protected virtual bool IsController(TypeDeclaration typeDeclaration)
		{
			bool isController =
				typeDeclaration.Name.EndsWith("Controller") && (typeDeclaration.Modifier & Modifiers.Partial) == Modifiers.Partial;

			if (!isController)
			{
				if ((typeDeclaration.Modifier & Modifiers.Partial) != Modifiers.Partial)
				{
					_logger.LogInfo("Controller Source for " + typeDeclaration.Name +
					                " will not be included in the generated sitemap because it is not a partial type");
				}
				if (!typeDeclaration.Name.EndsWith("Controller"))
				{
					_logger.LogInfo("Controller source for " + typeDeclaration.Name +
					                " will not be included in the generated site map because its type name does not end with controller");
				}
			}

			return isController;
		}

		protected virtual bool IsWizardController(TypeDeclaration typeDeclaration)
		{
			return typeDeclaration.BaseTypes.Exists(
				delegate(TypeReference typeReference) { return typeReference.ToString() == "IWizardController"; });
		}

		protected virtual string ResolvePrimitiveValue(Expression expression)
		{
			if (expression is PrimitiveExpression)
				return ((PrimitiveExpression) expression).Value.ToString();
			return null;
		}
	}
}
