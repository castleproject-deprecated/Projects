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

namespace Castle.Tools.CodeGenerator.MsBuild
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using Services.Generators;
	using Services.Visitors;
	using Microsoft.Build.Framework;
	using Microsoft.Build.Utilities;
	using External;
	using Services;
	using ILogger = Services.ILogger;

	public class GenerateMonoRailSiteTreeTask : AppDomainIsolatedTask
	{
		private readonly ISiteTreeGeneratorService service;
		private readonly List<ITaskItem> generatedItems = new List<ITaskItem>();
		private readonly ILogger logger;
		private readonly INamingService naming;
		private readonly IParsedSourceStorageService sourceStorage;
		private readonly ISourceGenerator source;
		private readonly ITypeResolver typeResolver;
		private readonly ITreeCreationService treeService;
		private readonly IViewSourceMapper viewSourceMapper;
		private readonly List<IGenerator> generators = new List<IGenerator>();

		public GenerateMonoRailSiteTreeTask()
		{
			logger = new MsBuildLogger(Log);
			naming = new DefaultNamingService();
			treeService = new DefaultTreeCreationService();
			source = new DefaultSourceGenerator();
			viewSourceMapper = new ViewSourceMapper(logger, treeService);
			sourceStorage = new DefaultSourceStorageService();
			typeResolver = new TypeResolver();
			service = new SiteTreeGeneratorService(logger, typeResolver, sourceStorage, new NRefactoryParserFactory());
			ServiceTypeName = typeof(ICodeGeneratorServices).FullName;
			ViewComponentSources = new ITaskItem[0];
			AssemblyReferences = new ITaskItem[0];
		}

		public GenerateMonoRailSiteTreeTask(ILogger logger, ISiteTreeGeneratorService service, INamingService naming,
											ISourceGenerator source, IParsedSourceStorageService sourceStorage,
											ITypeResolver typeResolver, ITreeCreationService treeService,
											IViewSourceMapper viewSourceMapper, IGenerator generator)
		{
			this.service = service;
			this.logger = logger;
			this.naming = naming;
			this.source = source;
			this.sourceStorage = sourceStorage;
			this.typeResolver = typeResolver;
			this.treeService = treeService;
			this.viewSourceMapper = viewSourceMapper;
			generators.Add(generator);
			ServiceTypeName = typeof(ICodeGeneratorServices).FullName;
			ViewComponentSources = new ITaskItem[0];
			AssemblyReferences = new ITaskItem[0];
		}

		[Required] public string Namespace { get; set; }
		[Required] public string OutputFile { get; set; }
        [Required] public ITaskItem[] ControllerSources { get; set; }
		[Required] public ITaskItem[] Sources { get; set; } 
		[Required] public ITaskItem[] ViewSources { get; set; }
        [Output] public ITaskItem[] GeneratedItems { get { return generatedItems.ToArray(); } }

		public ITaskItem[] AssemblyReferences { get; set; }
		public ITaskItem[] ViewComponentSources { get; set; }
		
		public bool Debug { get; set; }
		public string ServiceTypeName { get; set; }

		public override bool Execute()
		{
			Log.LogMessage(MessageImportance.High, "OutputFile: {0}", OutputFile);
			Log.LogMessage(MessageImportance.High, "Namespace: {0}", Namespace);
			Log.LogMessage(MessageImportance.High, "ControllerSources: {0}", ControllerSources.Length);
			Log.LogMessage(MessageImportance.High, "ViewSources: {0}", ViewSources.Length);
			Log.LogMessage(MessageImportance.High, "ViewComponentSources: {0}", ViewComponentSources.Length);
			Log.LogMessage(MessageImportance.High, "Loading References...");

			if (Debug) System.Diagnostics.Debugger.Launch();

			foreach (var reference in AssemblyReferences)
			{
				try
				{
					Assembly.LoadFrom(reference.ItemSpec);
					Log.LogMessage(MessageImportance.Low, "Loaded: {0}", reference.ItemSpec);
				}
				catch (Exception ex)
				{
					Log.LogMessage(MessageImportance.High, "Error loading reference: '{0}': {1}", reference.ItemSpec, ex.Message);
				}
			}

			if (File.Exists(OutputFile))
				File.Delete(OutputFile);
			
			Log.LogMessage(MessageImportance.High, "Parsing Sources...");

			foreach (var ti in Sources)
			{
				var filePath = ti.GetMetadata("FullPath");
				
				if (File.Exists(filePath))
				{
					var visitor = new TypeInspectionVisitor(typeResolver);
					service.Parse(visitor, filePath);
				}
			}

			foreach (var item in ViewComponentSources)
			{
				typeResolver.Clear();

				var visitor = new ViewComponentVisitor(logger, typeResolver, treeService);
				var filePath = item.GetMetadata("FullPath");
				visitor.VisitCompilationUnit(sourceStorage.GetParsedSource(filePath).CompilationUnit, null);
			}

			foreach (var item in ControllerSources)
			{
				typeResolver.Clear();

				var visitor = new ControllerVisitor(logger, typeResolver, treeService);
				var filePath = item.GetMetadata("FullPath");
				visitor.VisitCompilationUnit(sourceStorage.GetParsedSource(filePath).CompilationUnit, null);
			}

			foreach (var bi in ViewSources)
			{
				var filePath = bi.GetMetadata("FullPath");
				viewSourceMapper.AddViewSource(filePath);
			}

			Log.LogMessage(MessageImportance.High, "Generating {0}", OutputFile);

			new Generator(Namespace, OutputFile, ServiceTypeName, logger, naming, source, treeService).Execute();
			generatedItems.Add(new TaskItem(OutputFile));

			return true;
		}
	}
}