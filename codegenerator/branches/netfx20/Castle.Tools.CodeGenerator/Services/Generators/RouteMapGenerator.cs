using System.CodeDom;
using System.Collections;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using Castle.Tools.CodeGenerator.CodeDom;
using Castle.Tools.CodeGenerator.Model.TreeNodes;
using Castle.Tools.CodeGenerator.Services.Generators;
using Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration;

namespace Castle.Tools.CodeGenerator.Services.Generators
{
	public class RouteMapGenerator : AbstractGenerator
	{
		public const string engineContextFieldName = "engineContext";

		private CodeTypeDeclaration routesType = null;
		
		public RouteMapGenerator(ILogger logger, ISourceGenerator source, INamingService naming, string targetNamespace,
		                         string serviceType) : base(logger, source, naming, targetNamespace, serviceType)
		{
		}

		public override void Visit(ControllerTreeNode node)
		{
			CodeTypeDeclaration type = GenerateTypeDeclaration(_namespace, node.PathNoSlashes + _naming.ToRouteWrapperName(node.Name));

			_typeStack.Push(type);
			base.Visit(node);
			_typeStack.Pop();
		}

		public override void Visit(WizardControllerTreeNode node)
		{
			Visit((ControllerTreeNode) node);
		}

		public override void Visit(StaticRouteTreeNode node)
		{
			StaticRouteCreator routeCreator = new StaticRouteCreator(_namespace, _source, _naming, node, RouteDefinitionsType, RoutesType);
			routeCreator.Create();

			base.Visit(node);
		}

		public override void Visit(PatternRouteTreeNode node)
		{
			PatternRouteCreator routeCreator = new PatternRouteCreator(_namespace, _source, _naming, node, RouteDefinitionsType, RoutesType);
			routeCreator.Create();

			base.Visit(node);
		}

		private static void CreateRoutesTypeConstructor(CodeTypeDeclaration type)
		{
			CodeMemberField engineContext = CreateMemberField.WithNameAndType<IEngineContext>(engineContextFieldName).WithAttributes(MemberAttributes.Family).Field;
			type.Members.Add(engineContext);			

			CodeConstructor constructor = CreateConstructor
				.WithParameters(new CodeParameterDeclarationExpression(typeof(IEngineContext), engineContext.Name))
				.WithAttributes(MemberAttributes.Public)
				.WithBody(CreateAssignStatement.This(engineContext.Name).EqualsArgument(engineContext.Name).Statement)
				.Constructor;
			type.Members.Add(constructor);
		}

		private CodeTypeDeclaration GetParentType(string name)
		{
			CodeNamespace ns = _source.LookupNamespace(_namespace);
			CodeTypeDeclaration codeTypeDeclaration;

			for (int i = 0; i < ns.Types.Count; i++)
			{
				codeTypeDeclaration = ns.Types[i];

				if (codeTypeDeclaration.Name == name)
				{
					return codeTypeDeclaration;
				}
			}

			codeTypeDeclaration = _source.GenerateTypeDeclaration(_namespace, name);

			return codeTypeDeclaration;
		}

		private CodeTypeDeclaration RoutesType
		{
			get
			{
				if (routesType == null)
				{
					routesType = GetParentType("Routes");
					CreateRoutesTypeConstructor(routesType);
				}

				return routesType;
			}
		}

		private CodeTypeDeclaration RouteDefinitionsType
		{
			get
			{
				return GetParentType("RouteDefinitions");
			}
		}
	}
}