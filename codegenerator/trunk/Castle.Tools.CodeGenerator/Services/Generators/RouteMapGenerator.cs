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
	using MonoRail.Framework;
	using CodeDom;
	using Model.TreeNodes;
	using Generators;
	using RouteMapGeneration;

	public class RouteMapGenerator : AbstractGenerator
	{
		public const string engineContextFieldName = "engineContext";

		private CodeTypeDeclaration routesType;

		public RouteMapGenerator(ILogger logger, ISourceGenerator source, INamingService naming, string targetNamespace,
		                         string serviceType) : base(logger, source, naming, targetNamespace, serviceType)
		{
		}

		public override void Visit(ControllerTreeNode node)
		{
			var type = GenerateTypeDeclaration(@namespace, node.PathNoSlashes + naming.ToRouteWrapperName(node.Name));

			typeStack.Push(type);
			base.Visit(node);
			typeStack.Pop();
		}

		public override void Visit(WizardControllerTreeNode node)
		{
			Visit((ControllerTreeNode) node);
		}

		public override void Visit(StaticRouteTreeNode node)
		{
			var routeCreator = new StaticRouteCreator(@namespace, source, naming, node, RouteDefinitionsType, RoutesType);
			routeCreator.Create();

			base.Visit(node);
		}

		public override void Visit(PatternRouteTreeNode node)
		{
			if (!(node is RestRouteTreeNode))
			{
				var routeCreator = new PatternRouteCreator<PatternRouteTreeNode>(@namespace, source, naming, node, RouteDefinitionsType, RoutesType);
				routeCreator.Create();	
			}

			base.Visit(node);
		}

		public override void Visit(RestRouteTreeNode node)
		{
			var routeCreator = new RestRouteCreator(@namespace, source, naming, node, RouteDefinitionsType, RoutesType);
			routeCreator.Create();

			base.Visit(node);
		}

		private static void CreateRoutesTypeConstructor(CodeTypeDeclaration type)
		{
			var engineContext = CreateMemberField
				.WithNameAndType<IEngineContext>(engineContextFieldName)
				.WithAttributes(MemberAttributes.Family).Field;

			type.Members.Add(engineContext);

			var constructor = CreateConstructor
				.WithParameters(new CodeParameterDeclarationExpression(typeof (IEngineContext), engineContext.Name))
				.WithAttributes(MemberAttributes.Public)
				.WithBody(CreateAssignStatement.This(engineContext.Name).EqualsArgument(engineContext.Name).Statement)
				.Constructor;
			
			type.Members.Add(constructor);
		}

		private CodeTypeDeclaration GetParentType(string name)
		{
			var ns = source.LookupNamespace(@namespace);
			CodeTypeDeclaration codeTypeDeclaration;

			for (var i = 0; i < ns.Types.Count; i++)
			{
				codeTypeDeclaration = ns.Types[i];

				if (codeTypeDeclaration.Name == name)
					return codeTypeDeclaration;
			}

			codeTypeDeclaration = source.GenerateTypeDeclaration(@namespace, name);

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
			get { return GetParentType("RouteDefinitions"); }
		}
	}
}