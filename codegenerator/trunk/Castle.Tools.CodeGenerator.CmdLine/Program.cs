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

using System.Linq;

namespace Castle.Tools.CodeGenerator.CmdLine
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Xml;
	using System.Xml.XPath;
	using MsBuild;
	using Services;
	using Services.Generators;
	using Services.Visitors;
	using CommandLine;
	using External;

	public class Program
	{
		private static Arguments arguments;
		private static readonly ILogger logger = new NullLogger();
		private static readonly INamingService namingService = new DefaultNamingService();
		private static readonly IParserFactory parserFactory = new NRefactoryParserFactory();
		private static readonly IParsedSourceStorageService parsedSourceStorageService = new DefaultSourceStorageService();
		private static readonly ISourceGenerator sourceGenerator = new DefaultSourceGenerator();
		private static readonly ITreeCreationService treeCreationService = new DefaultTreeCreationService();
		private static readonly ITypeResolver typeResolver = new TypeResolver();
		private static readonly ISiteTreeGeneratorService service = new SiteTreeGeneratorService(
			logger, typeResolver, parsedSourceStorageService, parserFactory);

		private static readonly IViewSourceMapper viewSourceMapper = new ViewSourceMapper(logger, treeCreationService);

		public static void Main(string[] args)
		{
			arguments = new Arguments();

			if (!Parser.ParseArgumentsWithUsage(args, arguments) || !arguments.IsValid()) return;

			LoadReferences();
			DeleteExistingOutputFile();

			var sources = GetSources();
			Console.Out.WriteLine(string.Format("Castle.Tools.CodeGenerator: Parsing {0} sources...", sources.Count));
			ParseSources(sources);

			var controllerSources = GetControllerSources();
			Console.Out.WriteLine(string.Format("Castle.Tools.CodeGenerator: Parsing {0} controller sources...",
			                                    controllerSources.Count));
			ParseControllerSources(controllerSources);

			var viewSources = GetViewSources();
			Console.Out.WriteLine(string.Format("Castle.Tools.CodeGenerator: Parsing {0} view sources...", viewSources.Count));
			ParseViewSources(viewSources);

			var rootNamespace = GetRootNamespace();
			Console.Out.WriteLine(string.Format("Castle.Tools.CodeGenerator: Using root namespace: {0}", rootNamespace));

			var serviceType = typeof (ICodeGeneratorServices).FullName;
			var nameSpace = rootNamespace + "." + arguments.OutputNamespace;
			new Generator(nameSpace, arguments.OutputFilePath, serviceType, logger, namingService, sourceGenerator, treeCreationService).Execute();			
		}

		private static void DeleteExistingOutputFile()
		{
			if (File.Exists(arguments.OutputFilePath))
				File.Delete(arguments.OutputFilePath);
		}

		private static List<string> GetControllerSources()
		{
			var sources = new List<string>();
			var controllerFolderPath = Path.Combine(arguments.ProjectPath, arguments.ControllersFolder);

			sources.AddRange(Directory.GetFiles(controllerFolderPath, "*Controller.cs", SearchOption.AllDirectories));

			return sources;
		}

		private static XPathNodeIterator GetCsprojNodeIterator(string select)
		{
			var navigator = new XPathDocument(arguments.ProjectFilePath).CreateNavigator();
			var namespaceManager = new XmlNamespaceManager(navigator.NameTable);
			namespaceManager.AddNamespace("pr", "http://schemas.microsoft.com/developer/msbuild/2003");
			
			return navigator.Select(select, namespaceManager);
		}

		private static string GetRootNamespace()
		{
			var nodeIterator = GetCsprojNodeIterator(@"pr:Project/pr:PropertyGroup/pr:RootNamespace");
			nodeIterator.MoveNext();

			return nodeIterator.Current.Value;
		}

		private static List<string> GetSources()
		{
			var sources = new List<string>();
			var nodeIterator = GetCsprojNodeIterator(@"pr:Project/pr:ItemGroup/pr:Compile");

			while (nodeIterator.MoveNext())
				sources.Add(Path.Combine(arguments.ProjectPath, nodeIterator.Current.GetAttribute("Include", string.Empty)));

			return sources;
		}

		private static List<string> GetViewSources()
		{
			var sources = new List<string>();
			var extensions = arguments.ViewExtensions.Split(',');
			var viewFolderPath = Path.Combine(arguments.ProjectPath, arguments.ViewsFolder);

			foreach (var extension in extensions)
				sources.AddRange(Directory.GetFiles(viewFolderPath, "*." + extension, SearchOption.AllDirectories));

			return sources;
		}

		private static void LoadReferences()
		{
			var nodeIterator = GetCsprojNodeIterator(@"pr:Project/pr:ItemGroup/pr:Reference/pr:HintPath");
			var loaded = 0;

			while (nodeIterator.MoveNext())
			{
				var assemblyPath = Path.Combine(arguments.ProjectPath, nodeIterator.Current.Value);
				Assembly.LoadFrom(assemblyPath);
				loaded++;
			}

			nodeIterator = GetCsprojNodeIterator(@"pr:Project/pr:PropertyGroup/pr:OutputPath");
			var outputPaths = new List<string>();

			while (nodeIterator.MoveNext())
				outputPaths.Add(Path.Combine(arguments.ProjectPath, nodeIterator.Current.Value));
			
			nodeIterator = GetCsprojNodeIterator(@"pr:Project/pr:ItemGroup/pr:ProjectReference/pr:Name");

			while (nodeIterator.MoveNext())
			{
				string newestVersion = null;
				DateTime? newestDate = null;

				foreach (var outputPath in outputPaths)
				{
					var assemblyPath = Path.Combine(outputPath, nodeIterator.Current.Value + ".dll");
					
					if (!File.Exists(assemblyPath)) continue;

					var editDate = File.GetLastWriteTime(assemblyPath);

					if (newestDate.HasValue && editDate <= newestDate) continue;

					newestDate = editDate;
					newestVersion = assemblyPath;
				}
				
				if (newestVersion != null)
				{
					Assembly.LoadFrom(newestVersion);
					loaded++;
				}
				else
					Console.Out.WriteLine("Castle.Tools.CodeGenerator: Warning - could not find a compiled version of {0}", nodeIterator.Current.Value);
			}

			Console.Out.WriteLine(string.Format("Castle.Tools.CodeGenerator: Loaded {0} references... ", loaded));
		}

		private static void ParseControllerSources(IEnumerable<string> controllerSources)
		{
			foreach (var controllerSource in controllerSources)
			{
				typeResolver.Clear();

				var visitor = new ControllerVisitor(logger, typeResolver, treeCreationService);
				visitor.VisitCompilationUnit(parsedSourceStorageService.GetParsedSource(controllerSource).CompilationUnit, null);
			}
		}

		private static void ParseSources(IEnumerable<string> sources)
		{
			foreach (var source in sources.Where(File.Exists))
				service.Parse(new TypeInspectionVisitor(typeResolver), source);
		}

		private static void ParseViewSources(IEnumerable<string> viewSources)
		{
			foreach (var source in viewSources)
				viewSourceMapper.AddViewSource(source);
		}
	}
}