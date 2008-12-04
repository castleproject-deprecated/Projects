
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Altinoren.ActiveWriter.ServerExplorerSupport;
using EnvDTE;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using VSLangProj;

namespace Altinoren.ActiveWriter.CodeGeneration
{
    public class VisualStudioHelper
    {
        private readonly DTE _dte;
        private readonly ProjectItem _modelProjectItem;
        private readonly CodeLanguage _language;
        private readonly CodeDomProvider _codeDomProvider;
        private readonly string _defaultNamespace = "";
        private readonly bool _useStrongTyping = true;
        private readonly OutputWindowHelper _outputWindowHelper;

        public VisualStudioHelper(int processID, string modelFile)
        {
            _dte = DTEHelper.GetDTE(processID);
            _modelProjectItem = _dte.Solution.FindProjectItem(modelFile);
            _language = DTEHelper.GetProjectLanguage(_modelProjectItem.ContainingProject);
            _codeDomProvider = GetCodeComProvider(_language);
            if (_language == CodeLanguage.VB)
            {
                _defaultNamespace = GetDefaultNamespace(_modelProjectItem);
                _useStrongTyping = GetOptionExplicit(_modelProjectItem) == prjOptionStrict.prjOptionStrictOn;
            }

            _outputWindowHelper = new OutputWindowHelper(_dte);
        }

        public CodeDomProvider CodeDomProvider
        {
            get { return _codeDomProvider; }
        }

        public VSProject Project
        {
            get { return DTEHelper.GetVSProject(_modelProjectItem); }
        }

        public string AssemblyName
        {
            get { return DTEHelper.GetAssemblyName(_modelProjectItem.ContainingProject); }
        }

        public string DefaultNamespace
        {
            get { return _defaultNamespace; }
        }

        public bool UseStrongTyping
        {
            get { return _useStrongTyping; }
        }

        public bool OutputVisualBasic
        {
            get { return _language == CodeLanguage.VB; }
        }

        public string IntermediatePath
        {
            get
            {
                return DTEHelper.GetIntermediatePath(Project);
            }
        }

        public Dictionary<string, string> References
        {
            get
            {
                Dictionary<string, string> references = new Dictionary<string, string>();
                foreach (Reference reference in Project.References)
                {
                    references[reference.Path] = reference.Name;
                }
                return references;
            }
        }

        public static CodeDomProvider GetCodeComProvider(CodeLanguage codeLanguage)
        {
            switch (codeLanguage)
            {
                case CodeLanguage.CSharp:
                    return new CSharpCodeProvider();

                case CodeLanguage.VB:
                    return new VBCodeProvider();

                default:
                    throw new ArgumentException(
                        "Unsupported project type. ActiveWriter currently supports C# and Visual Basic.NET projects.");
            }
        }

        public static string GetDefaultNamespace(ProjectItem modelProjectItem)
        {
            return GetProperty<string>(modelProjectItem, "DefaultNamespace");
        }

        public static prjOptionStrict GetOptionExplicit(ProjectItem modelProjectItem)
        {
            return GetProperty<prjOptionStrict>(modelProjectItem, "OptionExplicit");
        }

        public static T GetProperty<T>(ProjectItem modelProjectItem, string propertyName)
        {
            var project = (VSProject)modelProjectItem.ContainingProject.Object;
            Property defaultNamespaceProperty = project.Project.Properties.Item(propertyName);
            return (T)defaultNamespaceProperty.Value;
        }

        public void Log(string message)
        {
            _outputWindowHelper.Write(string.Format("ActiveWriter: {0}", message));
        }

        public void AddCodeFile(string path, bool relateWithActiwFile)
        {
            AddProjectItem(path, relateWithActiwFile, prjBuildAction.prjBuildActionCompile);
        }

        public void AddResource(string path, bool relateWithActiwFile)
        {
            AddProjectItem(path, relateWithActiwFile, prjBuildAction.prjBuildActionEmbeddedResource);
        }

        public void AddProjectItem(string path, bool relateWithActiwFile, prjBuildAction buildAction)
        {
            ProjectItem item;

            if (relateWithActiwFile)
                item = _modelProjectItem.ProjectItems.AddFromFile(path);
            else
                item = _dte.ItemOperations.AddExistingItem(path);

            item.Properties.Item("BuildAction").Value = (int)buildAction;
        }

    }
}