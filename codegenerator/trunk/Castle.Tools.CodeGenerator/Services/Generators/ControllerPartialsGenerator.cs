using System.CodeDom;
using Castle.Tools.CodeGenerator.Model.TreeNodes;
using Castle.Tools.CodeGenerator.Services.Generators;

namespace Castle.Tools.CodeGenerator.Services.Generators
{
	public class ControllerPartialsGenerator : AbstractGenerator
	{
		public ControllerPartialsGenerator(ILogger logger, ISourceGenerator source, INamingService naming,
		                                   string targetNamespace, string serviceType)
			: base(logger, source, naming, targetNamespace, serviceType)
		{
		}

		public override void Visit(ControllerTreeNode node)
		{
			CodeTypeDeclaration type = _source.GenerateTypeDeclaration(node.Namespace, node.Name);

			string actionWrapperName = _namespace + "." + node.PathNoSlashes + _naming.ToControllerWrapperName(node.Name);
			type.Members.Add(
				_source.CreateReadOnlyProperty("MyActions", _source[actionWrapperName],
				                               new CodeObjectCreateExpression(_source[actionWrapperName],
				                                                              new CodePropertyReferenceExpression(
				                                                              	new CodeThisReferenceExpression(),
				                                                              	"CodeGeneratorServices"))));

			string viewWrapperName = _namespace + "." + node.PathNoSlashes + _naming.ToViewWrapperName(node.Name);
			type.Members.Add(
				_source.CreateReadOnlyProperty("MyViews", _source[viewWrapperName],
				                               new CodeObjectCreateExpression(_source[viewWrapperName],
				                                                              new CodePropertyReferenceExpression(
				                                                              	new CodeThisReferenceExpression(),
				                                                              	"CodeGeneratorServices"))));

			string routeWrapperName = _namespace + "." + node.PathNoSlashes + _naming.ToRouteWrapperName(node.Name);
			type.Members.Add(
				_source.CreateReadOnlyProperty("MyRoutes", _source[routeWrapperName],
				                               new CodeObjectCreateExpression(_source[routeWrapperName],
				                                                              new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "CodeGeneratorServices"))));

			CodeMemberMethod initialize = new CodeMemberMethod();
			initialize.Attributes = MemberAttributes.Override | MemberAttributes.Family;
			initialize.Name = "PerformGeneratedInitialize";
			initialize.Statements.Add(
				new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "PerformGeneratedInitialize"));

			initialize.Statements.Add(AddPropertyToPropertyBag("MyViews"));
			initialize.Statements.Add(AddPropertyToPropertyBag("MyActions"));
			initialize.Statements.Add(AddPropertyToPropertyBag("MyRoutes"));

			type.Members.Add(initialize);

			base.Visit(node);
		}

		public override void Visit(WizardControllerTreeNode node)
		{
			Visit((ControllerTreeNode) node);
		}

		protected virtual CodeStatement AddPropertyToPropertyBag(string property)
		{
			CodeStatement assign = new CodeAssignStatement(
				new CodeIndexerExpression(
					new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "PropertyBag"),
					new CodePrimitiveExpression(property)
					),
				new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), property)
				);
			return assign;
		}
	}
}