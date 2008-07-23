// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Tools.CodeGenerator.CmdLine
{
	using System;
	using System.IO;
	using CommandLine;

	public class Arguments
	{
		[Argument(ArgumentType.Required, HelpText = "The path to the folder containing the website.")] 
		public string ProjectPath;

		[Argument(ArgumentType.Required, HelpText = "The name of the csproj file of the website.")] 
		public string ProjectFileName;

		[Argument(ArgumentType.AtMostOnce, DefaultValue = "SiteMap.generated.cs", HelpText = "The name of the generated file.")] 
		public string OutputFileName;

		[Argument(ArgumentType.AtMostOnce, DefaultValue = "SiteMap", HelpText = "The namespace containing the generated code.")] 
		public string OutputNamespace;

		[Argument(ArgumentType.AtMostOnce, DefaultValue = "Views", HelpText = "The name of the folder containing your views.")] 
		public string ViewsFolder;

		[Argument(ArgumentType.AtMostOnce, DefaultValue = "aspx,brail,nv", ShortName = "E", HelpText = "A comma-separated list of file extensions which denote the view files.")] 
		public string ViewExtensions;

		[Argument(ArgumentType.AtMostOnce, DefaultValue = "Controllers", HelpText = "The name of the folder containing your controllers.")] 
		public string ControllersFolder;

		public string ProjectFilePath
		{
			get { return Path.Combine(ProjectPath, ProjectFileName); }
		}

		public string OutputFilePath
		{
			get { return Path.Combine(ProjectPath, OutputFileName); }
		}

		public bool IsValid()
		{
			if (!Directory.Exists(ProjectPath))
			{
				Console.Error.WriteLine("Project Path not found:" + ProjectPath);
				return false;
			}

			if (!File.Exists(Path.Combine(ProjectPath, ProjectFileName)))
			{
				Console.Error.WriteLine("Project Filename not found:" + Path.Combine(ProjectPath, ProjectFileName));
				return false;
			}

			if (string.IsNullOrEmpty(OutputFileName))
			{
				Console.Error.WriteLine("You must choose an Output Filename");
				return false;
			}

			if (string.IsNullOrEmpty(OutputNamespace))
			{
				Console.Error.WriteLine("You must choose an Output Namespace");
				return false;
			}

			if (!Directory.Exists(Path.Combine(ProjectPath, ViewsFolder)))
			{
				Console.Error.WriteLine("Views Folder not found:" + Path.Combine(ProjectPath, ViewsFolder));
				return false;
			}

			if (!Directory.Exists(Path.Combine(ProjectPath, ControllersFolder)))
			{
				Console.Error.WriteLine("Controllers Folder not found:" + Path.Combine(ProjectPath, ControllersFolder));
				return false;
			}

			if (string.IsNullOrEmpty(ViewExtensions))
			{
				Console.Error.WriteLine("You must choose some View Extensions");
				return false;
			}

			return true;
		}
	}
}