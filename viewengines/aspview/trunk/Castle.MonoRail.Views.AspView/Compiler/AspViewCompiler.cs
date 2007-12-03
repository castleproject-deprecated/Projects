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

using Castle.MonoRail.Views.AspView.Compiler.PreCompilationSteps;

namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System;
	using System.Text;
	using System.IO;
	using System.Collections.Generic;
	using System.CodeDom.Compiler;
	using Microsoft.CSharp;
	using System.Reflection;

	public class AspViewCompiler
	{
		const string AssemblyAttributeAllowPartiallyTrustedCallers = "[assembly: System.Security.AllowPartiallyTrustedCallers]";

		readonly IEnumerable<IPreCompilationStep> preCompilationSteps;

		#region members
		private CompilerParameters parameters;
		readonly AspViewCompilerOptions options;
		private static readonly ReferencedAssembly[] defaultAddedReferences = null;
		private static readonly string defaultSiteRoot = AppDomain.CurrentDomain.BaseDirectory;

		public string PathToAssembly = null;
		public Assembly Assembly = null;

		#endregion

		public AspViewCompiler(IPreCompilationStepsProvider preCompilationStepsProvider, AspViewCompilerOptions options)
		{
			preCompilationSteps = preCompilationStepsProvider.GetSteps();
			this.options = options;
			InitializeCompilerParameters();
		}
		public AspViewCompiler(AspViewCompilerOptions options) : this(new DefaultPreCompilationStepsProvider(), options)
		{
		}

		public void CompileSite()
		{
			CompileSite(defaultSiteRoot);
		}

		public void CompileSite(string siteRoot)
		{
			CompileSite(siteRoot, defaultAddedReferences);
		}

		public void ApplyPreCompilationStepsOn(IEnumerable<SourceFile> files)
		{
			foreach (SourceFile file in files)
				foreach (IPreCompilationStep step in preCompilationSteps)
					step.Process(file);
		}

		public void CompileSite(string siteRoot, ReferencedAssembly[] references)
		{
			List<SourceFile> files = GetSourceFiles(siteRoot ?? defaultSiteRoot);
			if (files.Count == 0)
				return;

			ApplyPreCompilationStepsOn(files);

			string targetDirectory = Path.Combine(siteRoot, "Bin");

			if (options.KeepTemporarySourceFiles)
			{
				string targetTemporarySourceFilesDirectory = GetTargetTemporarySourceFilesDirectory(targetDirectory);
				foreach (SourceFile file in files)
					SaveFile(file, targetTemporarySourceFilesDirectory);
			}

			CompilerResults results = Compile(files, references, targetDirectory);

			if (options.InMemory)
				Assembly = results.CompiledAssembly;
			else 
				PathToAssembly = results.PathToAssembly;

		}

		private string GetTargetTemporarySourceFilesDirectory(string targetDirectory)
		{
			string targetTemporarySourceFilesDirectory = options.TemporarySourceFilesDirectory;
			if (!Path.IsPathRooted(targetTemporarySourceFilesDirectory))
				targetTemporarySourceFilesDirectory = Path.Combine(targetDirectory, targetTemporarySourceFilesDirectory);
			if (!Directory.Exists(targetTemporarySourceFilesDirectory))
				Directory.CreateDirectory(targetTemporarySourceFilesDirectory);
			return targetTemporarySourceFilesDirectory;
		}

		private CompilerResults Compile(ICollection<SourceFile> files, IEnumerable<ReferencedAssembly> references, string targetDirectory)
		{
			if (!parameters.GenerateInMemory)
				parameters.OutputAssembly = Path.Combine(targetDirectory, "CompiledViews.dll");
			List<ReferencedAssembly> actualReferences = new List<ReferencedAssembly>();
			if (options.References != null)
				actualReferences.AddRange(options.References);
			if (references != null)
				actualReferences.AddRange(references);

			foreach (ReferencedAssembly reference in actualReferences)
			{
				string assemblyName = reference.Name;
				if (reference.Source == ReferencedAssembly.AssemblySource.BinDirectory)
					assemblyName = Path.Combine(targetDirectory, assemblyName);
				parameters.CompilerOptions += " /r:\"" + assemblyName + "\"";
			}

			List<string> sources = new List<string>(files.Count);
			foreach (SourceFile file in files)
				sources.Add(file.ConcreteClass);

			if (options.AllowPartiallyTrustedCallers)
			{
				sources.Add(AssemblyAttributeAllowPartiallyTrustedCallers);
			}

			CompilerResults results = new CSharpCodeProvider().CompileAssemblyFromSource(parameters, sources.ToArray());

			if (results.Errors.Count > 0)
			{
				StringBuilder message = new StringBuilder();
				using (CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider())
				{
					foreach (SourceFile file in files)
					{
						CompilerResults result = cSharpCodeProvider.CompileAssemblyFromSource(parameters, file.ConcreteClass);
						if (result.Errors.Count > 0)
							foreach (CompilerError err in result.Errors)
								message.AppendLine(string.Format(@"
On '{0}' (class name: {1}) Line {2}, Column {3}, {4} {5}:
{6}
========================================",
								file.ViewName, 
								file.ClassName, 
								err.Line, 
								err.Column,
								err.IsWarning ? "Warning" : "Error",
								err.ErrorNumber, 
								err.ErrorText));
					}
				}
				throw new Exception("Error while compiling views: " + message);
			}
			return results;
		}

		public static List<SourceFile> GetSourceFiles(string siteRoot)
		{
			List<SourceFile> files = new List<SourceFile>();
			string viewsDirectory = Path.Combine(siteRoot, "Views");
			
			if (!Directory.Exists(viewsDirectory))
				throw new Exception(string.Format("Could not find views folder [{0}]", viewsDirectory));

			string[] fileNames = Directory.GetFiles(viewsDirectory, "*.aspx", SearchOption.AllDirectories);
			foreach (string fileName in fileNames)
			{
				SourceFile file = new SourceFile();
				file.ViewName = fileName.Replace(viewsDirectory, "");
				file.ClassName = AspViewEngine.GetClassName(file.ViewName);
				file.ViewSource = ReadFile(fileName);
				files.Add(file);
			}
			return files;
		}

		private static string ReadFile(string fileName)
		{
			return File.ReadAllText(fileName);
		}

		private static void SaveFile(SourceFile file, string targetDirectory)
		{
			string fileName = Path.Combine(targetDirectory, file.FileName);
			using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8))
			{
				sw.Write(file.ConcreteClass);
				sw.Flush();
			}
		}

		private void InitializeCompilerParameters()
		{
			parameters = new CompilerParameters();
			parameters.GenerateInMemory = options.InMemory;
			parameters.GenerateExecutable = false;
			parameters.IncludeDebugInformation = options.Debug;
		}

	}
}
