using System.CodeDom;
using System.Collections.Generic;
using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
	public class RouteMapGenerator : AbstractGenerator
	{
		private static CodeTypeDeclaration routingInitializerType = null;
		private static CodeMemberMethod routingInitialize = null;

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

		public override void Visit(RouteTreeNode node)
		{
			CodeTypeDeclaration type = _typeStack.Peek();
			List<string> actionArgumentTypes = new List<string>();

			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = node.Name;
			method.ReturnType = _source[typeof (IControllerActionReference)];
			method.Attributes = MemberAttributes.Public;
			method.CustomAttributes.Add(_source.DebuggerAttribute);
			List<CodeExpression> actionArguments = CreateRouteArgumentsAndAddParameters(method, node, actionArgumentTypes);
			method.Statements.Add(new CodeMethodReturnStatement(CreateNewRouteReference(node, actionArguments, actionArgumentTypes)));
			type.Members.Add(method);

			if (routingInitializerType == null)
				routingInitializerType = GetRoutingInitializerType();

			if (routingInitialize == null)
				routingInitialize = GetRoutingInitializeMethod();

			ActionTreeNode actionTreeNode = (ActionTreeNode) node.Parent;

			CodeExpressionStatement statement = CreateRuleInitializationMethod(node, actionTreeNode);
			InsertedInitializeMethod(node, statement);

			base.Visit(node);
		}

		private CodeExpressionStatement CreateRuleInitializationMethod(RouteTreeNode node, ActionTreeNode actionTreeNode)
		{
			return new CodeExpressionStatement(
				new CodeMethodInvokeExpression(
					new CodePropertyReferenceExpression(
						new CodeTypeReferenceExpression("Castle.MonoRail.Framework.Routing.RoutingModuleEx"), 
						"Engine"
						), 
					"Add", 
					new CodeMethodInvokeExpression(
						new CodeTypeReferenceExpression("Castle.MonoRail.Framework.Routing.PatternRule"),
						"Build",
						new CodePrimitiveExpression(node.Name),
						new CodePrimitiveExpression(node.Pattern),
						new CodeTypeOfExpression(actionTreeNode.Controller.FullName),
						new CodePrimitiveExpression(actionTreeNode.Name)
						)
					)
				);
		}

		private void InsertedInitializeMethod(RouteTreeNode node, CodeExpressionStatement statement)
		{
			CodeCommentStatement comment = new CodeCommentStatement(node.Order.ToString());
			bool inserted = false;

			foreach (CodeStatement codeStatement in routingInitialize.Statements)
			{
				if (codeStatement is CodeCommentStatement)
				{
					CodeCommentStatement commentStatement = (CodeCommentStatement) codeStatement;
					int order = int.Parse(commentStatement.Comment.Text);

					if (order > node.Order)
					{
						routingInitialize.Statements.Insert(routingInitialize.Statements.IndexOf(codeStatement), comment);
						routingInitialize.Statements.Insert(routingInitialize.Statements.IndexOf(codeStatement), statement);
						inserted = true;
						break;
					}
				}
			}

			if (!inserted)
			{
				routingInitialize.Statements.Add(comment);
				routingInitialize.Statements.Add(statement);							
			}
		}

		protected CodeMemberMethod GetRoutingInitializeMethod()
		{
			if (routingInitialize == null)
			{
				CodeMemberMethod method = new CodeMemberMethod();
				method.Name = "Initialize";
				method.Attributes = MemberAttributes.Public | MemberAttributes.Static;
				method.CustomAttributes.Add(_source.DebuggerAttribute);
				routingInitializerType.Members.Add(method);
				
				return method;
			}
			else
				return routingInitialize;
		}

		private CodeTypeDeclaration GetRoutingInitializerType()
		{
			CodeNamespace ns = _source.LookupNamespace(_namespace);
			CodeTypeDeclaration codeTypeDeclaration;

			for (int i = 0; i < ns.Types.Count; i++)
			{				
				codeTypeDeclaration = ns.Types[i];

				if (codeTypeDeclaration.Name == "RoutingInitializer")
				{
					return codeTypeDeclaration;
				}
			}

			codeTypeDeclaration = _source.GenerateTypeDeclaration(_namespace, "RoutingInitializer");

			return codeTypeDeclaration;
		}

		protected CodeExpression CreateNewRouteReference(RouteTreeNode node, List<CodeExpression> routeArguments, List<string> routeArgumentTypes)
		{
			List<CodeExpression> routeArgumentRuntimeTypes = new List<CodeExpression>();
			
			foreach (string typeName in routeArgumentTypes)
				routeArgumentRuntimeTypes.Add(new CodeTypeOfExpression(_source[typeName]));
			
			CodeExpression[] constructionArguments = new CodeExpression[]
				{
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), _naming.ToMemberVariableName(_serviceIdentifier)),
					new CodePrimitiveExpression(node.Name),
					new CodeArrayCreateExpression(_source[typeof (ActionArgument)], routeArguments.ToArray())
				};

			return new CodeMethodInvokeExpression(
				new CodeMethodReferenceExpression(
					new CodePropertyReferenceExpression(
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
						                                 _naming.ToMemberVariableName(_serviceIdentifier)),
						"ControllerReferenceFactory"),
					"CreateRouteReference"),
				constructionArguments
				);
		}

		protected List<CodeExpression> CreateRouteArgumentsAndAddParameters(CodeMemberMethod method, RouteTreeNode node, List<string> routeArgumentTypes)
		{
			List<CodeExpression> routeArguments = new List<CodeExpression>();

			int index = 0;
			foreach (ParameterTreeNode parameterInfo in node.Children)
			{
				CodeParameterDeclarationExpression newParameter = new CodeParameterDeclarationExpression();
				newParameter.Name = parameterInfo.Name;
				newParameter.Type = _source[parameterInfo.Type];
				method.Parameters.Add(newParameter);

				routeArgumentTypes.Add(parameterInfo.Type);

				CodeObjectCreateExpression argumentCreate =
					new CodeObjectCreateExpression(_source[typeof (ActionArgument)], new CodeExpression[]
                 	{
                 		new CodePrimitiveExpression(index++),
                 		new CodePrimitiveExpression(parameterInfo.Name),
						new CodeTypeOfExpression(newParameter.Type),
                 		new CodeArgumentReferenceExpression(
                 			parameterInfo.Name)
                 	});

				routeArguments.Add(argumentCreate);
			}
			return routeArguments;
		}
	}
}
