using System.Reflection;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView.Compiler;

namespace Castle.MonoRail.Views.AspView.Tests.Compiler
{
	using Xunit;

	public class AspViewCompilerTests
	{
		[Fact]
		public void Compiler_Compiles()
		{
			// ViewSourceLoader 
			// ViewsFolder
			// Target Bin Folder
			// 

			IViewSourceLoader viewSourceLoader;
			AspViewCompilerOptions options;
			OnlineCompiler compiler = new OnlineCompiler(
				viewSourceLoader,
				options
				);

			Assembly compiledViews = compiler.Execute();


		}
	}
}
