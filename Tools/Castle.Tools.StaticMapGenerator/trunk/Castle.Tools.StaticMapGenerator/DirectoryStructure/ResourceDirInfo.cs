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

namespace Castle.Tools.StaticMapGenerator.DirectoryStructure
{
	using System.Collections.Generic;

	public class ResourceDirInfo : IDirInfo
	{
		readonly string url;
		string name;
		IDirInfo parent;
		readonly List<IDirInfo> directories = new List<IDirInfo>();
		readonly List<string> files = new List<string>();
		bool hasFiles;

		public ResourceDirInfo(string url)
		{
			this.url = url;
		}

		public void RemoveEmptyDirectories()
		{
			directories.RemoveAll(EmptyDirectories);

			foreach (IDirInfo subDir in directories)
			{
				subDir.RemoveEmptyDirectories();
			}
		}

		private static bool EmptyDirectories(IDirInfo dir)
		{
			return !dir.HasFiles;
		}

		public string Url { get { return url; } }

		public string Name
		{
			get
			{
				if (name == null)
				{
					string[] parts = url.Split('/');
					name = parts[parts.Length - 1];
				}
				return name;
			}
		}

		public IDirInfo[] Directories { get { return directories.ToArray(); } }

		public void AddSubDirectory(IDirInfo subDirectory)
		{
			directories.Add(subDirectory);
			subDirectory.Parent = this;
		}

		public List<string> Files { get { return files; } }

		public bool HasFiles
		{
			get { return hasFiles; }
			set
			{
				hasFiles = value;
				if (hasFiles && parent != null && !parent.HasFiles)
					parent.HasFiles = true;
			}
		}

		public IDirInfo Parent
		{
			get { return parent; }
			set { parent = value; }
		}

	}

}