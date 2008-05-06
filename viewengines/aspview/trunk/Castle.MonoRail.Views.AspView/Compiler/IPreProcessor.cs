using System.Collections.Generic;

namespace Castle.MonoRail.Views.AspView.Compiler
{
	public interface IPreProcessor
	{
		void ApplyPreCompilationStepsOn(IEnumerable<SourceFile> files);
	}
}
