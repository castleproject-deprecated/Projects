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
	using AspView.Compiler.PreCompilationSteps;
	using Xunit;

	
	public class ViewComponentTagsStepTestFixture : AbstractPreCompilationStepTestFixture
	{
		protected override void CreateStep()
		{
			step = new ViewComponentTagsStep();
		}

		[Fact]
		public void Process_WhenThereAreNoViewComponentTags_DoesNothing()
		{
			string source = @"
dkllgk
fgkdlfk
dfg
fdslk";

			file.RenderBody = source;
			step.Process(file);

			Assert.Equal(source, file.RenderBody);
		}

		[Fact]
		public void Process_ViewComponentsWithStringAttribute_Transforms()
		{
			string source = @"
before
<component:Simple name=""<%= ""Ken"" %>""></component:Simple>
after
";
			expected = @"
before
<% InvokeViewComponent(""Simple"", null, null, ""name"", ""Ken""); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			System.Console.WriteLine(file.RenderBody);

			AssertStepOutput();
		}

		[Fact]
		public void Process_ViewComponentsWithVariableAttribute_Transforms()
		{
			string source = @"
before
<component:Simple age=""<%= myAge %>""></component:Simple>
after
";
			expected = @"
before
<% InvokeViewComponent(""Simple"", null, null, ""age"", myAge); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Fact]
		public void Process_ViewComponentsWithVariableAndDotAttribute_Transforms()
		{
			string source = @"
before
<component:Simple age=""<%= me.Age %>""></component:Simple>
after
";
			expected = @"
before
<% InvokeViewComponent(""Simple"", null, null, ""age"", me.Age); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Fact]
		public void Process_ViewComponentsWithComplexVariableAttribute_Transforms()
		{
			string source = @"
before
<component:Simple age=""<%= people[index].GetAge(In.Years) %>""></component:Simple>
after
";
			 expected = @"
before
<% InvokeViewComponent(""Simple"", null, null, ""age"", people[index].GetAge(In.Years)); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Fact]
		public void Process_ViewComponentsWithBody_CreatesAndRegistersBodyHandler()
		{
			string source = @"
before
<component:Simple>
body
</component:Simple>
after
";

			file.RenderBody = source;
			step.Process(file);

			Assert.True(file.ViewComponentSectionHandlers.ContainsKey("Simple0_body"));
			string actualBody = file.ViewComponentSectionHandlers["Simple0_body"];
			string expectedBody = @"Output(@""
body
"");
";
			Assert.Equal(expectedBody, actualBody);
		}

		[Fact]
		public void Process_ViewComponentsWithBody_PassesBodyHandlerToTheComponent()
		{
			string source = @"
before
<component:Simple>
body
</component:Simple>
after
";
			expected = @"
before
<% InvokeViewComponent(""Simple"", Simple0_body, null); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Fact]
		public void Process_ViewComponentsWithSections_CreatesAndRegistersSectionHandlers()
		{
			string source = @"
before
<component:Simple>
<section:section1>
section1
</section:section1>
<section:section2>
section2
</section:section2>
body
</component:Simple>
after
";

			file.RenderBody = source;
			step.Process(file);

			Assert.True(file.ViewComponentSectionHandlers.ContainsKey("Simple0_body"));
			Assert.True(file.ViewComponentSectionHandlers.ContainsKey("Simple0_section1"));
			Assert.True(file.ViewComponentSectionHandlers.ContainsKey("Simple0_section2"));

			string actualBody = file.ViewComponentSectionHandlers["Simple0_body"];
			string actualSection1Body = file.ViewComponentSectionHandlers["Simple0_section1"];
			string actualSection2Body = file.ViewComponentSectionHandlers["Simple0_section2"];
			string expectedBody = @"Output(@""


body
"");
";
			string expectedSection1Body = @"Output(@""
section1
"");
";
			string expectedSection2Body = @"Output(@""
section2
"");
";
			Assert.Equal(expectedBody, actualBody);
			Assert.Equal(actualSection1Body, expectedSection1Body);
			Assert.Equal(actualSection2Body, expectedSection2Body);
		}
	}
}
