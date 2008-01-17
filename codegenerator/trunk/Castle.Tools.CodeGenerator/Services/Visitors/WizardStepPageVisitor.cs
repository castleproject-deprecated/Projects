using Castle.Tools.CodeGenerator.Services.Visitors;

namespace Castle.Tools.CodeGenerator.Services.Visitors
{
	public class WizardStepPageVisitor : TypeResolvingVisitor
	{
		private ITreeCreationService _treeService;
		private ILogger _logger;
		
		public WizardStepPageVisitor(ILogger logger, ITypeResolver typeResolver, ITreeCreationService treeService)
			: base(typeResolver)
		{
			_logger = logger;
			_treeService = treeService;
		}
	}
}