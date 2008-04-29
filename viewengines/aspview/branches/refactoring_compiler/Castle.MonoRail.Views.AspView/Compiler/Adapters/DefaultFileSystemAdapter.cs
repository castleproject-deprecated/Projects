using System.IO;
using System.Text;

namespace Castle.MonoRail.Views.AspView.Compiler.Adapters
{
	public class DefaultFileSystemAdapter : IFileSystemAdapter
	{
		public void Create(DirectoryInfo directory)
		{
			directory.Create();
		}

		public bool Exists(DirectoryInfo directory)
		{
			return directory.Exists;
		}

		public void ClearSourceFilesFrom(DirectoryInfo directory)
		{
			foreach (FileInfo file in directory.GetFiles("*.cs"))
			{
				file.Delete();
			}
		}

		public void Save(string fileName, string content, DirectoryInfo directory)
		{
			File.WriteAllText(Path.Combine(directory.FullName, fileName), content, Encoding.UTF8);
		}

		public string[] GetSourceFilesFrom(DirectoryInfo directory)
		{
			return Directory.GetFiles(directory.FullName, "*.cs", SearchOption.TopDirectoryOnly);
		}
	}
}
using System.IO;
using System.Text;

namespace Castle.MonoRail.Views.AspView.Compiler.Adapters
{
	public class DefaultFileSystemAdapter : IFileSystemAdapter
	{
		public void Create(DirectoryInfo directory)
		{
			directory.Create();
		}

		public bool Exists(DirectoryInfo directory)
		{
			return directory.Exists;
		}

		public void ClearSourceFilesFrom(DirectoryInfo directory)
		{
			foreach (FileInfo file in directory.GetFiles("*.cs"))
			{
				file.Delete();
			}
		}

		public void Save(string fileName, string content, DirectoryInfo directory)
		{
			File.WriteAllText(Path.Combine(directory.FullName, fileName), content, Encoding.UTF8);
		}

		public string[] GetSourceFilesFrom(DirectoryInfo directory)
		{
			return Directory.GetFiles(directory.FullName, "*.cs", SearchOption.TopDirectoryOnly);
		}
	}
}
