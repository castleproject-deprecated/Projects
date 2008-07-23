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
	using System.CodeDom;
	using System.Web;
	using MonoRail.Framework.Services;
	using CodeDom;
	using Model.TreeNodes;

	public abstract class RouteCreator<T> where T : RouteTreeNode
	{
		protected string @namespace;
		protected INamingService namingService;
		protected ISourceGenerator sourceGenerator;
		protected T node;
		protected CodeTypeDeclaration routeDefinitions;
		protected CodeTypeDeclaration routeDefinition;
		protected CodeTypeDeclaration routes;

		protected RouteCreator(string @namespace, ISourceGenerator sourceGenerator, INamingService namingService, T node,
		                       CodeTypeDeclaration routeDefinitions, CodeTypeDeclaration routes)
		{
			this.@namespace = @namespace;
			this.sourceGenerator = sourceGenerator;
			this.namingService = namingService;
			this.node = node;
			this.routeDefinitions = routeDefinitions;
			this.routes = routes;
		}

		public void Create()
		{
			CreateRouteDefinition();
			CreateRouteDefinitionsProperty();
			CreateRoutesMethod();
		}

		protected virtual void CreateRouteDefinition()
		{
			routeDefinition = sourceGenerator.GenerateTypeDeclaration(@namespace, node.Name + "Route", routeDefinitions.Name);
			AddRouteDefinitionBaseType();

			routeDefinition.Members.Add(
				CreateConstructor.WithParameters()
					.WithAttributes(MemberAttributes.Public)
					.WithBaseConstructorArgs(GetRouteDefinitionBaseConstructorArgs())
					.Constructor);
		}

		protected void CreateRouteDefinitionsProperty()
		{
			var property = CreateMemberProperty
				.OfType(routeDefinition.Name)
				.Called(node.Name)
				.WithSummaryComment(HttpUtility.HtmlEncode(node.Pattern))
				.WithAttributes(MemberAttributes.Public | MemberAttributes.Static)
				.WithGetter(new CodeMethodReturnStatement(CreateRouteDefinitionsPropertyGetter()))
				.Property;

			routeDefinitions.Members.Add(property);
		}

		protected virtual CodeExpression[] GetRouteDefinitionBaseConstructorArgs()
		{
			return new CodeExpression[]
			{
				new CodePrimitiveExpression(node.Name),
				new CodePrimitiveExpression(RouteDefinitionPattern)
			};
		}

		protected virtual CodeParameterDeclarationExpression[] GetRoutesMethodParameters()
		{
			return new CodeParameterDeclarationExpression[0];
		}

		private void CreateRoutesMethod()
		{
			var parameters = GetRoutesMethodParameters();

			var method = CreateMemberMethod.Called(node.Name)
				.WithSummaryComment(HttpUtility.HtmlEncode(node.Pattern))
				.WithParameters(parameters)
				.WithAttributes(MemberAttributes.Public)
				.WithBody(CreateRoutesMethodBody())
				.Returning<string>().Method;

			routes.Members.Add(method);
		}

		protected virtual CodeStatement[] CreateRoutesMethodBody()
		{
			var routes = new CodeTypeReferenceExpression(routeDefinitions.Name);
			var route = new CodePropertyReferenceExpression(routes, node.Name);
			var createUrl = new CodeMethodInvokeExpression(route, "CreateUrl", new CodePrimitiveExpression(null));
			var returnStatement = new CodeMethodReturnStatement(createUrl);

			return new CodeStatement[] {returnStatement};
		}

		protected virtual CodeExpression[] CreateBuildUrlParameters(CodeFieldReferenceExpression engineContext)
		{
			var urlInfo = new CodePropertyReferenceExpression(engineContext, "UrlInfo");

			var urlBuilderParameters = new CodeObjectCreateExpression(
				typeof (UrlBuilderParameters),
				new CodePrimitiveExpression(node.Action.Controller.Area),
				new CodePrimitiveExpression(namingService.ToControllerName(node.Action.Controller.Name)),
				new CodePrimitiveExpression(node.Action.Name));

			return new CodeExpression[] { urlInfo, urlBuilderParameters };
		}

		protected abstract void AddRouteDefinitionBaseType();
		protected abstract CodeExpression CreateRouteDefinitionsPropertyGetter();
		protected abstract string RouteDefinitionPattern { get; }
	}
}