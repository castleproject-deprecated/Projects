// Copyright 2006 Gokhan Castle - http://altinoren.com/
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

namespace Castle.ActiveWriter.CodeGeneration
{
    using System.Collections.Generic;
    using System;
    using System.CodeDom;
    using System.IO;
    using Castle.ActiveWriter.ServerExplorerSupport;
    using EnvDTE;
    using EnvDTE80;
    using CodeNamespace = System.CodeDom.CodeNamespace;
    using System.Collections;

    public class MonoRailGenerator
    {
        private CodeGenerationContext Context { get; set; }
        
        public MonoRailGenerator(CodeGenerationContext context)
        {
            Context = context;
        }
        
        public void Generate()
        {
            if (Context.Model.GenerateMonoRailProject && !String.IsNullOrEmpty(Context.Model.MonoRailProjectName) && !String.IsNullOrEmpty(Context.Model.MonoRailProjectPath))
            {
                if (Context.DTE == null)
                    throw new NullReferenceException("Could not get a reference to active DTE object.");

                Project project = null;
                project = GetProject(Context.DTE, Context.Model.MonoRailProjectName);

                if (project == null)
                {
                    project =
                        CreateProject(Context.DTE, Context.Model.MonoRailProjectPath + Path.DirectorySeparatorChar + Context.Model.MonoRailProjectName,
                                      Context.Model.MonoRailProjectName);
                }

                // We will handle the first namespace by default.
                if (Context.CompileUnit.Namespaces.Count > 0)
                {
                    CodeNamespace ns = Context.CompileUnit.Namespaces[0];
                    List<CodeTypeDeclaration> classes = null;
                    if (ns.Types.Count > 0)
                    {
                        classes = new List<CodeTypeDeclaration>();
                        foreach (CodeTypeDeclaration type in ns.Types)
                        {
                            if (type.IsClass)
                            {
                                foreach (CodeAttributeDeclaration attribute in type.CustomAttributes)
                                {
                                    if (attribute.Name == "ActiveRecord")
                                    {
                                        classes.Add(type);
                                        break;
                                    }
                                }
                            }
                        }

                        if (classes.Count > 0)
                        {
                            // TODO: ...
                        }
                    }
                }
            }
        }

        private Project CreateProject(DTE dte, string fileName, string projectName)
        {
            string templatePath = GetTemplatePath(dte);

            Project project = ((Solution2)((DTE2)dte).Solution).AddFromTemplate(templatePath, fileName, projectName, false);
            AddFolder(project, Common.ModelsFolderName);
            AddFolder(project, Common.ViewsFolderName);
            AddFolder(project, Common.ControllersFolderName);

            return project;
        }

        private string GetTemplatePath(DTE dte)
        {
            if (Context.Language == CodeLanguage.CSharp)
                return 
                    ((Solution2) ((DTE2) dte).Solution).GetProjectTemplate(Common.BlankProjectTemplateName,
                                                                           Common.DTEProjectTemplateLanguageCSharp);
            else
                return
                    ((Solution2)((DTE2)dte).Solution).GetProjectTemplate(Common.BlankProjectTemplateName,
                                                                         Common.DTEProjectTemplateLanguageVB);
        }

        private Project GetProject(DTE dte, string name)
        {
            foreach (Project _project in dte.Solution.Projects)
            {
                if (_project.Name == name)
                {
                    bool hasModels = false;
                    bool hasViews = false;
                    bool hasControllers = false;

                    foreach (ProjectItem item in _project.ProjectItems)
                    {
                        if (item.Kind == Constants.vsProjectItemKindPhysicalFolder)
                        {
                            if (item.Name == Common.ModelsFolderName)
                                hasModels = true;
                            else if (item.Name == Common.ViewsFolderName)
                                hasViews = true;
                            else if (item.Name == Common.ControllersFolderName)
                                hasControllers = true;
                        }
                    }

                    if (!hasModels)
                        AddFolder(_project, Common.ModelsFolderName);
                    if (!hasViews)
                        AddFolder(_project, Common.ViewsFolderName);
                    if (!hasControllers)
                        AddFolder(_project, Common.ControllersFolderName);
                    
                    return _project;
                }
            }

            return null;
        }

        private void AddFolder(Project project, string name)
        {
            project.ProjectItems.AddFolder(name, Constants.vsProjectItemKindPhysicalFolder);
        }
    }
}
