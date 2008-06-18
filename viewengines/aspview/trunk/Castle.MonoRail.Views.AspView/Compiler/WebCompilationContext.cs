namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System.IO;
	
	using Castle.MonoRail.Framework.Configuration;

	public class WebCompilationContext : CompilationContext
	{
		public WebCompilationContext(IMonoRailConfiguration monoRailConfiguration, DirectoryInfo siteRoot, DirectoryInfo temporarySourceFilesDirectory) : base(
			siteRoot.GetDirectories("bin")[0], 
			siteRoot,
			new DirectoryInfo(Path.Combine(siteRoot.FullName, monoRailConfiguration.ViewEngineConfig.ViewPathRoot)),
			temporarySourceFilesDirectory)
		{
		}
	}
}
