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
	using System;
	using System.IO;
	using DirectoryStructure;

	public class BuildDirectoryStructureService : IBuildDirectoryStructureService
	{
		readonly IFileSystemAdapter fileSystem;
		string[] ignore = new string[0];

		public BuildDirectoryStructureService(IFileSystemAdapter fileSystem)
		{
			this.fileSystem = fileSystem;
		}

		public IDirInfo Execute(string rootPath)
		{
			return Process(rootPath, rootPath);
		}

		public IDirInfo Execute(string rootPath, string[] ignoreDirectories)
		{
			ignore = ignoreDirectories;
			return Execute(rootPath);
		}

		private IDirInfo Process(string rootPath, string dirPath)
		{
			string url = NormalizeUrl(rootPath, dirPath);
			IDirInfo dir = new ResourceDirInfo(url);

			foreach (string fileName in fileSystem.GetScriptFiles(dirPath))
			{
				dir.Files.Add(fileName);
			}
			foreach (string fileName in fileSystem.GetStyleFiles(dirPath))
			{
				dir.Files.Add(fileName);
			}
			foreach (string fileName in fileSystem.GetImageFiles(dirPath))
			{
				dir.Files.Add(fileName);
			}

			foreach (string subdirPath in fileSystem.GetDirectories(dirPath))
			{
				string directoryname = fileSystem.GetDirectoryName(subdirPath);
				if (Array.Exists(ignore, delegate(string ignored) { return ignored == directoryname; }))
					continue;
				IDirInfo subdir = Process(rootPath, subdirPath);
				dir.AddSubDirectory(subdir);
				if (subdir.Files.Count > 0)
					subdir.HasFiles = true;
			}
			return dir;
		}

		static string NormalizeUrl(string rootPath, string dirPath)
		{
			string url = dirPath
				.Replace(rootPath, "")
				.Replace(Path.DirectorySeparatorChar, '/')
				.Trim('/');

			if (url == string.Empty)
				return url;

			return "/" + url;
		}
	}
}