
namespace Castle.MonoRail.Views.AspView.Compiler.Adapters
{
	using System.IO;

	public interface IFileSystemAdapter
	{
		void Create(DirectoryInfo directory);
		bool Exists(DirectoryInfo directory);
		void ClearSourceFilesFrom(DirectoryInfo directory);
		void Save(string fileName, string content,  DirectoryInfo directory);
		string[] GetSourceFilesFrom(DirectoryInfo directory);
	}
}

namespace Castle.MonoRail.Views.AspView.Compiler.Adapters
{
	using System.IO;

	public interface IFileSystemAdapter
	{
		void Create(DirectoryInfo directory);
		bool Exists(DirectoryInfo directory);
		void ClearSourceFilesFrom(DirectoryInfo directory);
		void Save(string fileName, string content,  DirectoryInfo directory);
		string[] GetSourceFilesFrom(DirectoryInfo directory);
	}
}
