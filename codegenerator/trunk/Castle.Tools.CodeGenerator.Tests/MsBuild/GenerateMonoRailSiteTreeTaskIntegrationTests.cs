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
	using System.IO;
	using Microsoft.Build.BuildEngine;
	using Microsoft.Build.Framework;
	using NUnit.Framework;
	using Rhino.Mocks;
	
	[TestFixture]
	[Ignore("If you understand what causes 'System.EntryPointNotFoundException: Entry point was not found.' under MSBuild, please fix me.")]
	public class GenerateMonoRailSiteTreeTaskIntegrationTests
	{
		private Engine engine;
		private IBuildEngine buildEngine;
		private GenerateMonoRailSiteTreeTask task;
		private Project project;

		[SetUp]
		public void Setup()
		{
			engine = Engine.GlobalEngine;
			engine.BinPath = Directory.Exists(@"C:\Program Files (x86)\MSBuild") ? @"C:\Program Files (x86)\MSBuild" : @"C:\Program Files\MSBuild";
			project = new Project();
			buildEngine = MockRepository.GenerateMock<MockBuildEngine>(project);
			task = new GenerateMonoRailSiteTreeTask();
		}

		[Test]
		public void Execute_NoFiles_ReturnsTrue()
		{
			buildEngine.Expect(e => e.LogMessageEvent(Arg<BuildMessageEventArgs>.Is.Anything)).Repeat.Any();

			task.BuildEngine = buildEngine;
			task.Sources = new ITaskItem[0];
			task.ControllerSources = new ITaskItem[0];
			task.ViewSources = new ITaskItem[0];
			task.Namespace = "Eleutian.Empty";
			task.OutputFile = "SiteMap.generated.cs";

			Assert.IsTrue(task.Execute());
			Assert.AreEqual("SiteMap.generated.cs", task.GeneratedItems[0].ItemSpec);

			File.Delete(task.OutputFile);
		}
	}
}