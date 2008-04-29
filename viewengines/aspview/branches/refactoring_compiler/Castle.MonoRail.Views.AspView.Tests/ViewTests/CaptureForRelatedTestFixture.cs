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

using Castle.MonoRail.Framework.ViewComponents;

namespace Castle.MonoRail.Views.AspView.Tests.ViewTests
{
	using Views;
	using Xunit;

	
	public class CaptureForRelatedTestFixture : AbstractViewComponentsTestFixture
	{
		[Fact]
		public void CaptureForInView_VisibleInLayout()
		{
			RegisterComponent("CaptureFor", typeof(CaptureFor));
			InitializeView(typeof(WithCaptureFor));
			SetLayout(typeof(LayoutUsesCaptureFor));
			view.Process();

			expected = @"View: Parent

Parent
From CaptureFor: The captured content";

			AssertViewOutputEqualsToExpected();
		}

		[Fact]
		public void CaptureForInSubView_VisibleInLayout()
		{
			RegisterComponent("CaptureFor", typeof(CaptureFor));
			AddCompilation("subview", typeof(WithCaptureFor));
			InitializeView(typeof(WithSubView));
			SetLayout(typeof(LayoutUsesCaptureFor));
			view.Process();

			expected = @"View: Parent
Parent

Parent
Parent
From CaptureFor: The captured content";

			AssertViewOutputEqualsToExpected();
		}

		[Fact]
		public void CaptureForInSubView_VisibleInView()
		{
			RegisterComponent("CaptureFor", typeof(CaptureFor));
			AddCompilation("subview", typeof(WithCaptureFor));
			InitializeView(typeof(UsingBubbledCaptureFromSubView));
			view.Process();

			expected = @"Parnt View
From subview: Parent

Parent
From CaptureFor: The captured content";

			AssertViewOutputEqualsToExpected();
		}
	}
}
