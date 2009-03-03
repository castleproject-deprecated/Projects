// Copyright 2006 Gokhan Altinoren - http://altinoren.com/
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

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using VSLangProj;

namespace Altinoren.ActiveWriter.CodeGeneration
{
    using System;
    using System.CodeDom;
    using System.IO;
    using EnvDTE;
    using Microsoft.CSharp;
    using Microsoft.VisualBasic;
    using Microsoft.VisualStudio.TextTemplating;
    using ServerExplorerSupport;

    public class CodeGenerationContext
    {
        public string AssemblyName { get; private set; }
        public CodeCompileUnit CompileUnit { get; private set; }
        public string DefaultNamespace { get; private set; }
        public DTE DTE { get; private set; }
        public CodeLanguage Language { get; private set; }
        public Model Model { get; private set; }
        public string ModelFileName { get; private set; }
        public string Namespace { get; private set; }
        internal OutputWindowHelper Output { get; private set; }
        public ProjectItem ProjectItem { get; private set; }
        public CodeDomProvider Provider { get; private set; }
        public ITextTemplatingEngineHost TextTemplatingHost { get; private set; }

        /// <summary>
        /// This is the output from the code generation that is written out by the ActiveWriterReport.tt template.
        /// </summary>
        public string PrimaryOutput { get; set; }

        public string ModelFilePath
        {
            get { return Path.GetDirectoryName(ModelFileName); }
        }

        public string ModelName
        {
            get { return Path.GetFileNameWithoutExtension(ModelFileName); }
        }

        public string HelperClassName
        {
            get { return ModelName + "Helper"; }
        }

        public string InternalPropertyAccessorName
        {
            get { return ModelName + "InternalPropertyAccessor"; }
        }

        public CodeGenerationContext(Model model, string nameSpace, string processID, string modelFileFullName, ITextTemplatingEngineHost textTemplatingHost)
        {
            CompileUnit = new CodeCompileUnit();

            Model = model;
            if (string.IsNullOrEmpty(Model.Namespace))
                Namespace = nameSpace;
            else
                Namespace = Model.Namespace;

            DTE = DTEHelper.GetDTE(processID);

            TextTemplatingHost = textTemplatingHost;

            ModelFileName = modelFileFullName;

            ProjectItem = DTE.Solution.FindProjectItem(ModelFileName);
            AssemblyName = DTEHelper.GetAssemblyName(ProjectItem.ContainingProject);

            Language = DTEHelper.GetProjectLanguage(ProjectItem.ContainingProject);
            switch (Language)
            {
                case CodeLanguage.CSharp:
                    Provider = new CSharpCodeProvider();
                    break;
                case CodeLanguage.VB:
                    Provider = new VBCodeProvider();

                    // use VB default namespace if it was set
                    VSProject project = (VSProject)ProjectItem.ContainingProject.Object;
                    Property DefaultNamespaceProperty = project.Project.Properties.Item("DefaultNamespace");

                    DefaultNamespace = (string)DefaultNamespaceProperty.Value;

                    break;
                default:
                    throw new ArgumentException(
                        "Unsupported project type. ActiveWriter currently supports C# and Visual Basic.NET projects.");
            }

            Output = new OutputWindowHelper(DTE);
        }
    }
}
