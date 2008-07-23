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

namespace Castle.Tools.CodeGenerator.Services
{
	using Model;
	using NUnit.Framework;
	using Rhino.Mocks;
	using External;

	[TestFixture]
	public class ControllerReferenceFactoryTests
	{
		private MockRepository mocks;
		private DefaultControllerReferenceFactory factory;
		private ICodeGeneratorServices services;

		[SetUp]
		public virtual void Setup()
		{
			mocks = new MockRepository();
			services = mocks.DynamicMock<ICodeGeneratorServices>();
			factory = new DefaultControllerReferenceFactory();
		}

		[Test]
		public void CreateActionReference_Always_CreatesValidReference()
		{
			var reference = (ControllerActionReference) factory.CreateActionReference(services, typeof (TestController), "Area", "Controller", "Action", null, new ActionArgument[0]);
			Assert.AreEqual("Controller", reference.ControllerName);
			Assert.AreEqual("Area", reference.AreaName);
			Assert.AreEqual("Action", reference.ActionName);
			Assert.AreEqual(typeof (TestController), reference.ControllerType);
			Assert.IsEmpty(reference.Arguments);
		}

		[Test]
		public void CreateViewReference_Always_CreatesValidReference()
		{
			var reference = (ControllerViewReference) factory.CreateViewReference(services, typeof (TestController), "Area", "Controller", "Action");
			Assert.AreEqual("Controller", reference.ControllerName);
			Assert.AreEqual("Area", reference.AreaName);
			Assert.AreEqual("Action", reference.ActionName);
			Assert.AreEqual(typeof (TestController), reference.ControllerType);
		}
	}
}