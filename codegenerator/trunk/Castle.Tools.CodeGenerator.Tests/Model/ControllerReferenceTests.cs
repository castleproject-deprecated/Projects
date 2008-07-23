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
	using System.Collections;
	using System.IO;
	using MonoRail.Framework;
	using MonoRail.Framework.Test;
	using External;
	using NUnit.Framework;
	using Rhino.Mocks;

	public class ControllerReferenceTests
	{
		protected IArgumentConversionService argumentConversionService;
		protected TestController controller;
		protected IEngineContext railsContext;
		protected ControllerActionReference reference;
		protected StubResponse response;
		protected IServerUtility serverUtility;
		protected ICodeGeneratorServices services;
		protected string virtualDirectory = String.Empty;
		protected IDictionary parameters;

		[SetUp]
		public virtual void Setup()
		{
			response = new StubResponse();
			var url = new UrlInfo("eleutian.com", "www", virtualDirectory, "http", 80, 
				Path.Combine(Path.Combine("Area", "Controller"), "Action"), "Area", "Controller", "Action", "rails", "");
			
			var stubEngineContext = new StubEngineContext(new StubRequest(), response, new StubMonoRailServices(), url)
			{
				Server = MockRepository.GenerateMock<IServerUtility>()
			};

			railsContext = stubEngineContext;
			serverUtility = railsContext.Server;

			argumentConversionService = MockRepository.GenerateMock<IArgumentConversionService>();

			controller = new TestController();
			controller.Contextualize(railsContext, MockRepository.GenerateStub<IControllerContext>());
			
			parameters = new Hashtable();
			
			services = MockRepository.GenerateMock<ICodeGeneratorServices>();
			services.Expect(s => s.ArgumentConversionService).Return(argumentConversionService).Repeat.Any();
			services.Expect(s => s.Controller).Return(controller).Repeat.Any();
			services.Expect(s => s.RailsContext).Return(railsContext).Repeat.Any();
			argumentConversionService.Expect(s => s.CreateParameters()).Return(parameters).Repeat.Any();
		}

		[TearDown]
		public virtual void Teardown()
		{
			virtualDirectory = String.Empty;
		}
	}
}