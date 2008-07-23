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
	using System.CodeDom;
	using System.IO;
	using Model.TreeNodes;
	using Services;
	using Services.Generators;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;
	using Microsoft.Build.BuildEngine;
	using Microsoft.Build.Framework;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Rhino.Mocks.Constraints;
	using ILogger = Services.ILogger;

	[TestFixture]
	[Ignore("If you understand what causes 'System.EntryPointNotFoundException: Entry point was not found.' under MSBuild, please fix me.")]
	public class GenerateMonoRailSiteTreeTaskTests
	{
		private MockRepository mocks;
		private Engine engine;
		private IBuildEngine buildEngine;
		private GenerateMonoRailSiteTreeTask task;
		private Project project;
		private ILogger logger;
		private INamingService naming;
		private ISiteTreeGeneratorService parserService;
		private IParsedSourceStorageService sourceStorage;
		private ISourceGenerator source;
		private ITypeResolver typeResolver;
		private ITreeCreationService treeService;
		private IViewSourceMapper viewSourceMapper;
		private IGenerator generator;
		private ITaskItem item;
		private IParser parsedSource;
		private CodeCompileUnit ccu;

		[SetUp]
		public void Setup()
		{
			ccu = new CodeCompileUnit();
			mocks = new MockRepository();
			engine = Engine.GlobalEngine;
			engine.BinPath = @"C:\Program Files (x86)\MSBuild";
			project = new Project();
			buildEngine = mocks.DynamicMock<MockBuildEngine>(project);

			logger = new NullLogger();
			parserService = mocks.DynamicMock<ISiteTreeGeneratorService>();
			naming = mocks.DynamicMock<INamingService>();
			sourceStorage = mocks.DynamicMock<IParsedSourceStorageService>();
			source = mocks.DynamicMock<ISourceGenerator>();
			typeResolver = mocks.DynamicMock<ITypeResolver>();
			treeService = mocks.DynamicMock<ITreeCreationService>();
			viewSourceMapper = mocks.DynamicMock<IViewSourceMapper>();
			generator = mocks.DynamicMock<IGenerator>();

			task = new GenerateMonoRailSiteTreeTask(logger, parserService, naming, source, sourceStorage, typeResolver,
			                                         treeService, viewSourceMapper, generator);

			item = mocks.DynamicMock<ITaskItem>();
			parsedSource = mocks.DynamicMock<IParser>();
		}

		[Test]
		public void Execute_NoFiles_ReturnsTrue()
		{
			var root = new TreeNode("Root");
			
			using (mocks.Unordered())
			{
				buildEngine.LogMessageEvent(null);
				LastCall.IgnoreArguments().Repeat.Any();
				Expect.Call(treeService.Root).Return(root).Repeat.Any();
				generator.Generate(root);
				Expect.Call(source.Ccu).Return(ccu).Repeat.Any();
			}

			task.BuildEngine = buildEngine;
			task.Sources = new ITaskItem[0];
			task.ControllerSources = new ITaskItem[0];
			task.ViewSources = new ITaskItem[0];
			task.Namespace = "Eleutian.Empty";
			task.OutputFile = "SiteMap.generated.cs";

			mocks.ReplayAll();
			Assert.IsTrue(task.Execute());
			Assert.AreEqual("SiteMap.generated.cs", task.GeneratedItems[0].ItemSpec);

			File.Delete(task.OutputFile);
		}

		[Test]
		public void Execute_OneControllerSource_ReturnsTrue()
		{
			var root = new TreeNode("Root");
			
			using (mocks.Unordered())
			{
				buildEngine.LogMessageEvent(null);
				LastCall.IgnoreArguments().Repeat.Any();
				Expect.Call(item.GetMetadata("FullPath")).Return("HomeController.cs").Repeat.Any();
				Expect.Call(sourceStorage.GetParsedSource("HomeController.cs")).Return(parsedSource);
				Expect.Call(parsedSource.CompilationUnit).Return(new CompilationUnit());
				Expect.Call(treeService.Root).Return(root).Repeat.Any();
				generator.Generate(root);
				Expect.Call(source.Ccu).Return(ccu).Repeat.Any();
				typeResolver.Clear();
			}

			task.BuildEngine = buildEngine;
			task.Sources = new ITaskItem[0];
			task.ControllerSources = new[] {item};
			task.ViewSources = new ITaskItem[0];
			task.Namespace = "Eleutian.Empty";
			task.OutputFile = "SiteMap.generated.cs";

			mocks.ReplayAll();
			Assert.IsTrue(task.Execute());
			Assert.AreEqual("SiteMap.generated.cs", task.GeneratedItems[0].ItemSpec);

			File.Delete(task.OutputFile);
		}

		[Test]
		public void Execute_OneViewSource_ReturnsTrue()
		{
			var root = new TreeNode("Root");
			
			using (mocks.Unordered())
			{
				buildEngine.LogMessageEvent(null);
				LastCall.IgnoreArguments().Repeat.Any();
				Expect.Call(item.GetMetadata("FullPath")).Return("Index.brail").Repeat.Any();
				viewSourceMapper.AddViewSource("Index.brail");
				Expect.Call(treeService.Root).Return(root).Repeat.Any();
				generator.Generate(root);
				Expect.Call(source.Ccu).Return(ccu).Repeat.Any();
			}

			task.BuildEngine = buildEngine;
			task.Sources = new ITaskItem[0];
			task.ControllerSources = new ITaskItem[0];
			task.ViewSources = new[] {item};
			task.Namespace = "Eleutian.Empty";
			task.OutputFile = "SiteMap.generated.cs";

			mocks.ReplayAll();
			Assert.IsTrue(task.Execute());
			Assert.AreEqual("SiteMap.generated.cs", task.GeneratedItems[0].ItemSpec);

			File.Delete(task.OutputFile);
		}

		[Test]
		public void Execute_WithEvaluatedItems_Works()
		{
			var root = new TreeNode("Root");
			File.CreateText(@"CoolSuperSourceCode.cs").Close();

			using (mocks.Unordered())
			{
				buildEngine.LogMessageEvent(null);
				LastCall.IgnoreArguments().Repeat.Any();
				Expect.Call(item.GetMetadata("FullPath")).Return("CoolSuperSourceCode.cs").Repeat.Any();
				parserService.Parse(null, null);
				LastCall.Constraints(Is.NotNull(), Is.Equal("CoolSuperSourceCode.cs"));
				Expect.Call(treeService.Root).Return(root).Repeat.Any();
				generator.Generate(root);
				Expect.Call(source.Ccu).Return(ccu).Repeat.Any();
			}

			task.BuildEngine = buildEngine;
			task.Sources = new[] {item};
			task.ControllerSources = new ITaskItem[0];
			task.ViewSources = new ITaskItem[0];
			task.Namespace = "Eleutian.Empty";
			task.OutputFile = "SiteMap.generated.cs";

			mocks.ReplayAll();
			Assert.IsTrue(task.Execute());
			Assert.AreEqual("SiteMap.generated.cs", task.GeneratedItems[0].ItemSpec);

			File.Delete(task.OutputFile);
		}
	}
}