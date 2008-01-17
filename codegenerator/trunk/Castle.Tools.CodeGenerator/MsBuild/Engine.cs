using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Castle.Tools.CodeGenerator.Services;
using Castle.Tools.CodeGenerator.Services.Generators;

namespace Castle.Tools.CodeGenerator.MsBuild
{
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

		public Generator(string nameSpace, string outputFile, string serviceTypeName, ILogger logger, INamingService namingService, ISourceGenerator sourceGenerator, ITreeCreationService treeCreationService)
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

			foreach (IGenerator generator in generators)
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
			using (StreamWriter writer = File.CreateText(outputFile))
			{
				System.CodeDom.Compiler.CodeGenerator.ValidateIdentifiers(sourceGenerator.Ccu);
				CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
				CodeGeneratorOptions options = new CodeGeneratorOptions();
				provider.GenerateCodeFromCompileUnit(sourceGenerator.Ccu, writer, options);
			}
		}
	}
}