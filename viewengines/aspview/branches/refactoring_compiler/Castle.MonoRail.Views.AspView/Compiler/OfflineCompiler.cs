namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System.CodeDom.Compiler;
	using System.Collections.Generic;

	using Adapters;
	using Factories;

	public class OfflineCompiler : AbstractCompiler
	{
		const string AllowPartiallyTrustedCallersFileName = "AllowPartiallyTrustedCallers.generated.cs";
		readonly IFileSystemAdapter fileSystem;

		public OfflineCompiler(ICodeProviderAdapterFactory codeProviderAdapterFactory, IPreProcessor preProcessor, ICompilationContext context, AspViewCompilerOptions options, IFileSystemAdapter fileSystem)
			: base(codeProviderAdapterFactory, preProcessor, context, options)
		{
			this.fileSystem = fileSystem;
			parameters.GenerateInMemory = false;
		}

		public string Execute()
		{
			return InternalExecute().PathToAssembly;
		}

		protected override void AfterPreCompilation(List<SourceFile> files)
		{
			if (options.KeepTemporarySourceFiles)
				KeepTemporarySourceFiles(files);
		}

		protected override CompilerResults GetResultsFrom(List<SourceFile> files)
		{
			string[] sources = files.ConvertAll<string>(SourceFileToFileName).ToArray();

			return codeProvider.CompileAssemblyFromFile(parameters, sources);
		}

		static string SourceFileToFileName(SourceFile file)
		{
			return file.FileName;
		}

		void KeepTemporarySourceFiles(List<SourceFile> files)
		{
				SetupTemporarySourceFilesDirectory();

				foreach (SourceFile file in files)
					fileSystem.Save(file.FileName, file.ConcreteClass, context.TemporarySourceFilesDirectory);

				if (options.AllowPartiallyTrustedCallers)
				{
					fileSystem.Save(
						AllowPartiallyTrustedCallersFileName,
						GetAllowPartiallyTrustedCallersFileContent(),
						context.TemporarySourceFilesDirectory);
				}
		}
		void SetupTemporarySourceFilesDirectory()
		{
			if (fileSystem.Exists(context.TemporarySourceFilesDirectory) == false)
			{
				fileSystem.Create(context.TemporarySourceFilesDirectory);
				return;
			}

			fileSystem.ClearSourceFilesFrom(context.TemporarySourceFilesDirectory);
		}

	}
}
