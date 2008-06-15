namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System.Collections.Generic;
	using PreCompilationSteps;

	public class PreProcessor : IPreProcessor
	{
		readonly IEnumerable<IPreCompilationStep> preCompilationSteps;
		public PreProcessor()
		{
			preCompilationSteps = Resolve.PreCompilationStepsProvider.GetSteps();
		}

		public void ApplyPreCompilationStepsOn(IEnumerable<SourceFile> files)
		{
			foreach (SourceFile sourceFile in files)
			{
				ApplyPreCompilationStepsOn(sourceFile);
			}
		} 

		void ApplyPreCompilationStepsOn(SourceFile file)
		{
			foreach (IPreCompilationStep step in preCompilationSteps)
			{
				step.Process(file);
			}
		}
	}
}
