
namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System.IO;

	public interface ICompilationContext
	{
		DirectoryInfo BinFolder { get; }
		DirectoryInfo SiteRoot { get; }
		DirectoryInfo ViewRootDir { get; }
		DirectoryInfo TemporarySourceFilesDirectory { get; }
	}
}
