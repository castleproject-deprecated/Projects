namespace Castle.MonoRail.Views.AspView.Compiler.Adapters
{
	using System.CodeDom.Compiler;

	public interface ICodeProviderAdapter
	{
		CompilerResults CompileAssemblyFromSource(CompilerParameters parameters, params string[] sources);
		CompilerResults CompileAssemblyFromFile(CompilerParameters parameters, params string[] fileNames);
	}
}
namespace Castle.MonoRail.Views.AspView.Compiler.Adapters
{
	using System.CodeDom.Compiler;

	public interface ICodeProviderAdapter
	{
		CompilerResults CompileAssemblyFromSource(CompilerParameters parameters, params string[] sources);
		CompilerResults CompileAssemblyFromFile(CompilerParameters parameters, params string[] fileNames);
	}
}
