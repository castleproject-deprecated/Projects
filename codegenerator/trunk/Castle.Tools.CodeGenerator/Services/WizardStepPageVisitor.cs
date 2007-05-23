namespace Castle.Tools.CodeGenerator.Services
{
	public class WizardStepPageVisitor : TypeResolvingVisitor
	{
		#region Member Data
		private ITreeCreationService _treeService;
		private ILogger _logger;
		#endregion

		#region WizardStepVisitor()
		public WizardStepPageVisitor(ILogger logger, ITypeResolver typeResolver, ITreeCreationService treeService)
			: base(typeResolver)
		{
			_logger = logger;
			_treeService = treeService;
		}
		#endregion
	}
}
