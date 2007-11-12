using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using Castle.Tools.CodeGenerator.MsBuild;
using Castle.Tools.CodeGenerator.Services;
using CommandLine;

namespace Castle.Tools.CodeGenerator.CmdLine
{
	public class Program
	{
		private static Arguments arguments;
		private static readonly List<IGenerator> generators = new List<IGenerator>();
		private static readonly ILogger logger = new NullLogger();
		private static readonly INamingService namingService = new DefaultNamingService();
		private static readonly IParserFactory parserFactory = new NRefactoryParserFactory();
		private static readonly IParsedSourceStorageService parsedSourceStorageService = new DefaultSourceStorageService();
		private static readonly ISourceGenerator sourceGenerator = new DefaultSourceGenerator();
		private static readonly ITreeCreationService treeCreationService = new DefaultTreeCreationService();
		private static readonly ITypeResolver typeResolver = new TypeResolver();		
		private static readonly ISiteTreeGeneratorService service = new SiteTreeGeneratorService(logger, typeResolver, parsedSourceStorageService, parserFactory);
		private static readonly IViewSourceMapper viewSourceMapper = new ViewSourceMapper(logger, treeCreationService, namingService);
		
		public static void Main(string[] args)
		{
			arguments = new Arguments();
			
			if (Parser.ParseArgumentsWithUsage(args, arguments) && arguments.IsValid())
			{
				LoadReferences();
				DeleteExistingOutputFile();

				List<string> sources = GetSources();
				Console.Out.WriteLine(string.Format("Castle.Tools.CodeGenerator: Parsing {0} sources...", sources.Count));
				ParseSources(sources);

				List<string> controllerSources = GetControllerSources();
				Console.Out.WriteLine(string.Format("Castle.Tools.CodeGenerator: Parsing {0} controller sources...", controllerSources.Count));
				ParseControllerSources(controllerSources);

				List<string> viewSources = GetViewSources();
				Console.Out.WriteLine(string.Format("Castle.Tools.CodeGenerator: Parsing {0} view sources...", viewSources.Count));
				ParseViewSources(viewSources);

				string rootNamespace = GetRootNamespace();
				Console.Out.WriteLine(string.Format("Castle.Tools.CodeGenerator: Using root namespace: {0}", rootNamespace));

				string serviceType = typeof(ICodeGeneratorServices).FullName;
				string nameSpace = rootNamespace + "." + arguments.OutputNamespace;
				Generator generator = new Generator(nameSpace, arguments.OutputFilePath, serviceType, logger, namingService, sourceGenerator, treeCreationService);
				generator.Execute();
			}
		}

		private static void DeleteExistingOutputFile()
		{
			if (File.Exists(arguments.OutputFilePath))
				File.Delete(arguments.OutputFilePath);
		}

		private static List<string> GetControllerSources()
		{
			List<string> sources = new List<string>();

			string controllerFolderPath = Path.Combine(arguments.ProjectPath, arguments.ControllersFolder);
			sources.AddRange(Directory.GetFiles(controllerFolderPath, "*Controller.cs", SearchOption.AllDirectories));

			return sources;
		}

		private static XPathNodeIterator GetCsprojNodeIterator(string select)
		{
			XPathNavigator navigator = new XPathDocument(arguments.ProjectFilePath).CreateNavigator();
			XmlNamespaceManager namespaceManager = new XmlNamespaceManager(navigator.NameTable);
			namespaceManager.AddNamespace("pr", "http://schemas.microsoft.com/developer/msbuild/2003");
			return navigator.Select(select, namespaceManager);
		}

		private static string GetRootNamespace()
		{
			XPathNodeIterator nodeIterator = GetCsprojNodeIterator(@"pr:Project/pr:PropertyGroup/pr:RootNamespace");
			nodeIterator.MoveNext();

			return nodeIterator.Current.Value;
		}

		private static List<string> GetSources()
		{
			List<string> sources = new List<string>();

			XPathNodeIterator nodeIterator = GetCsprojNodeIterator(@"pr:Project/pr:ItemGroup/pr:Compile");

			while (nodeIterator.MoveNext())
				sources.Add(Path.Combine(arguments.ProjectPath, nodeIterator.Current.GetAttribute("Include", string.Empty)));

			return sources;
		}

		private static List<string> GetViewSources()
		{
			List<string> sources = new List<string>();
			string[] extensions = arguments.ViewExtensions.Split(',');
			string viewFolderPath = Path.Combine(arguments.ProjectPath, arguments.ViewsFolder);

			foreach (string extension in extensions)
				sources.AddRange(Directory.GetFiles(viewFolderPath, "*." + extension, SearchOption.AllDirectories));

			return sources;
		}

		private static void LoadReferences()
		{
			XPathNodeIterator nodeIterator = GetCsprojNodeIterator(@"pr:Project/pr:ItemGroup/pr:Reference/pr:HintPath");
			int loaded = 0;

			while (nodeIterator.MoveNext())
			{
				string assemblyPath = Path.Combine(arguments.ProjectPath, nodeIterator.Current.Value);				
				Assembly.LoadFrom(assemblyPath);
				loaded++;
			}

			nodeIterator = GetCsprojNodeIterator(@"pr:Project/pr:PropertyGroup/pr:OutputPath");
			nodeIterator.MoveNext();

			string outputPath = Path.Combine(arguments.ProjectPath, nodeIterator.Current.Value);

			nodeIterator = GetCsprojNodeIterator(@"pr:Project/pr:ItemGroup/pr:ProjectReference/pr:Name");
			
			while (nodeIterator.MoveNext())
			{
				string assemblyPath = Path.Combine(outputPath, nodeIterator.Current.Value + ".dll");
				Assembly.LoadFrom(assemblyPath);
				loaded++;
			}

			Console.Out.WriteLine(string.Format("Castle.Tools.CodeGenerator: Loaded {0} references... ", loaded));
		}

		private static void ParseControllerSources(List<string> controllerSources)
		{
			foreach (string controllerSource in controllerSources)
			{
				typeResolver.Clear();

				ControllerVisitor visitor = new ControllerVisitor(logger, typeResolver, treeCreationService);
				visitor.VisitCompilationUnit(parsedSourceStorageService.GetParsedSource(controllerSource).CompilationUnit, null);
			}
		}

		private static void ParseSources(List<string> sources)
		{
			foreach (string source in sources)
			{
				if (File.Exists(source))
				{
					TypeInspectionVisitor visitor = new TypeInspectionVisitor(typeResolver);
					service.Parse(visitor, source);
				}
			}
		}

		private static void ParseViewSources(List<string> viewSources)
		{
			foreach (string source in viewSources)
				viewSourceMapper.AddViewSource(source);
		}
	}
}