namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System.Collections.Generic;
	using PreCompilationSteps;

	public class PreProcessor : IPreProcessor
	{
		delegate void PreCompilationStepDelegate(SourceFile file);

		readonly IEnumerable<IPreCompilationStep> preCompilationSteps;
		public PreProcessor()
		{
			preCompilationSteps = Resolve.PreCompilationStepsProvider.GetSteps();
		}

		public void ApplyPreCompilationStepsOn(IEnumerable<SourceFile> files)
		{
			foreach (SourceFile sourceFile in files)
//				((PreCompilationStepDelegate) delegate(SourceFile file) {
				ApplyPreCompilationStepsOn(sourceFile);
//				})
//					.BeginInvoke(sourceFile, null, null);
		} 

		void ApplyPreCompilationStepsOn(SourceFile file)
		{
			foreach (IPreCompilationStep step in preCompilationSteps)
				step.Process(file);
		}
	}
}
