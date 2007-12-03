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

namespace Castle.MonoRail.Views.AspView.Tests.Compiler.PreCompilationSteps
{
	using AspView.Compiler;
	using AspView.Compiler.PreCompilationSteps;
	using NUnit.Framework;

	[TestFixture]
	public class ViewComponentTagsStepTestFixture
	{
		readonly IPreCompilationStep step = new ViewComponentTagsStep();

		SourceFile file;

		[SetUp]
		public void Setup()
		{
			file = new SourceFile();
		}

		[Test]
		public void Process_WhenThereAreNoViewComponentTags_DoesNothing()
		{
			string source = @"
dkllgk
fgkdlfk
dfg
fdslk";

			file.ViewSource = source;
			step.Process(file);

			Assert.AreEqual(source, file.ViewSource);
		}

		[Test]
		public void Process_ViewComponentsWithStringAttribute_Transforms()
		{
			string source = @"
before
<component:Simple name=""<%= ""Ken"" %>""></component:Simple>
after
";
			string expected = @"
before
<% OutputViewComponent(""Simple"", ""name"", ""Ken""); %>
after
";

			file.ViewSource = source;
			step.Process(file);

			System.Console.WriteLine(file.ViewSource);

			Assert.AreEqual(expected, file.ViewSource);
		}

		[Test]
		public void Process_ViewComponentsWithVariableAttribute_Transforms()
		{
			string source = @"
before
<component:Simple age=""<%= myAge %>""></component:Simple>
after
";
			string expected = @"
before
<% OutputViewComponent(""Simple"", ""age"", myAge); %>
after
";

			file.ViewSource = source;
			step.Process(file);

			Assert.AreEqual(expected, file.ViewSource);
		}

		[Test]
		public void Process_ViewComponentsWithVariableAndDotAttribute_Transforms()
		{
			string source = @"
before
<component:Simple age=""<%= me.Age %>""></component:Simple>
after
";
			string expected = @"
before
<% OutputViewComponent(""Simple"", ""age"", me.Age); %>
after
";

			file.ViewSource = source;
			step.Process(file);

			Assert.AreEqual(expected, file.ViewSource);
		}

		[Test]
		public void Process_ViewComponentsWithComplexVariableAttribute_Transforms()
		{
			string source = @"
before
<component:Simple age=""<%= people[index].GetAge(In.Years) %>""></component:Simple>
after
";
			string expected = @"
before
<% OutputViewComponent(""Simple"", ""age"", people[index].GetAge(In.Years)); %>
after
";

			file.ViewSource = source;
			step.Process(file);

			Assert.AreEqual(expected, file.ViewSource);
		}
	}
}
