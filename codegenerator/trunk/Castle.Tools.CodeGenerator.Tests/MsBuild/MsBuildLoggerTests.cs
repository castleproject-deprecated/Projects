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
	using Microsoft.Build.Framework;
	using Microsoft.Build.Utilities;
	using Rhino.Mocks;
	using NUnit.Framework;

	[TestFixture]
	[Ignore("If you understand what causes 'System.EntryPointNotFoundException: Entry point was not found.' under MSBuild, please fix me.")]
	public class MsBuildLoggerTests
	{
		private MsBuildLogger logger;
		private ITask task;
		private IBuildEngine engine;

		[SetUp]
		public void Setup()
		{
			task = MockRepository.GenerateStub<ITask>();
			engine = MockRepository.GenerateMock<IBuildEngine>();
			logger = new MsBuildLogger(new TaskLoggingHelper(task));

			task.Expect(t => t.BuildEngine).Return(engine).Repeat.Any();
		}

		[Test]
		public void LogInfo_Always_Works()
		{
			logger.LogInfo("Hello {0}!", "World");

			engine.AssertWasCalled(e => e.LogMessageEvent(Arg<BuildMessageEventArgs>.Is.Anything));
		}
	}
}