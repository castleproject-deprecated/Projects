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
	using NUnit.Framework;

	[TestFixture]
	public class ViewComponentTagsStepTestFixture : AbstractPreCompilationStepTestFixture
	{
		protected override void CreateStep()
		{
			step = new ViewComponentTagsStep();
		}

		[Test]
		public void Process_WhenThereAreNoViewComponentTags_DoesNothing()
		{
			string source = @"
dkllgk
fgkdlfk
dfg
fdslk";

			file.RenderBody = source;
			step.Process(file);

			Assert.AreEqual(source, file.RenderBody);
		}

		[Test]
		public void Process_ViewComponentsWithStringAttribute_Transforms()
		{
			string source = @"
before
<component:Simple name=""<%= ""Ken"" %>""></component:Simple>
after
";
			expected = @"
before
<% InvokeViewComponent(""Simple"", null, new KeyValuePair<string, object>[] {  } , ""name"", ""Ken""); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			System.Console.WriteLine(file.RenderBody);

			AssertStepOutput();
		}

		[Test]
		public void Process_ViewComponentsWithVariableAttribute_Transforms()
		{
			string source = @"
before
<component:Simple age=""<%= myAge %>""></component:Simple>
after
";
			expected = @"
before
<% InvokeViewComponent(""Simple"", null, new KeyValuePair<string, object>[] {  } , ""age"", myAge); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_ViewComponentsWithVariableAndDotAttribute_Transforms()
		{
			string source = @"
before
<component:Simple age=""<%= me.Age %>""></component:Simple>
after
";
			expected = @"
before
<% InvokeViewComponent(""Simple"", null, new KeyValuePair<string, object>[] {  } , ""age"", me.Age); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_ViewComponentsWithComplexVariableAttribute_Transforms()
		{
			string source = @"
before
<component:Simple age=""<%= people[index].GetAge(In.Years) %>""></component:Simple>
after
";
			 expected = @"
before
<% InvokeViewComponent(""Simple"", null, new KeyValuePair<string, object>[] {  } , ""age"", people[index].GetAge(In.Years)); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}
	}
}
