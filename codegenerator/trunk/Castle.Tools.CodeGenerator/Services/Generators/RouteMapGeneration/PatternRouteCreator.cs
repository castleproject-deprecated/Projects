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

namespace Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration
{
	using System;
	using System.CodeDom;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using MonoRail.Framework.Helpers;
	using MonoRail.Framework.Routing;
	using Model.TreeNodes;
	using RouteParameters;

	public class PatternRouteCreator<T> : RouteCreator<T> where T : PatternRouteTreeNode
	{
		public const string routeParametersFieldName = "routeParameters";

		private readonly RouteParameters.RouteParameters requiredRouteParameters;
		private readonly RouteParameters.RouteParameters optionalRouteParameters;
		private readonly RouteParameterDefaults routeParameterDefaults;

		public PatternRouteCreator(string @namespace, ISourceGenerator sourceGenerator, INamingService namingService,
		                           T node, CodeTypeDeclaration routeDefinitions, CodeTypeDeclaration routes)
			: base(@namespace, sourceGenerator, namingService, node, routeDefinitions, routes)
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
			routeDefinition.BaseTypes.Add(typeof (PatternRoute));
		}

		protected override CodeExpression CreateRouteDefinitionsPropertyGetter()
		{
			var newRouteDefinition = new CodeObjectCreateExpression(routeDefinition.Name);

			var defaultForArea = new CodeMethodInvokeExpression(newRouteDefinition, "DefaultForArea");
			var areaIs = new CodeMethodInvokeExpression(defaultForArea, "Is", new CodePrimitiveExpression(node.Action.Controller.Area));

			var defaultForController = new CodeMethodInvokeExpression(areaIs, "DefaultForController");
			var controllerIsRef = new CodeMethodReferenceExpression(defaultForController, "Is", new CodeTypeReference(node.Action.Controller.FullName));
			var controllerIs = new CodeMethodInvokeExpression(controllerIsRef);

			var defaultForAction = new CodeMethodInvokeExpression(controllerIs, "DefaultForAction");
			var actionIs = new CodeMethodInvokeExpression(defaultForAction, "Is", new CodePrimitiveExpression(node.Action.Name));

			var requiredParameterReferences = new CodeTypeReferenceExpression(routeDefinition.Name + ".RequiredParameters");
			var optionalParameterReferences = new CodeTypeReferenceExpression(routeDefinition.Name + ".OptionalParameters");

			CodeExpression expression = actionIs;
			expression = CreateRestrictionInitializers(requiredRouteParameters, requiredParameterReferences, expression);
			expression = CreateRestrictionInitializers(optionalRouteParameters, optionalParameterReferences, expression);

			return new CodeCastExpression(routeDefinition.Name, expression);
		}

		protected override CodeStatement[] CreateRoutesMethodBody()
		{
			var statements = new List<CodeStatement>(base.CreateRoutesMethodBody());

			statements.Insert(0, new CodeVariableDeclarationStatement(
			                     	typeof (IDictionary),
			                     	routeParametersFieldName,
			                     	new CodeMethodInvokeExpression(
			                     		new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof (DictHelper)),
			                     		                                  "Create"))));

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
				                  			? (CodeExpression) new CodePrimitiveExpression(routeParameterDefaults[routeParameter.Key])
				                  			: new CodeArgumentReferenceExpression(routeParameter.Key)
				                  		)
				                  	)
					);
			}

			var returnStatement = (CodeMethodReturnStatement) statements[statements.Count - 1];
			var invokeExpression = (CodeMethodInvokeExpression) returnStatement.Expression;
			invokeExpression.Parameters.Clear();
			invokeExpression.Parameters.Add(new CodeVariableReferenceExpression(routeParametersFieldName));

			return statements.ToArray();
		}

		protected override CodeParameterDeclarationExpression[] GetRoutesMethodParameters()
		{
			var parameters = new List<CodeParameterDeclarationExpression>();

			foreach (var requiredRouteParameter in requiredRouteParameters)
			{
				if (!routeParameterDefaults.ContainsKey(requiredRouteParameter.Key))
					parameters.Add(new CodeParameterDeclarationExpression(requiredRouteParameter.Value.RequiredMethodParameterType,
					                                                      requiredRouteParameter.Key));
			}

			foreach (var optionalRouteParameter in optionalRouteParameters)
			{
				if (!routeParameterDefaults.ContainsKey(optionalRouteParameter.Key))
					parameters.Add(new CodeParameterDeclarationExpression(optionalRouteParameter.Value.OptionalMethodParameterType,
					                                                      optionalRouteParameter.Key));
			}

			return parameters.ToArray();
		}

		protected override CodeExpression[] CreateBuildUrlParameters(CodeFieldReferenceExpression engineContext)
		{
			var expressions = base.CreateBuildUrlParameters(engineContext);
			Array.Resize(ref expressions, expressions.Length + 1);

			expressions[expressions.Length - 2] = new CodeObjectCreateExpression(typeof (Hashtable));
			expressions[expressions.Length - 1] = new CodeVariableReferenceExpression(routeParametersFieldName);

			return expressions;
		}

		protected override string RouteDefinitionPattern
		{
			get
			{
				var pattern = new Regex(@"<\w+:[^>]+>")
					.Replace(node.Pattern, match => string.Format("<{0}>", match.Value.Substring(1, match.Value.Length - 2).Split(':')[0]));

				pattern = new Regex(@"\[\w+:[^\]]+\]")
					.Replace(pattern, match => string.Format("[{0}]", match.Value.Substring(1, match.Value.Length - 2).Split(':')[0]));

				return pattern;
			}
		}

		private void CreateParameterConstantsType(string paramatersClassName, IEnumerable<KeyValuePair<string, IRouteParameterType>> routeParameters)
		{
			var parameters = sourceGenerator.GenerateTypeDeclaration(
				@namespace, paramatersClassName, routeDefinitions.Name, routeDefinition.Name);

			foreach (var routeParameter in routeParameters)
				parameters.Members.Add(sourceGenerator.NewPublicConstant(routeParameter.Key, routeParameter.Key));
		}

		private static CodeExpression CreateRestrictionInitializers(IEnumerable<KeyValuePair<string, IRouteParameterType>> routeParameters,
		                                                            CodeExpression parameterConstantsType,
		                                                            CodeExpression expression)
		{
			foreach (var routeParameter in routeParameters)
			{
				if (!routeParameter.Value.RequiresRestriction) continue;

				var propertyRef = new CodePropertyReferenceExpression(parameterConstantsType, routeParameter.Key);
				var restrict = new CodeMethodInvokeExpression(expression, "Restrict", propertyRef);

				if (routeParameter.Value is StringRouteParameterType)
				{
					var stringRouteParameterType = (StringRouteParameterType) routeParameter.Value;
					var expressions = new List<CodeExpression>();

					foreach (var choice in stringRouteParameterType.anyOf)
						expressions.Add(new CodePrimitiveExpression(choice));

					expression = new CodeMethodInvokeExpression(restrict, "AnyOf", expressions.ToArray());
				}
				else if (routeParameter.Value is GuidRouteParameterType)
					expression = new CodePropertyReferenceExpression(restrict, "ValidGuid");
				else if (routeParameter.Value is IntRouteParameterType)
					expression = new CodePropertyReferenceExpression(restrict, "ValidInteger");
			}

			return expression;
		}
	}
}