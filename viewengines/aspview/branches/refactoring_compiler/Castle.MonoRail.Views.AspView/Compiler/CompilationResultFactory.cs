using System.CodeDom.Compiler;

namespace Castle.MonoRail.Views.AspView.Compiler
{
	public class CompilationResultFactory
	{
		readonly CompilerResults compilerResults;
		readonly bool inMemory;

		public CompilationResultFactory(CompilerResults compilerResults, bool inMemory)
		{
			this.compilerResults = compilerResults;
			this.inMemory = inMemory;
		}

		public ICompilationResult GetResult()
		{
			if (inMemory)
				return new InMemoryCompilationResult(compilerResults.CompiledAssembly);
			return new PreCompilationResult(compilerResults.PathToAssembly);
		}
	}
}
