using System.CodeDom;
using System.Web;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.Framework.Services;
using Castle.Tools.CodeGenerator.CodeDom;
using Castle.Tools.CodeGenerator.Model.TreeNodes;

namespace Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration
{
	public abstract class RouteCreator<T> where T : RouteTreeNode
	{
		protected string @namespace;
		protected INamingService namingService;
		protected ISourceGenerator sourceGenerator;
		protected T node;		
		protected CodeTypeDeclaration routeDefinitions;
		protected CodeTypeDeclaration routeDefinition;
		protected CodeTypeDeclaration routes;

		protected RouteCreator(string @namespace, ISourceGenerator sourceGenerator, INamingService namingService, T node, CodeTypeDeclaration routeDefinitions, CodeTypeDeclaration routes)
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
			CodeMemberProperty property = CreateMemberProperty
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
			CodeParameterDeclarationExpression[] parameters = GetRoutesMethodParameters();
			
			CodeMemberMethod method = CreateMemberMethod.Called(node.Name)
				.WithSummaryComment(HttpUtility.HtmlEncode(node.Pattern))
				.WithParameters(parameters)
				.WithAttributes(MemberAttributes.Public)
				.WithBody(CreateRoutesMethodBody())
				.Returning<string>().Method;

			routes.Members.Add(method);
		}

		protected virtual CodeStatement[] CreateRoutesMethodBody()
		{
			CodeTypeReferenceExpression routes = new CodeTypeReferenceExpression(this.routeDefinitions.Name);
			CodePropertyReferenceExpression route = new CodePropertyReferenceExpression(routes, node.Name);
			CodeMethodInvokeExpression createUrl = new CodeMethodInvokeExpression(route, "CreateUrl", new CodePrimitiveExpression(null));
			CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement(createUrl);
			
//			CodeFieldReferenceExpression engineContext = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), RouteMapGenerator.engineContextFieldName);
//			CodePropertyReferenceExpression services = new CodePropertyReferenceExpression(engineContext, "Services");
//			CodePropertyReferenceExpression urlBuilder = new CodePropertyReferenceExpression(services, "UrlBuilder");
//			CodeMethodInvokeExpression buildUrl = new CodeMethodInvokeExpression(urlBuilder, "BuildUrl", CreateBuildUrlParameters(engineContext));
//			CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement(buildUrl);

			return new CodeStatement[] { returnStatement };
		}

		protected virtual CodeExpression[] CreateBuildUrlParameters(CodeFieldReferenceExpression engineContext)
		{
			CodePropertyReferenceExpression urlInfo = new CodePropertyReferenceExpression(engineContext, "UrlInfo");

			CodeObjectCreateExpression urlBuilderParameters = new CodeObjectCreateExpression(
				typeof(UrlBuilderParameters),
				new CodePrimitiveExpression(node.Action.Controller.Area),
				new CodePrimitiveExpression(namingService.ToControllerName(node.Action.Controller.Name)),
				new CodePrimitiveExpression(node.Action.Name));

			return new CodeExpression[]
			{
				urlInfo,
				urlBuilderParameters
			};
		}

		protected abstract void AddRouteDefinitionBaseType();
		protected abstract CodeExpression CreateRouteDefinitionsPropertyGetter();		
		protected abstract string RouteDefinitionPattern { get; }
	}
}