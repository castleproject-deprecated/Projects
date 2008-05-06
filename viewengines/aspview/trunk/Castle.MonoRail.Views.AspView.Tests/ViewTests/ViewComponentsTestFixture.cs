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

namespace Castle.MonoRail.Views.AspView.Tests.ViewTests
{
	using ViewComponents;
	using Views;
	using Xunit;

	
	public class ViewComponentsTestFixture : AbstractViewComponentsTestFixture
	{

		[Fact]
		public void WithRenderText_Works()
		{
			RegisterComponent("MyComponent", typeof(TextRendererViewComponent));

			InitializeView(typeof(WithComponent));

			view.Process();

			expected =
@"Parent
Default text from component's RenderText()
Parent";

			AssertViewOutputEqualsToExpected();
		}

		[Fact]
		public void WithRenderView_Works()
		{
			RegisterComponent("MyComponent", typeof(ViewRendererViewComponent));

			InitializeView(typeof(WithComponent));

			AddCompilation("\\components\\MyComponent\\SimpleView", typeof(SimpleView));

			view.Process();

			expected =
@"Parent
Simple
Parent";

			AssertViewOutputEqualsToExpected();
		}

		[Fact]
		public void WithRenderTextAndView_ViewIsAlwaysAfterTheText()
		{
			RegisterComponent("MyComponent", typeof(TextAndViewRendererViewComponent));

			InitializeView(typeof(WithComponent));

			AddCompilation("\\components\\MyComponent\\SimpleView", typeof(SimpleView));

			view.Process();

			expected =
@"Parent
Default text from component's RenderText()Simple
Parent";

			AssertViewOutputEqualsToExpected();
		}

		[Fact]
		public void WithBody_RendersTheBody()
		{
			RegisterComponent("MyComponent", typeof(BodyRendererViewComponent));

			InitializeView(typeof(WithComponentAndBody));

			view.Process();

			expected =
@"Parent
Component's Body
Parent";

			AssertViewOutputEqualsToExpected();
		}

		[Fact]
		public void WithBody_ButNoBody_Throws()
		{
			RegisterComponent("MyComponent", typeof(BodyRendererViewComponent));

			InitializeView(typeof(WithComponent));

			Assert.Throws<AspViewException>("This component does not have a body content to be rendered", delegate 
			{
				view.Process();
			});
			
		}

		[Fact]
		public void WithSections_RendersTheSections()
		{
			RegisterComponent("MyComponent", typeof(SectionsRendererViewComponent));

			InitializeView(typeof(WithComponentAndSections));

			view.Process();

			expected =
@"Parent
section1Textsection2
Parent";

			AssertViewOutputEqualsToExpected();
		}

		
		[Fact]
		public void WithViewComponent_ParameterNamesAreCaseInsentive() {

			// if component parameter's names where case sensitive 
			// WithMandatoryParameterViewComponent would throw because 'Text' parameter was not set

			#region test context var
			string text = "insensitive check pass";
			#endregion
			
			RegisterComponent("withmandatoryparameter", typeof(WithMandatoryParameterViewComponent));
		
			InitializeView(typeof(WithComponentWithParameter));
			
			view.Properties["text"] = text;
			
			view.Process();

			expected = text;
			AssertViewOutputEqualsToExpected();

		}
		/*
component with subview in view
component with subview in section
component with subview in body
nested components
		 * */
	}
}
