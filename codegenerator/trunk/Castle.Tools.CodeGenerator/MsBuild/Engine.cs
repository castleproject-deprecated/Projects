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

namespace Castle.Tools.CodeGenerator.MsBuild
{
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.IO;
	using Services;
	using Services.Generators;

	public class Generator
	{
		private readonly List<IGenerator> generators = new List<IGenerator>();
		private readonly string nameSpace;
		private readonly string outputFile;
		private readonly string serviceTypeName;
		private readonly ILogger logger;
		private readonly INamingService namingService;
		private readonly ISourceGenerator sourceGenerator;
		private readonly ITreeCreationService treeCreationService;

		public Generator(string nameSpace, string outputFile, string serviceTypeName, ILogger logger,
		                 INamingService namingService, ISourceGenerator sourceGenerator,
		                 ITreeCreationService treeCreationService)
		{
			this.nameSpace = nameSpace;
			this.outputFile = outputFile;
			this.serviceTypeName = serviceTypeName;
			this.logger = logger;
			this.namingService = namingService;
			this.sourceGenerator = sourceGenerator;
			this.treeCreationService = treeCreationService;
		}

		public virtual void Execute()
		{
			CreateDefaultGenerators();

			foreach (var generator in generators)
				generator.Generate(treeCreationService.Root);

			CreateGeneratedItems();
		}

		protected virtual void CreateDefaultGenerators()
		{
			if (generators.Count > 0) return;

			generators.Add(new ActionMapGenerator(logger, sourceGenerator, namingService, nameSpace, serviceTypeName));
			generators.Add(new RouteMapGenerator(logger, sourceGenerator, namingService, nameSpace, serviceTypeName));
			generators.Add(new ViewMapGenerator(logger, sourceGenerator, namingService, nameSpace, serviceTypeName));
			generators.Add(new ControllerPartialsGenerator(logger, sourceGenerator, namingService, nameSpace, serviceTypeName));
			generators.Add(new ControllerMapGenerator(logger, sourceGenerator, namingService, nameSpace, serviceTypeName));
			generators.Add(new WizardStepMapGenerator(logger, sourceGenerator, namingService, nameSpace, serviceTypeName));
		}

		protected virtual void CreateGeneratedItems()
		{
			using (var writer = File.CreateText(outputFile))
			{
				CodeGenerator.ValidateIdentifiers(sourceGenerator.Ccu);
				CodeDomProvider.CreateProvider("C#").GenerateCodeFromCompileUnit(sourceGenerator.Ccu, writer, new CodeGeneratorOptions());
			}
		}
	}
}