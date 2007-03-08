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

namespace Altinoren.ActiveWriter.CodeGeneration
{
    using System.Collections.Generic;
    using System;
    using System.CodeDom;
    using System.IO;
    using Altinoren.ActiveWriter.ServerExplorerSupport;
    using EnvDTE;
    using EnvDTE80;
    using CodeNamespace = System.CodeDom.CodeNamespace;
    using System.Collections;

    public class MonoRailGenerator
    {
        private Hashtable _propertyBag;
        private Model _model;
        private DTE _dte;
        private CodeLanguage _language;
        
        public MonoRailGenerator(Hashtable propertyBag)
        {
            _propertyBag = propertyBag;
        }
        
        public void Generate()
        {
            _model = (Model) _propertyBag["Generic.Model"];
            
            if (_model.GenerateMonoRailProject && !String.IsNullOrEmpty(_model.MonoRailProjectName) && !String.IsNullOrEmpty(_model.MonoRailProjectPath))
            {
                _dte = (DTE)_propertyBag["Generic.DTE"];
                if (_dte == null)
                {
                    throw new NullReferenceException("Could not get a reference to active DTE object.");
                }
                else
                {
                    _language = (CodeLanguage)_propertyBag["Generic.Language"];

                    Project project = null;
                    project = GetProject(_dte, _model.MonoRailProjectName);

                    if (project == null)
                    {
                        project =
                            CreateProject(_dte, _model.MonoRailProjectPath + Path.DirectorySeparatorChar + _model.MonoRailProjectName,
                                          _model.MonoRailProjectName);
                    }

                    CodeCompileUnit compileUnit = (CodeCompileUnit)_propertyBag["CodeGeneration.CodeCompileUnit"];
                    
                    // We will handle the first namespace by default.
                    if (compileUnit.Namespaces.Count > 0)
                    {
                        CodeNamespace ns = compileUnit.Namespaces[0];
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
            if (_language == CodeLanguage.CSharp)
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
