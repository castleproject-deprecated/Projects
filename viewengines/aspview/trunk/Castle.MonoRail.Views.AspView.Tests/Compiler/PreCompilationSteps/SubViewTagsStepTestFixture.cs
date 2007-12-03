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
	public class SubViewTagsStepTestFixture
	{
		readonly IPreCompilationStep step = new SubViewTagsStep();

		SourceFile file;

		[SetUp]
		public void Setup()
		{
			file = new SourceFile();
		}

		[Test]
		public void Process_WhenThereAreNoSubViewTags_DoesNothing()
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
		public void Process_SimpleSubViewTag_Transforms()
		{
			string source = @"
before
<subView:Simple></subView:Simple>
after
";
			string expected = @"
before
<% OutputSubView(""Simple""); %>
after
";

			file.ViewSource = source;
			step.Process(file);

			Assert.AreEqual(expected, file.ViewSource);
		}

		[Test]
		public void Process_TwoSubViewTags_Transforms()
		{
			string source = @"
before
<subView:Simple></subView:Simple>
<subView:Simple2></subView:Simple2>
after
";
			string expected = @"
before
<% OutputSubView(""Simple""); %>
<% OutputSubView(""Simple2""); %>
after
";

			file.ViewSource = source;
			step.Process(file);

			Assert.AreEqual(expected, file.ViewSource);
		}

		[Test]
		public void Process_SubViewsWithSimpleStringAttribute_Transforms()
		{
			string source = @"
before
<subView:Simple name=""Ken""></subView:Simple>
after
";
			string expected = @"
before
<% OutputSubView(""Simple"", ""name"", ""Ken""); %>
after
";

			file.ViewSource = source;
			step.Process(file);

			Assert.AreEqual(expected, file.ViewSource);
		}

		[Test]
		public void Process_SubViewsWithConstantAttribute_Transforms()
		{
			string source = @"
before
<subView:Simple age=""<%= 29 %>""></subView:Simple>
after
";
			string expected = @"
before
<% OutputSubView(""Simple"", ""age"", 29); %>
after
";

			file.ViewSource = source;
			step.Process(file);

			Assert.AreEqual(expected, file.ViewSource);
		}

		[Test]
		public void Process_SubViewsWithStringAttribute_Transforms()
		{
			string source = @"
before
<subView:Simple name=""<%= ""Ken"" %>""></subView:Simple>
after
";
			string expected = @"
before
<% OutputSubView(""Simple"", ""name"", ""Ken""); %>
after
";

			file.ViewSource = source;
			step.Process(file);

			Assert.AreEqual(expected, file.ViewSource);
		}

		[Test]
		public void Process_SubViewsWithVariableAttribute_Transforms()
		{
			string source = @"
before
<subView:Simple age=""<%= myAge %>""></subView:Simple>
after
";
			string expected = @"
before
<% OutputSubView(""Simple"", ""age"", myAge); %>
after
";

			file.ViewSource = source;
			step.Process(file);

			Assert.AreEqual(expected, file.ViewSource);
		}

		[Test]
		public void Process_SubViewsWithVariableAndDotAttribute_Transforms()
		{
			string source = @"
before
<subView:Simple age=""<%= me.Age %>""></subView:Simple>
after
";
			string expected = @"
before
<% OutputSubView(""Simple"", ""age"", me.Age); %>
after
";

			file.ViewSource = source;
			step.Process(file);

			Assert.AreEqual(expected, file.ViewSource);
		}

		[Test]
		public void Process_SubViewsWithComplexVariableAttribute_Transforms()
		{
			string source = @"
before
<subView:Simple age=""<%= people[index].GetAge(In.Years) %>""></subView:Simple>
after
";
			string expected = @"
before
<% OutputSubView(""Simple"", ""age"", people[index].GetAge(In.Years)); %>
after
";

			file.ViewSource = source;
			step.Process(file);

			Assert.AreEqual(expected, file.ViewSource);
		}
	}
}
