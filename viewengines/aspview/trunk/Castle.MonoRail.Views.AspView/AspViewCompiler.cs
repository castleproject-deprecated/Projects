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
        #endregion

        public AspViewCompiler(AspViewCompilerOptions options)
        {
            this.options = options;
            preProcessor = new AspViewPreProcessor();
            InitializeCompilerParameters();
        }

        public void CompileSite(string siteRoot, string[] references)
        {
            List<AspViewFile> files = GetViewFiles(siteRoot);
            if (files.Count == 0)
                return;

            string targetDirectory = Path.Combine(siteRoot, "bin");

            preProcessor.Process(files);

            List<AspViewFile> vbFiles = files.FindAll(delegate(AspViewFile file) { return (file.Language == ScriptingLanguage.VbNet); });
            List<AspViewFile> csFiles = files.FindAll(delegate(AspViewFile file) { return (file.Language == ScriptingLanguage.CSharp); });

            if (vbFiles.Count == 0)
                CompileCSharpModule(targetDirectory, csFiles, references, true, null);
            else
            {
                if (csFiles.Count == 0)
                    CompileVbModule(targetDirectory, vbFiles, references, true, null);
                else
                {
                    string vbModulePath = CompileVbModule(targetDirectory, vbFiles, references, false, null);
                    CompileCSharpModule(targetDirectory, csFiles, references, true, new string[1] { vbModulePath });
                }
            }
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

        private void InitializeCompilerParameters()
        {
            parameters = new CompilerParameters();
            parameters.GenerateInMemory = options.InMemory;
            parameters.GenerateExecutable = false;
            parameters.IncludeDebugInformation = options.Debug;
        }

        public string CompileModule(
            CodeDomProvider codeProvider, string targetDirectory, List<AspViewFile> files,
            string[] references, bool createAssembly, string[] modulesToAdd)
        {
            if (!createAssembly)
            {
                parameters.CompilerOptions = "/t:module";
                parameters.OutputAssembly = Path.Combine(targetDirectory,
                    string.Format("CompiledViews.{0}.netmodule", codeProvider.FileExtension));
            }
            else
                parameters.OutputAssembly = Path.Combine(targetDirectory, "CompiledViews.dll");

            if (references != null)
                foreach (string reference in references)
                    parameters.CompilerOptions += " /r:" + reference;


            if (modulesToAdd != null && modulesToAdd.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" /addmodule: ");
                foreach (string moduleToAdd in modulesToAdd)
                    sb.Append(Path.Combine(targetDirectory, moduleToAdd));
                parameters.CompilerOptions += sb.ToString();
            }

            List<string> sources = new List<string>(files.Count);
            foreach (AspViewFile file in files)
                sources.Add(file.ConcreteClass);
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, sources.ToArray());
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
        public string CompileCSharpModule(string targetDirectory, List<AspViewFile> files, string[] references, bool createAssembly, string[] modulesToAdd)
        {
            return CompileModule(new CSharpCodeProvider(), targetDirectory, files, references, createAssembly, modulesToAdd);
        }
        public string CompileVbModule(string targetDirectory, List<AspViewFile> files, string[] references, bool createAssembly, string[] modulesToAdd)
        {
            return CompileModule(new VBCodeProvider(), targetDirectory, files, references, createAssembly, modulesToAdd);
        }
    }
}
