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

namespace Castle.Tools.CodeGenerator.External
{
	using MonoRail.Framework;
	using External;

	public class DefaultCodeGeneratorServices : ICodeGeneratorServices
	{
		public DefaultCodeGeneratorServices(IControllerReferenceFactory controllerReferenceFactory,
		                                    IArgumentConversionService argumentConversionService,
		                                    IRuntimeInformationService runtimeInformationService)
		{
			ControllerReferenceFactory = controllerReferenceFactory;
			ArgumentConversionService = argumentConversionService;
			RuntimeInformationService = runtimeInformationService;
		}

		public Controller Controller { get; set; }
		public IControllerReferenceFactory ControllerReferenceFactory { get; private set; }
		public IEngineContext RailsContext { get; set; }
		public IArgumentConversionService ArgumentConversionService { get; private set; }
		public IRuntimeInformationService RuntimeInformationService { get; private set; }
	}
}