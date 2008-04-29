namespace Castle.MonoRail.Views.AspView.Compiler.Adapters
{
	using System;
	using System.CodeDom.Compiler;

	public class DefaultCodeProviderAdapter : ICodeProviderAdapter, IDisposable
	{
		readonly CodeDomProvider codeProvider;

		public DefaultCodeProviderAdapter(CodeDomProvider codeProvider)
		{
			this.codeProvider = codeProvider;
		}

		public CompilerResults CompileAssemblyFromSource(CompilerParameters parameters, params string[] sources)
		{
			return codeProvider.CompileAssemblyFromSource(parameters, sources);

		}

		public CompilerResults CompileAssemblyFromFile(CompilerParameters parameters, params string[] fileNames)
		{
			return codeProvider.CompileAssemblyFromFile(parameters, fileNames);
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		public void Dispose()
		{
			if (codeProvider != null)
			{
				codeProvider.Dispose();
			}
		}
	}
}
namespace Castle.MonoRail.Views.AspView.Compiler.Adapters
{
	using System;
	using System.CodeDom.Compiler;

	public class DefaultCodeProviderAdapter : ICodeProviderAdapter, IDisposable
	{
		readonly CodeDomProvider codeProvider;

		public DefaultCodeProviderAdapter(CodeDomProvider codeProvider)
		{
			this.codeProvider = codeProvider;
		}

		public CompilerResults CompileAssemblyFromSource(CompilerParameters parameters, params string[] sources)
		{
			return codeProvider.CompileAssemblyFromSource(parameters, sources);

		}

		public CompilerResults CompileAssemblyFromFile(CompilerParameters parameters, params string[] fileNames)
		{
			return codeProvider.CompileAssemblyFromFile(parameters, fileNames);
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		public void Dispose()
		{
			if (codeProvider != null)
			{
				codeProvider.Dispose();
			}
		}
	}
}
