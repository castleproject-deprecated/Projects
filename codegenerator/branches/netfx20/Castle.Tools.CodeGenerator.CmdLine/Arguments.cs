using System;
using System.IO;
using CommandLine;

namespace Castle.Tools.CodeGenerator.CmdLine
{
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
			get
			{
				return Path.Combine(ProjectPath, ProjectFileName);
			}
		}

		public string OutputFilePath
		{
			get
			{
				return Path.Combine(ProjectPath, OutputFileName);
			}
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