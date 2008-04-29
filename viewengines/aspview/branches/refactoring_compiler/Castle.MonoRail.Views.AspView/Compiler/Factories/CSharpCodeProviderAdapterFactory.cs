namespace Castle.MonoRail.Views.AspView.Compiler.Factories
{
	using System.CodeDom.Compiler;
	using System.Configuration;
	using System.Security;
	using Microsoft.CSharp;

	using Adapters;

	public class CSharpCodeProviderAdapterFactory : ICodeProviderAdapterFactory
	{
		public ICodeProviderAdapter GetAdapter()
		{
			CodeDomProvider codeProvider;
			try
			{
				codeProvider = CodeDomProvider.GetCompilerInfo("csharp").CreateProvider();
			}
			catch (SecurityException)
			{
				codeProvider = new CSharpCodeProvider();
			}
			catch (ConfigurationException)
			{
				codeProvider = new CSharpCodeProvider();
			}

			return new DefaultCodeProviderAdapter(codeProvider);
		}
	}
}
namespace Castle.MonoRail.Views.AspView.Compiler.Factories
{
	using System.CodeDom.Compiler;
	using System.Configuration;
	using System.Security;
	using Microsoft.CSharp;

	using Adapters;

	public class CSharpCodeProviderAdapterFactory : ICodeProviderAdapterFactory
	{
		public ICodeProviderAdapter GetAdapter()
		{
			CodeDomProvider codeProvider;
			try
			{
				codeProvider = CodeDomProvider.GetCompilerInfo("csharp").CreateProvider();
			}
			catch (SecurityException)
			{
				codeProvider = new CSharpCodeProvider();
			}
			catch (ConfigurationException)
			{
				codeProvider = new CSharpCodeProvider();
			}

			return new DefaultCodeProviderAdapter(codeProvider);
		}
	}
}
