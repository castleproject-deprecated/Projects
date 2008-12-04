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
using System.IO;
using System.Reflection;
using System.Text;
using Altinoren.ActiveWriter.ServerExplorerSupport;
using EnvDTE;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TextTemplating;

namespace Altinoren.ActiveWriter.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.CodeDom;
    using Microsoft.VisualStudio.Modeling;

    public class CodeGenerationContext
    {
        private readonly Model _model;
        private readonly string _namespace;
        private readonly string _defaultNamespace;
        private readonly string _assemblyName;
        private readonly CodeDomProvider _codeDomProvider;
        private readonly bool _useStrongTyping;
        private readonly Dictionary<ModelClass, CodeTypeDeclaration> _classDeclarations = new Dictionary<ModelClass, CodeTypeDeclaration>();
        private readonly Dictionary<NestedClass, CodeTypeDeclaration> _nestedClassDeclarations = new Dictionary<NestedClass, CodeTypeDeclaration>();
        private readonly List<string> _generatedClassNames = new List<string>();
        private readonly bool _outputVisualBasic;
        private readonly string _outputPath;
        private readonly string _tempPath;
        private readonly Dictionary<string, string> _references;

        public CodeGenerationContext(Model model, string nameSpace, string defaultNamespace, string assemblyName, string outputPath, string tempPath)
            : this(model, nameSpace, defaultNamespace, assemblyName, outputPath, tempPath, new CSharpCodeProvider(), true, new Dictionary<string, string>())
        {}

        public CodeGenerationContext(Model model, string nameSpace, string defaultNamespace, string assemblyName, string outputPath, string tempPath, CodeDomProvider codeDomProvider)
            : this(model, nameSpace, defaultNamespace, assemblyName, outputPath, tempPath, codeDomProvider, true, new Dictionary<string, string>())
        {}

        public CodeGenerationContext(Model model, string nameSpace, string defaultNamespace, string assemblyName, string outputPath, string tempPath, CodeDomProvider codeDomProvider, bool useStrongTyping)
            : this(model, nameSpace, defaultNamespace, assemblyName, outputPath, tempPath, codeDomProvider, useStrongTyping, new Dictionary<string, string>())
        {}

        public CodeGenerationContext(Model model, string nameSpace, string defaultNamespace, string assemblyName, string outputPath, string tempPath, CodeDomProvider codeDomProvider, bool useStrongTyping, Dictionary<string, string> references)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (outputPath == null) throw new ArgumentNullException("outputPath");
            if (!Directory.Exists(outputPath)) throw new DirectoryNotFoundException(string.Format("Model directory {0} not found", outputPath));

            _model = model;
            _namespace = nameSpace;
            _defaultNamespace = defaultNamespace;
            _assemblyName = assemblyName;
            _codeDomProvider = codeDomProvider;
            _outputVisualBasic = typeof (VBCodeProvider) == codeDomProvider.GetType();
            _useStrongTyping = useStrongTyping;
            _outputPath = outputPath;
            _tempPath = tempPath;
            _references = references;
            _namespace =
                string.IsNullOrEmpty(model.Namespace) ?
                    nameSpace :
                    model.Namespace;
        }

        public bool OutputVisualBasic
        {
            get { return _outputVisualBasic; }
        }

        public bool UseStrongTyping
        {
            get { return _useStrongTyping; }
        }

        public string Namespace
        {
            get { return _namespace; }
        }

        public string DefaultNamespace
        {
            get { return _defaultNamespace; }
        }

        public string AssemblyName
        {
            get { return _assemblyName; }
        }

        public Model Model
        {
            get { return _model; }
        }

        public void RegisterClass(ModelElement cls, CodeTypeDeclaration declaration)
        {
            if (cls == null)
                throw new ArgumentNullException("cls", "No class supplied");
            if (declaration == null)
                throw new ArgumentNullException("declaration", "No CodeTypeDeclaration supplied");

            if (cls.GetType() == typeof(ModelClass))
            {
                ModelClass modelClass = (ModelClass)cls;
                if (!_classDeclarations.ContainsKey(modelClass))
                    _classDeclarations.Add(modelClass, declaration);
            }
            else if (cls.GetType() == typeof(NestedClass))
            {
                NestedClass nestedClass = (NestedClass)cls;
                if (!_nestedClassDeclarations.ContainsKey(nestedClass))
                    _nestedClassDeclarations.Add(nestedClass, declaration);
            }
            else
                throw new ArgumentException("Only ModelClass and NestedClass entities supported", "cls");

            _generatedClassNames.Add(((NamedElement)cls).Name);
        }
        
        public CodeTypeDeclaration GetTypeDeclaration(ModelElement cls)
        {
            if (cls == null)
                throw new ArgumentNullException("cls", "No class supplied");

            if (cls.GetType() == typeof(ModelClass))
            {
                return _classDeclarations[(ModelClass)cls];
            }
            if (cls.GetType() == typeof(NestedClass))
            {
                return _nestedClassDeclarations[(NestedClass)cls];
            }
            
            throw new ArgumentException("Only ModelClass and NestedClass entities supported", "cls");
        }

        public bool IsClassGenerated(ModelElement cls)
        {
            if (cls == null)
                throw new ArgumentNullException("cls", "No class supplied");

            if (cls.GetType() == typeof(ModelClass))
            {
                return _classDeclarations.ContainsKey((ModelClass)cls);
            }

            if (cls.GetType() == typeof(NestedClass))
            {
                return _nestedClassDeclarations.ContainsKey((NestedClass)cls);
            }

            throw new ArgumentException("Only ModelClass and NestedClass entities supported", "cls");
        }

        public bool IsClassWithSameNameGenerated(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "Name not supplied");

            return _generatedClassNames.Contains(name);
        }

        public string FullPath(string path)
        {
            return Path.Combine(_outputPath, path);
        }

        public string TemporaryPath
        {
            get { return _tempPath; }
        }

        public Dictionary<string, string> References
        {
            get { return _references; }
        }

        public Assembly Compile(CompilerParameters parameters, CodeCompileUnit compileUnit, out CompilerErrorCollection errorCollection)
        {
            CompilerResults results = _codeDomProvider.CompileAssemblyFromDom(parameters, compileUnit);

            errorCollection = results.Errors;

            return results.Errors.Count == 0 ? results.CompiledAssembly : null;
        }

        public string GenerateCode(CodeCompileUnit compileUnit)
        {
            StringBuilder codeBuilder = new StringBuilder();
            using (StringWriter codeWriter = new StringWriter(codeBuilder))
            {
                _codeDomProvider.GenerateCodeFromCompileUnit(compileUnit, codeWriter, new CodeGeneratorOptions());
            }

            return codeBuilder.ToString();
        }

    }
}
