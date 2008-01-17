using System;
using System.CodeDom;
using Castle.Tools.CodeGenerator.Model;
using Castle.Tools.CodeGenerator.Model.TreeNodes;

namespace Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration
{
	public class StaticRouteCreator : RouteCreator<StaticRouteTreeNode>
	{
		public StaticRouteCreator(string @namespace, ISourceGenerator sourceGenerator, INamingService namingService, StaticRouteTreeNode node, CodeTypeDeclaration routeDefinitions, CodeTypeDeclaration routes) : base(@namespace, sourceGenerator, namingService, node, routeDefinitions, routes)
		{
		}

		protected override void AddRouteDefinitionBaseType()
		{
			routeDefinition.BaseTypes.Add(typeof(StaticRoute));
		}

		protected override CodeExpression CreateRouteDefinitionsPropertyGetter()
		{
			return new CodeObjectCreateExpression(routeDefinition.Name);
		}

		protected override CodeExpression[] GetRouteDefinitionBaseConstructorArgs()
		{
			CodeExpression[] expressions = base.GetRouteDefinitionBaseConstructorArgs();
			Array.Resize(ref expressions, expressions.Length + 3);

			expressions[expressions.Length - 3] = new CodePrimitiveExpression(node.Action.Controller.Area);
			expressions[expressions.Length - 2] = new CodePrimitiveExpression(namingService.ToControllerName(node.Action.Controller.Name));
			expressions[expressions.Length - 1] = new CodePrimitiveExpression(node.Action.Name);

			return expressions;
		}

		protected override string RouteDefinitionPattern
		{
			get { return node.Pattern; }
		}
	}
}