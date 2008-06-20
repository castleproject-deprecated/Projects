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
	using Microsoft.CSharp;
	using DirectoryStructure;
	using GeneratedClassMetadata;

	public class DirectoryStructureToClassDescriptorsService : IDirectoryStructureToClassDescriptorsService
	{
		public ClassDescriptor[] Execute(IDirInfo site)
		{
			List<ClassDescriptor> classDescriptors = new List<ClassDescriptor>();
			PutAllClassDescriptorFromIn(site, classDescriptors);
			return classDescriptors.ToArray();
		}

		static void PutAllClassDescriptorFromIn(IDirInfo dir, List<ClassDescriptor> classDescriptors)
		{
			classDescriptors.Add(GetClassDescriptorFrom(dir));

			foreach (IDirInfo subDir in dir.Directories)
			{
				PutAllClassDescriptorFromIn(subDir, classDescriptors);
			}
		}

		static ClassDescriptor GetClassDescriptorFrom(IDirInfo dir)
		{
			ClassDescriptor classDescriptor = new ClassDescriptor();

			classDescriptor.Name = UrlToClassName(dir.Url);

			foreach (string file in dir.Files)
			{
				string fileName = Normalize(file);
				string url = dir.Url + "/" + file.ToLowerInvariant();
				classDescriptor.Members.Add(new MemberDescriptor(
					"string", fileName, "\"" + url + "\""));
			}

			foreach (IDirInfo subDir in dir.Directories)
			{
				string name = subDir.Name;
				if (dir.Files.Exists(delegate (string fileName) {
					return fileName == name;
				}))
				{
					name = subDir.Name + "Directory";
				}

				classDescriptor.Members.Add(new MemberDescriptor(UrlToClassName(subDir.Url), Normalize(name)));
			}
			return classDescriptor;
		}

		static readonly CSharpCodeProvider csharp = new CSharpCodeProvider();

		static string Normalize(string url)
		{
			string candidate = url
				.Replace('/', '_')
				.Replace('-', '_')
				.Replace('.', '_');

			while (!csharp.IsValidIdentifier(candidate))
				candidate = "_" + candidate;

			return candidate;
		}

		static string UrlToClassName(string url)
		{
			if (url == "")
				return "Root";

			return "Root" + Normalize(url);			
		}

	}
}
