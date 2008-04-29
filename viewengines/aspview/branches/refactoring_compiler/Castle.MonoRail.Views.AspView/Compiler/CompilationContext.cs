namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System.IO;

	public class CompilationContext : ICompilationContext
	{
		readonly DirectoryInfo binFolder;
		readonly DirectoryInfo siteRoot;
		readonly DirectoryInfo temporarySourceFilesDirectory;
		readonly DirectoryInfo viewRootDir;

		public CompilationContext(DirectoryInfo binFolder, DirectoryInfo siteRoot, DirectoryInfo viewRootDir, DirectoryInfo temporarySourceFilesDirectory)
		{
			this.binFolder = binFolder;
			this.siteRoot = siteRoot;
			this.temporarySourceFilesDirectory = temporarySourceFilesDirectory;
			this.viewRootDir = viewRootDir;
		}

		public DirectoryInfo BinFolder
		{
			get { return binFolder; }
		}

		public DirectoryInfo ViewRootDir
		{
			get { return viewRootDir; }
		}

		public DirectoryInfo SiteRoot
		{
			get { return siteRoot; }
		}
		
		public DirectoryInfo TemporarySourceFilesDirectory
		{
			get { return temporarySourceFilesDirectory; }
		}

	}
}
