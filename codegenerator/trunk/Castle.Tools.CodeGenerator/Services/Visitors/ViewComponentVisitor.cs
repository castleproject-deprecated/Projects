using Castle.Tools.CodeGenerator.Model.TreeNodes;
using Castle.Tools.CodeGenerator.Services.Visitors;
using ICSharpCode.NRefactory.Ast;

namespace Castle.Tools.CodeGenerator.Services.Visitors
{
	public class ViewComponentVisitor : TypeResolvingVisitor
	{
		private ITreeCreationService _treeService;
		private ILogger _logger;
		
		public ViewComponentVisitor(ILogger logger, ITypeResolver typeResolver, ITreeCreationService treeService)
			: base(typeResolver)
		{
			_logger = logger;
			_treeService = treeService;
		}
		
		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			_treeService.PushArea("Components");
			object r = base.VisitCompilationUnit(compilationUnit, data);
			_treeService.PopNode();
			return base.VisitCompilationUnit(compilationUnit, data);
		}

		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (!IsViewComponent(typeDeclaration))
			{
				return null;
			}

			string typeNamespace = GetNamespace(typeDeclaration);

			ViewComponentTreeNode node = new ViewComponentTreeNode(typeDeclaration.Name, typeNamespace);
			_treeService.PushNode(node);

			object r = base.VisitTypeDeclaration(typeDeclaration, data);

			_treeService.PopNode();

			return r;
		}
		
		protected virtual bool IsViewComponent(TypeDeclaration typeDeclaration)
		{
			return
				typeDeclaration.Name.EndsWith("Component");// && (typeDeclaration.Modifier & Modifier.Partial) == Modifier.Partial;
		}
	}
}