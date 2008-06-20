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

namespace Castle.Tools.StaticMapGenerator
{
	using System;
	using System.IO;
	using DirectoryStructure;
	using GeneratedClassMetadata;
	using Services;

	class Program
	{
		static void RegisterComponents()
		{
			IoC.Register<IFileSystemAdapter, FileSystemAdapter>();
			IoC.Register<IBuildDirectoryStructureService, BuildDirectoryStructureService>();
			IoC.Register<IDirectoryStructureToClassDescriptorsService, DirectoryStructureToClassDescriptorsService>();
			IoC.Register<IDescriptorToClassService, DescriptorToClassService>();
			IoC.Register<IDescriptorsToClassesService, DescriptorsToClassesService>();
		}

		static void Main(string[] args)
		{
			RegisterComponents();

			string[] ignore = new string[] { ".svn", "obj", "bin" };
			string dirPath = null;
			string generatedNamespace = "StaticSiteMap";

			foreach (string arg in args)
			{
				string[] parts = arg.Split(':');
				switch (parts[0])
				{
					case "/site":
						dirPath = string.Join(":", parts, 1, parts.Length - 1)
							.Trim('\"');
						break;

					case "/ignore":
						ignore = parts[1].Split('|');
						break;

					case "/namespace":
						generatedNamespace = parts[1];
						break;
				}
			
			}

			if (dirPath == null)
			{
				DirectoryInfo currentDir = new DirectoryInfo(Environment.CurrentDirectory);
				if (currentDir.Name.Equals("bin", StringComparison.InvariantCultureIgnoreCase))
					currentDir = currentDir.Parent;

				dirPath = currentDir.FullName;
			}

			string generatedFileName = "Static.Site.Generated.cs";
			string generatedFilePath = Path.Combine(dirPath, generatedFileName);

			
			IBuildDirectoryStructureService buildDirectoryStructureService = IoC.Resolve<IBuildDirectoryStructureService>();
			
			IDirInfo site = buildDirectoryStructureService.Execute(dirPath, ignore);

			site.RemoveEmptyDirectories();
			
			IDirectoryStructureToClassDescriptorsService directoryStructureToClassDescriptorsService = IoC.Resolve<IDirectoryStructureToClassDescriptorsService>();

			ClassDescriptor[] classDescriptors = directoryStructureToClassDescriptorsService.Execute(site);

			IDescriptorsToClassesService descriptorsToClassesService = IoC.Resolve<IDescriptorsToClassesService>();

			string[] classes = descriptorsToClassesService.Execute(classDescriptors);

			string generatedClasses = string.Join(Environment.NewLine, classes);

			string generatedFile = string.Format(@"
namespace {0}
{{
	public static class Static
	{{
		public static Root Site = new Root();
	}}
{1}
}}
", generatedNamespace, generatedClasses);
				
			File.WriteAllText(generatedFilePath, generatedFile);

		}
	}
}
