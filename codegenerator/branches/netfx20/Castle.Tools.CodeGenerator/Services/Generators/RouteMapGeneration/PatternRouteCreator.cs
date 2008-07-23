using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.Framework.Routing;
using Castle.Tools.CodeGenerator.Model.TreeNodes;
using Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration.RouteParameters;

namespace Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration
{
	public class PatternRouteCreator : RouteCreator<PatternRouteTreeNode>
	{
		public const string routeParametersFieldName = "routeParameters";

		private readonly RouteParameters.RouteParameters requiredRouteParameters;
		private readonly RouteParameters.RouteParameters optionalRouteParameters;
		private readonly RouteParameterDefaults routeParameterDefaults;

		public PatternRouteCreator(string @namespace, ISourceGenerator sourceGenerator, INamingService namingService, PatternRouteTreeNode node, CodeTypeDeclaration routeDefinitions, CodeTypeDeclaration routes) : base(@namespace, sourceGenerator, namingService, node, routeDefinitions, routes)
		{
			requiredRouteParameters = new RequiredRouteParameters().GetFrom(node.Pattern);
			optionalRouteParameters = new OptionalRouteParameters().GetFrom(node.Pattern);
			routeParameterDefaults = new RouteParameterDefaults().GetFrom(node.Defaults);
		}

		protected override void CreateRouteDefinition()
		{
			base.CreateRouteDefinition();

			CreateParameterConstantsType("RequiredParameters", requiredRouteParameters);
			CreateParameterConstantsType("OptionalParameters", optionalRouteParameters);
		}

		protected override void AddRouteDefinitionBaseType()
		{
			routeDefinition.BaseTypes.Add(typeof(PatternRoute));
		}

		protected override CodeExpression CreateRouteDefinitionsPropertyGetter()
		{
			CodeObjectCreateExpression newRouteDefinition = new CodeObjectCreateExpression(routeDefinition.Name);

			CodeMethodInvokeExpression defaultForArea = new CodeMethodInvokeExpression(newRouteDefinition, "DefaultForArea");
			CodeMethodInvokeExpression areaIs = new CodeMethodInvokeExpression(defaultForArea, "Is", new CodePrimitiveExpression(node.Action.Controller.Area));

			CodeMethodInvokeExpression defaultForController = new CodeMethodInvokeExpression(areaIs, "DefaultForController");
			CodeMethodReferenceExpression controllerIsRef = new CodeMethodReferenceExpression(defaultForController, "Is", new CodeTypeReference(node.Action.Controller.FullName));
			CodeMethodInvokeExpression controllerIs = new CodeMethodInvokeExpression(controllerIsRef);

			CodeMethodInvokeExpression defaultForAction = new CodeMethodInvokeExpression(controllerIs, "DefaultForAction");
			CodeMethodInvokeExpression actionIs = new CodeMethodInvokeExpression(defaultForAction, "Is", new CodePrimitiveExpression(node.Action.Name));

			CodeTypeReferenceExpression requiredParameterReferences = new CodeTypeReferenceExpression(routeDefinition.Name + ".RequiredParameters");
			CodeTypeReferenceExpression optionalParameterReferences = new CodeTypeReferenceExpression(routeDefinition.Name + ".OptionalParameters");

			CodeExpression expression = actionIs;
			expression = CreateRestrictionInitializers(requiredRouteParameters, requiredParameterReferences, expression);
			expression = CreateRestrictionInitializers(optionalRouteParameters, optionalParameterReferences, expression);

			return new CodeCastExpression(routeDefinition.Name, expression);
		}

		protected override CodeStatement[] CreateRoutesMethodBody()
		{
			List<CodeStatement> statements = new List<CodeStatement>(base.CreateRoutesMethodBody());			

			statements.Insert(0, new CodeVariableDeclarationStatement(
				typeof(IDictionary), 
				routeParametersFieldName,
				new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(DictHelper)), "Create"))));

			foreach (KeyValuePair<string, IRouteParameterType> routeParameter in requiredRouteParameters)
			{
				statements.Insert(statements.Count - 1, 
					new CodeExpressionStatement(
						new CodeMethodInvokeExpression(
							new CodeMethodReferenceExpression(
								new CodeVariableReferenceExpression(routeParametersFieldName),
								"Add"
							),
							new CodePrimitiveExpression(routeParameter.Key),
							routeParameterDefaults.ContainsKey(routeParameter.Key) 
								? (CodeExpression) new CodePrimitiveExpression(routeParameterDefaults[routeParameter.Key])
								: new CodeArgumentReferenceExpression(routeParameter.Key)
						)
					)
				);
			}

			foreach (KeyValuePair<string, IRouteParameterType> routeParameter in optionalRouteParameters)
			{
				statements.Insert(statements.Count - 1,
					new CodeExpressionStatement(
						new CodeMethodInvokeExpression(
							new CodeMethodReferenceExpression(
								new CodeVariableReferenceExpression(routeParametersFieldName),
								"Add"
							),
							new CodePrimitiveExpression(routeParameter.Key),
							routeParameterDefaults.ContainsKey(routeParameter.Key)
								? (CodeExpression)new CodePrimitiveExpression(routeParameterDefaults[routeParameter.Key])
								: new CodeArgumentReferenceExpression(routeParameter.Key)
						)
					)
				);
			}
			
			CodeMethodReturnStatement returnStatement = (CodeMethodReturnStatement) statements[statements.Count - 1];
			CodeMethodInvokeExpression invokeExpression = (CodeMethodInvokeExpression) returnStatement.Expression;
			invokeExpression.Parameters.Clear();
			invokeExpression.Parameters.Add(new CodeVariableReferenceExpression(routeParametersFieldName));
			
			return statements.ToArray();
		}

		protected override CodeParameterDeclarationExpression[] GetRoutesMethodParameters()
		{
			List<CodeParameterDeclarationExpression> parameters = new List<CodeParameterDeclarationExpression>();

			foreach (KeyValuePair<string, IRouteParameterType> requiredRouteParameter in requiredRouteParameters)
			{
				if (!routeParameterDefaults.ContainsKey(requiredRouteParameter.Key))
					parameters.Add(new CodeParameterDeclarationExpression(requiredRouteParameter.Value.RequiredMethodParameterType, requiredRouteParameter.Key));
			}

			foreach (KeyValuePair<string, IRouteParameterType> optionalRouteParameter in optionalRouteParameters)
			{
				if (!routeParameterDefaults.ContainsKey(optionalRouteParameter.Key))
					parameters.Add(new CodeParameterDeclarationExpression(optionalRouteParameter.Value.OptionalMethodParameterType, optionalRouteParameter.Key));
			}

			return parameters.ToArray();
		}

		protected override CodeExpression[] CreateBuildUrlParameters(CodeFieldReferenceExpression engineContext)
		{
			CodeExpression[] expressions = base.CreateBuildUrlParameters(engineContext);
			Array.Resize(ref expressions, expressions.Length + 1);

			expressions[expressions.Length - 2] = new CodeObjectCreateExpression(typeof (Hashtable));
			expressions[expressions.Length - 1] = new CodeVariableReferenceExpression(routeParametersFieldName);

			return expressions;
		}

		protected override string RouteDefinitionPattern
		{
			get 
			{
				string pattern = new Regex(@"<\w+:[^>]+>").Replace(node.Pattern, delegate(Match match)
				{
					return string.Format("<{0}>", match.Value.Substring(1, match.Value.Length - 2).Split(':')[0]);
				});

				pattern = new Regex(@"\[\w+:[^\]]+\]").Replace(pattern, delegate(Match match)
				{
					return string.Format("[{0}]", match.Value.Substring(1, match.Value.Length - 2).Split(':')[0]);
				});

				return pattern;
			}
		}

		private void CreateParameterConstantsType(string paramatersClassName, RouteParameters.RouteParameters routeParameters)
		{
			CodeTypeDeclaration parameters = sourceGenerator.GenerateTypeDeclaration(@namespace, paramatersClassName, routeDefinitions.Name, routeDefinition.Name);

			foreach (KeyValuePair<string, IRouteParameterType> routeParameter in routeParameters)
				parameters.Members.Add(sourceGenerator.NewPublicConstant(routeParameter.Key, routeParameter.Key));
		}

		private static CodeExpression CreateRestrictionInitializers(RouteParameters.RouteParameters routeParameters, CodeTypeReferenceExpression parameterConstantsType, CodeExpression expression)
		{
			foreach (KeyValuePair<string, IRouteParameterType> routeParameter in routeParameters)
			{
				if (routeParameter.Value.RequiresRestriction)
				{
					CodePropertyReferenceExpression propertyRef = new CodePropertyReferenceExpression(parameterConstantsType, routeParameter.Key);
					CodeMethodInvokeExpression restrict = new CodeMethodInvokeExpression(expression, "Restrict", propertyRef);

					if (routeParameter.Value is StringRouteParameterType)
					{
						StringRouteParameterType stringRouteParameterType = (StringRouteParameterType) routeParameter.Value;
						List<CodeExpression> expressions = new List<CodeExpression>();

						foreach (string choice in stringRouteParameterType.anyOf)
							expressions.Add(new CodePrimitiveExpression(choice));
						
						expression = new CodeMethodInvokeExpression(restrict, "AnyOf", expressions.ToArray());
					}
					else if (routeParameter.Value is GuidRouteParameterType)
					{
						expression = new CodePropertyReferenceExpression(restrict, "ValidGuid");
					}
					else if (routeParameter.Value is IntRouteParameterType)
					{
						expression = new CodePropertyReferenceExpression(restrict, "ValidInteger");
					}
				}
			}

			return expression;
		}

		//public static void For(PatternRouteTreeNode node)
		//{
		//    CreateRouteDefinition(node, requiredParameters, optionalParameters);
		//    CreateRouteProperty(node, requiredParameters, optionalParameters);
		//    CreateRouteBuilder(node, requiredParameters, optionalParameters);
		//}
	}
}