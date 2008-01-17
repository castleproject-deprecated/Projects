#region license
// Copyright 2008 Ken Egozi http://www.kenegozi.com/blog
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace Castle.Tools.StaticMapGenerator.Services
{
	using System.Collections.Generic;
	using System.IO;

	public class FileSystemAdapter : IFileSystemAdapter
	{
		#region IFileSystemAdapter
		public virtual string[] GetScriptFiles(string dir)
		{
			return GetFiles(dir, "*.js");
		}

		public virtual string[] GetStyleFiles(string dir)
		{
			return GetFiles(dir, "*.css");
		}

		public virtual string[] GetImageFiles(string dir)
		{
			List<string> imageFiles = new List<string>();
			imageFiles.AddRange(GetFiles(dir, "*.jpg"));
			imageFiles.AddRange(GetFiles(dir, "*.gif"));
			imageFiles.AddRange(GetFiles(dir, "*.png"));
			return imageFiles.ToArray();
		}

		public virtual string[] GetDirectories(string dir)
		{
			return Directory.GetDirectories(dir);
		}

		public string GetDirectoryName(string dirPath)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
			return directoryInfo.Name;
		}
		#endregion

		#region helpers
		private static string[] GetFileNamesFrom(IEnumerable<string> filePaths)
		{
			List<string> files = new List<string>();
			foreach (string filePath in filePaths)
			{
				files.Add(Path.GetFileName(filePath));
			}
			return files.ToArray();
		}


		private static string[] GetFiles(string dir, string pattern)
		{
			return GetFileNamesFrom(Directory.GetFiles(dir, pattern, SearchOption.TopDirectoryOnly));
		}
		#endregion


	}
}
