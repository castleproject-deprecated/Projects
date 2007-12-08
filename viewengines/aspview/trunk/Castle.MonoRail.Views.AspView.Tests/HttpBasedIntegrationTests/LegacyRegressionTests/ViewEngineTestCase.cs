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

using Castle.MonoRail.Views.AspView.Compiler;

namespace Castle.MonoRail.Views.AspView.Tests
{
	using System.Collections;
	using NUnit.Framework;

	[TestFixture]
	public class ViewEngineTestCase
	{
		AspViewEngine engine;
		IAspViewEngineTestAccess engineWithTestAccess;

		[SetUp]
		public void SetupFixture()
		{
			AspViewEngineOptions options = new AspViewEngineOptions(new AspViewCompilerOptions());
			engine = new AspViewEngine();
			engine.Initialize(options);
			engineWithTestAccess = engine;
		}

		[Test]
		public void ViewEngine_Always_LoadCompiledViewsAssembly()
		{
			IDictionary compilations = engineWithTestAccess.Compilations;
			Assert.IsNotNull(compilations);
			Assert.Greater(compilations.Count, 0);
		}

		[Test]
		public void HasTemplate_WhenAskedForFakeTemplate_ReturnsFalse()
		{
			string fakeTemplateName = "nosuchcontroller/nosuchtemplate";
			Assert.IsFalse(engine.HasTemplate(fakeTemplateName));
		}

		[Test]
		public void HasTemplate_WhenAskedForRealTemplate_ReturnsTrue()
		{
			string templateName = "home/index";
			Assert.IsTrue(engine.HasTemplate(templateName));
		}

		/*
		 * CreateView
		 * GetView
		 * GetLayout
		 * RcompileViews?
		 * GetClassName
		 * NormalizeClassName
		 * Process
		 * ProcessContents
		 * */
	}
}
