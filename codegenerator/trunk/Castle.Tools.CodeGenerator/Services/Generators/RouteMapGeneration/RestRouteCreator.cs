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

using Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration.RouteParameters;

namespace Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration
{
	using System.CodeDom;
	using MonoRail.Rest;
	using Model.TreeNodes;
	using System.Linq;

	public class RestRouteCreator : PatternRouteCreator<RestRouteTreeNode>
	{
		const string FormatKey = "format";

		public RestRouteCreator(string @namespace, ISourceGenerator sourceGenerator, INamingService namingService, RestRouteTreeNode node, CodeTypeDeclaration routeDefinitions, CodeTypeDeclaration routes) : base(@namespace, sourceGenerator, namingService, node, routeDefinitions, routes)
		{
			OptionalRouteParameters.Add(FormatKey, new StringRouteParameterType());
		}

		protected override void AddRouteDefinitionBaseType()
		{
			routeDefinition.BaseTypes.Add(typeof(RestfulRoute));
		}

		protected override CodeExpression[] GetRouteDefinitionBaseConstructorArgs()
		{
			return base.GetRouteDefinitionBaseConstructorArgs().Union(new [] { new CodePrimitiveExpression(node.RequiredVerb) }).ToArray();
		}

		protected override CodeExpression CreateRouteDefinitionsPropertyGetter()
		{
			var expression = (CodeCastExpression) base.CreateRouteDefinitionsPropertyGetter();

			if (!string.IsNullOrEmpty(node.RestVerbResolver))
			{
				var exp = (CodeMethodInvokeExpression) expression.Expression;
				exp = (CodeMethodInvokeExpression) exp.Method.TargetObject;
				exp = (CodeMethodInvokeExpression) exp.Method.TargetObject;
				exp = (CodeMethodInvokeExpression) exp.Method.TargetObject;
				exp = (CodeMethodInvokeExpression) exp.Method.TargetObject;
				exp = (CodeMethodInvokeExpression) exp.Method.TargetObject;

				var resolverCreation = new CodeObjectCreateExpression(node.RestVerbResolver);

				var parent = (CodeObjectCreateExpression) exp.Method.TargetObject;
				var withRestVerbResolver = new CodeMethodInvokeExpression(parent, "WithRestVerbResolver", resolverCreation);
				exp.Method.TargetObject = withRestVerbResolver;
			}

			return expression;
		}

		protected override string RouteDefinitionPattern
		{
			get
			{
				return base.RouteDefinitionPattern + ".[" + FormatKey + "]";
			}
		}
	}
}