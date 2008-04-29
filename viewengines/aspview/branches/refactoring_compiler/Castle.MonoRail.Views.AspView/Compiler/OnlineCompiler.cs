namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.Reflection;

	using Factories;

	public class OnlineCompiler : AbstractCompiler
	{
		public OnlineCompiler(ICodeProviderAdapterFactory codeProviderAdapterFactory, IPreProcessor preProcessor, ICompilationContext context, AspViewCompilerOptions options) : base(codeProviderAdapterFactory, preProcessor, context, options)
		{
			parameters.GenerateInMemory = true;
		}

		public Assembly Execute()
		{
			return InternalExecute().CompiledAssembly;
		}

		protected override CompilerResults GetResultsFrom(List<SourceFile> files)
		{
			string[] sources = files.ConvertAll<string>(SourceFileToSource).ToArray();

			return codeProvider.CompileAssemblyFromSource(parameters, sources);
		}

		static string SourceFileToSource(SourceFile file)
		{
			return file.ConcreteClass;
		}


	}
}
