// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Tools.CodeGenerator.Model
{
	using System;
	using NUnit.Framework;
	using External;

	[TestFixture]
	public class ControllerViewReferenceTests : ControllerReferenceTests
	{
		[SetUp]
		public override void Setup()
		{
			base.Setup();
		}

		[Test]
		public void Constructor_Always_ProperlyInitializes()
		{
			var reference = new ControllerViewReference(services, typeof (TestController), "Area", "Test", "Action");
			
			Assert.AreEqual("Action", reference.ActionName);
			Assert.AreEqual("Area", reference.AreaName);
			Assert.AreEqual("Test", reference.ControllerName);
			Assert.AreEqual(typeof (TestController), reference.ControllerType);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Constructor_NoServices_Throws()
		{
			new ControllerViewReference(null, typeof (TestController), "Area", "Test", "Action");
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Constructor_NoType_Throws()
		{
			new ControllerViewReference(services, null, "Area", "Test", "Action");
		}

		[Test]
		public void Render_Always_SetsSelectedView()
		{
			var reference = new ControllerViewReference(services, typeof (TestController), "Area", "Test", "Action");

			reference.Render();
			
			Assert.AreEqual(@"Area\Test\Action", controller.SelectedViewName);
		}

		[Test]
		public void Render_NoArea_SetsSelectedView()
		{
			var reference = new ControllerViewReference(services, typeof (TestController), "", "Test", "Action");

			reference.Render();
			
			Assert.AreEqual(@"Test\Action", controller.SelectedViewName);
		}
	}
}