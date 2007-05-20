// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.AspView
{
	using System;
	using System.Text;
	using System.IO;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.CodeDom.Compiler;
	using Microsoft.CSharp;
	using Microsoft.VisualBasic;
	using Castle.MonoRail.Framework;

	public class AspViewCompiler
	{
		#region members
		private CompilerParameters parameters;
		private AspViewCompilerOptions options;
		private AspViewPreProcessor preProcessor;
		private static readonly ReferencedAssembly[] defaultAddedReferences = null;
		private static readonly string defaultSiteRoot = AppDomain.CurrentDomain.BaseDirectory;
		private static readonly string defaultBinDirectory = Path.Combine(defaultSiteRoot, "bin");
		private string targetDirectory = null;
		private string binDirectory = null;

		public string PathToAssembly = null;

		#endregion

		public AspViewCompiler(AspViewCompilerOptions options)
		{
			this.options = options;
			preProcessor = new AspViewPreProcessor();
			InitializeCompilerParameters();
		}

		public void CompileSite()
		{
			CompileSite(defaultSiteRoot);
		}
		public void CompileSite(string siteRoot)
		{
			binDirectory = Path.Combine(siteRoot, "bin");
			targetDirectory = binDirectory;
			CompileSite(siteRoot, defaultAddedReferences);
		}

		public void CompileSite(string siteRoot, ReferencedAssembly[] references)
		{
			List<AspViewFile> files = GetViewFiles(siteRoot);
			if (files.Count == 0)
				return;


			preProcessor.Process(files);

			List<AspViewFile> vbFiles = files.FindAll(delegate(AspViewFile file) { return (file.Language == ScriptingLanguage.VbNet); });
			List<AspViewFile> csFiles = files.FindAll(delegate(AspViewFile file) { return (file.Language == ScriptingLanguage.CSharp); });


			if (options.KeepTemporarySourceFiles)
			{
				string targetTemporarySourceFilesDirectory = options.TemporarySourceFilesDirectory;
				if (!Path.IsPathRooted(targetTemporarySourceFilesDirectory))
					targetTemporarySourceFilesDirectory = Path.Combine(targetDirectory, targetTemporarySourceFilesDirectory);
				if (!Directory.Exists(targetTemporarySourceFilesDirectory))
					Directory.CreateDirectory(targetTemporarySourceFilesDirectory);
				foreach (AspViewFile file in files)
					SaveFile(file, targetTemporarySourceFilesDirectory);
			}

			PathToAssembly = Compile(targetDirectory, csFiles, vbFiles, references);

		}

		private string Compile(string targetDirectory, List<AspViewFile> csFiles, List<AspViewFile> vbFiles, ReferencedAssembly[] references)
		{
			if (vbFiles.Count == 0)
				return CompileCSharpModule(targetDirectory, csFiles, references, true, null);
			if (csFiles.Count == 0)
				return CompileVbModule(targetDirectory, vbFiles, references, true, null);

			string vbModulePath = CompileVbModule(targetDirectory, vbFiles, references, false, null);
			return CompileCSharpModule(targetDirectory, csFiles, references, true, new string[1] { vbModulePath });
		}

		private List<AspViewFile> GetViewFiles(string siteRoot)
		{
			List<AspViewFile> files = new List<AspViewFile>();
			string viewsDirectory = Path.Combine(siteRoot, "Views");

			string[] fileNames = Directory.GetFiles(viewsDirectory, "*.aspx", SearchOption.AllDirectories);
			foreach (string fileName in fileNames)
			{
				AspViewFile file = new AspViewFile();
				file.ViewName = fileName.Replace(viewsDirectory, "");
				file.ClassName = AspViewEngine.GetClassName(file.ViewName);
				file.ViewSource = ReadFile(fileName);
				files.Add(file);
			}
			return files;
		}
		private static string ReadFile(string fileName)
		{
			string source = string.Empty;
			using (StreamReader sr = new StreamReader(fileName))
			{
				source = sr.ReadToEnd();
			}
			return source;
		}
		private static void SaveFile(AspViewFile file, string targetDirectory)
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

		private string CompileModule(
			CodeDomProvider codeProvider, string targetDirectory, List<AspViewFile> files,
			ReferencedAssembly[] references, bool createAssembly, string[] modulesToAdd)
		{
			if (!createAssembly)
			{
				parameters.CompilerOptions = "/t:module";
				parameters.OutputAssembly = Path.Combine(targetDirectory,
					string.Format("CompiledViews.{0}.netmodule", codeProvider.FileExtension));
			}
			else
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

			if (modulesToAdd != null && modulesToAdd.Length > 0)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(" /addmodule: ");
				foreach (string moduleToAdd in modulesToAdd)
					sb.Append(Path.Combine(targetDirectory, moduleToAdd));
				parameters.CompilerOptions += "\"" + sb.ToString() + "\"";
			}

			CompilerResults results;
			if (options.KeepTemporarySourceFiles)
			{
				string targetTemporarySourceFilesDirectory = Path.Combine(targetDirectory, options.TemporarySourceFilesDirectory);
				List<string> fileNames = new List<string>(files.Count);
				foreach (AspViewFile file in files)
					fileNames.Add(Path.Combine(targetTemporarySourceFilesDirectory, file.FileName));
				results = codeProvider.CompileAssemblyFromFile(parameters, fileNames.ToArray());
			}
			else
			{
				List<string> sources = new List<string>(files.Count);
				foreach (AspViewFile file in files)
					sources.Add(file.ConcreteClass);
				results = codeProvider.CompileAssemblyFromSource(parameters, sources.ToArray());
			}
			if (results.Errors.Count > 0)
			{
				StringBuilder message = new StringBuilder();
				foreach (CompilerError err in results.Errors)
					message.AppendLine(err.ToString());
				throw new Exception(string.Format(
					"Error while compiling'':\r\n{0}",
					message.ToString()));
			}
			return results.PathToAssembly;
		}
		private string CompileCSharpModule(string targetDirectory, List<AspViewFile> files, ReferencedAssembly[] references, bool createAssembly, string[] modulesToAdd)
		{
			return CompileModule(new CSharpCodeProvider(), targetDirectory, files, references, createAssembly, modulesToAdd);
		}
		private string CompileVbModule(string targetDirectory, List<AspViewFile> files, ReferencedAssembly[] references, bool createAssembly, string[] modulesToAdd)
		{
			return CompileModule(new VBCodeProvider(), targetDirectory, files, references, createAssembly, modulesToAdd);
		}
	}
}
