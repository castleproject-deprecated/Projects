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
	using MonoRail.Framework;
	using Model;
	using NUnit.Framework;
	using Rhino.Mocks;
	using External;

	[TestFixture]
	public class DefaultCodeGeneratorServicesTests
	{
		private MockRepository mocks;
		private DefaultCodeGeneratorServices services;
		private IControllerReferenceFactory controllerReferenceFactory;
		private IArgumentConversionService argumentConversionService;
		private IEngineContext context;
		private IRuntimeInformationService runtimeInformationService;
		private TestController controller;

		[SetUp]
		public void Setup()
		{
			controller = new TestController();
			mocks = new MockRepository();
			controllerReferenceFactory = mocks.DynamicMock<IControllerReferenceFactory>();
			argumentConversionService = mocks.DynamicMock<IArgumentConversionService>();
			runtimeInformationService = mocks.DynamicMock<IRuntimeInformationService>();
			services = new DefaultCodeGeneratorServices(controllerReferenceFactory, argumentConversionService, runtimeInformationService);
			context = mocks.DynamicMock<IEngineContext>();

			Assert.AreEqual(controllerReferenceFactory, services.ControllerReferenceFactory);
			Assert.AreEqual(argumentConversionService, services.ArgumentConversionService);
			Assert.AreEqual(runtimeInformationService, services.RuntimeInformationService);
		}

		[Test]
		public void GetAndSetController_Always_Work()
		{
			Assert.IsNull(services.Controller);
			services.Controller = controller;
			Assert.AreEqual(controller, services.Controller);
		}

		[Test]
		public void GetAndSetRailsContext_Always_Works()
		{
			Assert.IsNull(services.RailsContext);
			services.RailsContext = context;
			Assert.AreEqual(context, services.RailsContext);
		}
	}
}