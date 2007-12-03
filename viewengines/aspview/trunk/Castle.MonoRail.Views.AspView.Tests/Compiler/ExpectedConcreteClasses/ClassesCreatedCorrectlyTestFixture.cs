#region license
// Copyright 2006-2007 Ken Egozi http://www.kenegozi.com/
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


namespace Castle.MonoRail.Views.AspView.Tests.Compiler.ExpectedConcreteClasses
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using AspView.Compiler;
	using NUnit.Framework;

	[TestFixture]
	public class ClassesCreatedCorrectlyTestFixture
	{
		const string TestSiteRoot = @"..\..\..\AspViewTestSite";
		const string ExpectedConcreteClassesPath = @"..\..\Compiler\ExpectedConcreteClasses";

		[Test]
		public void Compiler_OnTestSite_GeneratesCorrectClasses()
		{
			List<SourceFile> files = AspViewCompiler.GetSourceFiles(TestSiteRoot);

			AspViewCompiler compiler = new AspViewCompiler(new AspViewCompilerOptions());

			compiler.ApplyPreCompilationStepsOn(files);

			foreach (SourceFile file in files)
			{
				Console.Write("Testing [{0}] ...", file.ViewName);
				AssertThatTheConcreteClassIsCorrectFor(file);
				Console.WriteLine("ok.", file.ViewName);
			}

		}

		private static void AssertThatTheConcreteClassIsCorrectFor(SourceFile file)
		{
			string expectedPath = Path.Combine(
				ExpectedConcreteClassesPath, file.ClassName + ".cs");
			string expected = File.ReadAllText(expectedPath);
			Assert.AreEqual(expected, file.ConcreteClass, "Concrete class on [{0}] differs.", file.ViewName);
		}
	}
}
