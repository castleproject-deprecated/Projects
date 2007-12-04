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
	using Stubs;
	using System;
	using System.Collections.Generic;
	using ViewComponents;
	using Framework;
	using Framework.Services;
	using Views;
	using NUnit.Framework;

	[TestFixture]
	public class ViewComponentsTestFixture : AbstractViewTestFixture
	{

		MyViewComponentFactory componentFactory = new MyViewComponentFactory();

		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
		[ExpectedException(typeof(RailsException), ExpectedMessage = "This component does not have a body content to be rendered")]
		public void WithBody_ButNoBody_Throws()
		{
			RegisterComponent("MyComponent", typeof(BodyRendererViewComponent));

			InitializeView(typeof(WithComponent));

			view.Process();
		}

		[Test]
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

		/*
component with subview in view
component with subview in section
component with subview in body
nested components
		 * */
		protected override void Clear()
		{
			base.Clear();
			componentFactory = null;
		}

		protected override void CreateStubsAndMocks()
		{
			base.CreateStubsAndMocks();
			componentFactory = new MyViewComponentFactory();
		}

		protected override void CreateDefaultStubsAndMocks()
		{
			base.CreateDefaultStubsAndMocks();
			context.AddService(typeof(IViewComponentFactory), componentFactory);
		}

		private void RegisterComponent(string name, Type type)
		{
			componentFactory.RegisteredComponents.Add(name, type);
		}
		class MyViewComponentFactory : DefaultViewComponentFactory
		{
			public readonly Dictionary<string, Type> RegisteredComponents =
				new Dictionary<string, Type>();

			public override ViewComponent Create(string name)
			{
				return Activator.CreateInstance(RegisteredComponents[name]) as ViewComponent;
			}
		}
	}
}
