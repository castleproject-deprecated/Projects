namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System.IO;

	public class WebCompilationContext : CompilationContext
	{
		public WebCompilationContext(DirectoryInfo siteRoot, DirectoryInfo temporarySourceFilesDirectory) : base(
			siteRoot.GetDirectories("bin")[0], 
			siteRoot, 
			siteRoot.GetDirectories("bin")[0],
			temporarySourceFilesDirectory)
		{
		}
	}
}
