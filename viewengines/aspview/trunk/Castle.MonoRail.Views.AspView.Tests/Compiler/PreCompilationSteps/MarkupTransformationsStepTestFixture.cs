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
	using AspView.Compiler.MarkupTransformers;
	using AspView.Compiler.PreCompilationSteps;
	using NUnit.Framework;

	[TestFixture]
	public class MarkupTransformationsStepTestFixture : AbstractPreCompilationStepTestFixture
	{
		#region constants
		const string INPUT = "should not be seen";
		const string OUTPUT = "transformed";

		const string CODE = @"<% string s;
Output(s + ""Hello""); %>";

		const string MARKUP = "Markup" + INPUT + "Markup";
		const string TRANSFORMED_MARKUP = "Markup" + OUTPUT + "Markup";
		#endregion

		protected override void CreateStep()
		{
			step =
				new MarkupTransformationsStep(
					new MockMarkupTransformersProvider());
		}

		[Test]
		public void Process_WhenThereIsCodeOnly_NothingHappens()
		{
			file.RenderBody = CODE;

			expected = CODE;

			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_WhenThereAreTwoAdjacentCodeSections_NothingHappens()
		{
			file.RenderBody = CODE + CODE;

			expected = CODE + CODE;

			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_WhenThereIsMarkupOnly_Works()
		{
			file.RenderBody = MARKUP;

			expected = TRANSFORMED_MARKUP;

			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_WhenThereIsMarkupFollowedByCode_Works()
		{
			file.RenderBody = MARKUP + CODE;

			expected = TRANSFORMED_MARKUP + CODE;

			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_WhenThereIsCodeFollowedByMarkup_Works()
		{
			file.RenderBody = CODE + MARKUP;

			expected = CODE + TRANSFORMED_MARKUP;

			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_MarkupCodeMarkup_Works()
		{
			file.RenderBody = MARKUP + CODE + MARKUP;

			expected = TRANSFORMED_MARKUP + CODE + TRANSFORMED_MARKUP;

			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_CodeMarkupCode_Works()
		{
			file.RenderBody = CODE + MARKUP + CODE;

			expected = CODE + TRANSFORMED_MARKUP + CODE;

			step.Process(file);

			AssertStepOutput();
		}

		#region mocks
		class MockMarkupTransformersProvider : IMarkupTransformersProvider
		{
			public System.Collections.Generic.ICollection<IMarkupTransformer> GetMarkupTransformers()
			{
				return new IMarkupTransformer[] { new MockMarkupTransformer() };
			}
		}

		class MockMarkupTransformer : IMarkupTransformer
		{
			public string Transform(string markup)
			{
				return markup.Replace(INPUT, OUTPUT);
			}
		}
		#endregion
	}
}
