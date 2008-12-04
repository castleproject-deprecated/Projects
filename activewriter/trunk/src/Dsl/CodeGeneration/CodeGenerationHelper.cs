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

using Altinoren.ActiveWriter.CodeGeneration.Adapters;

namespace Altinoren.ActiveWriter.CodeGeneration
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using Microsoft.CSharp;
    using Microsoft.VisualBasic;
    using EnvDTE;
    using System.ComponentModel.Design;
    using Microsoft.VisualStudio.Modeling;
    using ServerExplorerSupport;
    using VSLangProj;
    using Microsoft.VisualStudio.TextTemplating;
    using CodeNamespace = System.CodeDom.CodeNamespace;

    public class CodeGenerationHelper
    {
        #region Private Variables

        private readonly CodeGenerationContext _codeGenerationContext;
        private Assembly _activeRecord;
        private readonly Dictionary<string, string> nHibernateConfigs = new Dictionary<string, string>();

        public event EventHandler<FileCreatedEventArgs> CodeFileCreated;
        public event EventHandler<FileCreatedEventArgs> ResourceCreated;
        public event EventHandler<LogMessageEventArgs> Log;
        public event EventHandler<CompilerErrorsEventArgs> CompilerErrors;

        #endregion

        #region ctors

        public CodeGenerationHelper(CodeGenerationContext codeGenerationContext)
        {
            _codeGenerationContext = codeGenerationContext;

            Model model = codeGenerationContext.Model;
        }

        #endregion

        #region Public Methods

        public string Generate()
        {
            CodeCompileUnit compileUnit = GenerateCompileUnit();

            Model model = _codeGenerationContext.Model;
            switch (model.Target)
            {
                case CodeGenerationTarget.NHibernate:
                    return GenerateNHibernateCode(compileUnit, model);

                case CodeGenerationTarget.ActiveRecord:
                default:
                    return GenerateActiveRecordCode(compileUnit, model);
            }
        }

        private CodeCompileUnit GenerateCompileUnit()
        {
            Model model = _codeGenerationContext.Model;

            CodeCompileUnit compileUnit = new CodeCompileUnit();

            if (_codeGenerationContext.UseStrongTyping)
            {
                compileUnit.UserData.Add("AllowLateBound", false);
            }

            CodeNamespace nameSpace = new CodeNamespace(_codeGenerationContext.Namespace);
            nameSpace.Imports.AddRange(model.NamespaceImports.ToArray());
            compileUnit.Namespaces.Add(nameSpace);

            foreach (ModelClass cls in model.Classes)
            {
                GenerateClass(_codeGenerationContext, cls, nameSpace);
            }

            // Just to make sure if there are nested classes not conneced to a class, they are generated.
            foreach (NestedClass cls in model.NestedClasses)
            {
                GenerateNestedClass(_codeGenerationContext, cls, nameSpace);
            }

            if (model.GenerateMetaData != MetaDataGeneration.False)
            {
                GenerateMetaData(nameSpace);
            }
            return compileUnit;
        }

        private string GenerateNHibernateCode(CodeCompileUnit compileUnit, Model model)
        {
            Type starter = null;
            Assembly assembly = null;

            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

                // Code below means: ActiveRecordStarter.ModelsCreated += new ModelsCreatedDelegate(OnARModelCreated);
                _activeRecord = Assembly.Load(model.ActiveRecordAssemblyName);

                starter = _activeRecord.GetType("Castle.ActiveRecord.ActiveRecordStarter");

                EventInfo eventInfo = starter.GetEvent("ModelsValidated");
                if (eventInfo == null)
                {
                    eventInfo = starter.GetEvent("ModelsCreated");
                }

                Type eventType = eventInfo.EventHandlerType;
                MethodInfo info =
                    this.GetType().GetMethod("OnARModelCreated", BindingFlags.Public | BindingFlags.Instance);
                Delegate del = Delegate.CreateDelegate(eventType, this, info);
                eventInfo.AddEventHandler(this, del);

                assembly = GenerateARAssembly(compileUnit, !model.UseNHQG);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
            }

            // Code below means: ActiveRecordStarter.Initialize(assembly, new InPlaceConfigurationSource());
            Type config = _activeRecord.GetType("Castle.ActiveRecord.Framework.Config.InPlaceConfigurationSource");
            object configSource = Activator.CreateInstance(config);
            try
            {
                starter.InvokeMember("ResetInitializationFlag",
                                     BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
                starter.InvokeMember("Initialize",
                                     BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null,
                                     null,
                                     new object[] { assembly, configSource });
            }
            catch (TargetInvocationException ex)
            {
                // Eat config errors
                if (!ex.InnerException.Message.StartsWith("Could not find configuration for"))
                    throw;
            }
            ClearARAttributes(compileUnit);
            string primaryOutput = _codeGenerationContext.GenerateCode(compileUnit);

            foreach (KeyValuePair<string, string> pair in nHibernateConfigs)
            {
                string path = _codeGenerationContext.FullPath(RemoveNamespaceFromStart(pair.Key) + ".hbm.xml");
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode))
                {
                    writer.Write(pair.Value);
                }

                InvokeResourceCreatedEvent(path);
            }

            if (model.UseNHQG)
            {
                CallQueryGenerator(assembly);
            }

            return primaryOutput;
        }

        private string GenerateActiveRecordCode(CodeCompileUnit compileUnit, Model model)
        {
            string primaryOutput = _codeGenerationContext.GenerateCode(compileUnit);

            if (model.UseNHQG)
            {
                try
                {
                    AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
                    Assembly assembly = GenerateARAssembly(compileUnit, false);
                    CallQueryGenerator(assembly);
                }
                finally
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
                }
            }

            return primaryOutput;
        }

        private void InvokeResourceCreatedEvent(string path)
        {
            if (ResourceCreated != null)
            {
                ResourceCreated(this, new FileCreatedEventArgs(path));
            }
        }

        #endregion

        #region Private Methods

 
        #region Code Generation

        private CodeTypeDeclaration GenerateNestedClass(CodeGenerationContext codeGenerationContext, NestedClass cls, CodeNamespace nameSpace)
        {
            if (cls == null)
                throw new ArgumentNullException( "cls", "Nested class not supplied");
            if (nameSpace == null)
                throw new ArgumentNullException("nameSpace", "Namespace not supplied");
            if (String.IsNullOrEmpty(cls.Name))
                throw new ArgumentException("Class name cannot be blank", "cls");

            Model model = codeGenerationContext.Model;

            if (!codeGenerationContext.IsClassGenerated(cls))
            {
                if (codeGenerationContext.IsClassWithSameNameGenerated(cls.Name))
                    throw new ArgumentException(
                        "Ambiguous class name. Code for a class with the same name already generated. Please use a different name.",
                        cls.Name);

                CodeTypeDeclaration classDeclaration = new ClassAdapter(codeGenerationContext, cls, nameSpace);

                // Properties and Fields
                foreach (var property in cls.Properties)
                {
                    CodeMemberField memberField = new FieldAdapter(codeGenerationContext, classDeclaration, property);

                    classDeclaration.Members.Add(memberField);

                    if (property.DebuggerDisplay)
                        classDeclaration.CustomAttributes.Add(property.GetDebuggerDisplayAttribute());
                    if (property.DefaultMember)
                        classDeclaration.CustomAttributes.Add(property.GetDefaultMemberAttribute());
                }

                return classDeclaration;
            }
            
            return codeGenerationContext.GetTypeDeclaration(cls);
        }

        private CodeTypeDeclaration GenerateClass(CodeGenerationContext codeGenerationContext, ModelClass cls, CodeNamespace nameSpace)
        {
            if (cls == null)
                throw new ArgumentNullException("cls", "Class not supplied");
            if (nameSpace == null)
                throw new ArgumentNullException("nameSpace", "Namespace not supplied");
            if (String.IsNullOrEmpty(cls.Name))
                throw new ArgumentException("Class name cannot be blank", "cls");

            if (!codeGenerationContext.IsClassGenerated(cls))
            {
                if (codeGenerationContext.IsClassWithSameNameGenerated(cls.Name))
                    throw new ArgumentException(
                        "Ambiguous class name. Code for a class with the same name already generated. Please use a different name.",
                        cls.Name);

                ClassAdapter classDeclaration = new ClassAdapter(codeGenerationContext, cls, nameSpace);

                List<ModelProperty> compositeKeys = new List<ModelProperty>();

                // Properties and Fields
                foreach (ModelProperty property in cls.Properties)
                {
                    if (property.KeyType != KeyType.CompositeKey)
                    {
                        CodeMemberField memberField = new FieldAdapter(codeGenerationContext, classDeclaration, property);

                        classDeclaration.Members.Add(memberField);

                        if (property.DebuggerDisplay)
                            classDeclaration.CustomAttributes.Add(property.GetDebuggerDisplayAttribute());
                        if (property.DefaultMember)
                            classDeclaration.CustomAttributes.Add(property.GetDefaultMemberAttribute());
                    }
                    else
                        compositeKeys.Add(property);
                }

                if (compositeKeys.Count > 0)
                {
                    CodeTypeDeclaration compositeClass =
                        new ClassAdapter(_codeGenerationContext, nameSpace, classDeclaration, compositeKeys, cls.DoesImplementINotifyPropertyChanged());

                    // TODO: All access fields in a composite group assumed to be the same.
                    // We have a model validator for this case but the user may save anyway.
                    // Check if all access fields are the same.
                    CodeMemberField memberField = new FieldAdapter(codeGenerationContext, compositeClass, PropertyAccess.Property);
                    classDeclaration.Members.Add(memberField);

                    classDeclaration.Members.Add(new CompositeKeyPropertyAdapter(codeGenerationContext, compositeClass, memberField, cls.DoesImplementINotifyPropertyChanged(), cls.DoesImplementINotifyPropertyChanging()));
                }

                //ManyToOne links where this class is the target (1-n)
                ReadOnlyCollection<ManyToOneRelation> manyToOneSources = ManyToOneRelation.GetLinksToSources(cls);
                foreach (ManyToOneRelation relationship in manyToOneSources)
                {
                    classDeclaration.GenerateHasManyRelation(codeGenerationContext, nameSpace, relationship);
                }

                //ManyToOne links where this class is the source (n-1)
                ReadOnlyCollection<ManyToOneRelation> manyToOneTargets = ManyToOneRelation.GetLinksToTargets(cls);
                foreach (ManyToOneRelation relationship in manyToOneTargets)
                {
                    classDeclaration.GenerateBelongsToRelation(codeGenerationContext, nameSpace, relationship);
                }

                //ManyToMany links where this class is the source
                ReadOnlyCollection<ManyToManyRelation> manyToManyTargets = ManyToManyRelation.GetLinksToManyToManyTargets(cls);
                foreach (ManyToManyRelation relationship in manyToManyTargets)
                {
                    classDeclaration.GenerateHasAndBelongsToRelationFromTargets(codeGenerationContext, nameSpace, relationship);
                }

                //ManyToMany links where this class is the target
                ReadOnlyCollection<ManyToManyRelation> manyToManySources = ManyToManyRelation.GetLinksToManyToManySources(cls);
                foreach (ManyToManyRelation relationship in manyToManySources)
                {
                    classDeclaration.GenerateHasAndBelongsToRelationFromSources(codeGenerationContext, nameSpace, relationship);
                }

                //OneToOne link where this class is the source
                OneToOneRelation oneToOneTarget = OneToOneRelation.GetLinkToOneToOneTarget(cls);
                if (oneToOneTarget != null)
                {
                    classDeclaration.GenerateOneToOneRelationFromTarget(codeGenerationContext, nameSpace, oneToOneTarget);
                }

                //OneToOne links where this class is the target
                ReadOnlyCollection<OneToOneRelation> oneToOneSources = OneToOneRelation.GetLinksToOneToOneSources(cls);
                foreach (OneToOneRelation relationship in oneToOneSources)
                {
                    classDeclaration.GenerateOneToOneRelationFromSources(codeGenerationContext, nameSpace, relationship);
                }

                //Nested links
                ReadOnlyCollection<NestedClassReferencesModelClasses> nestingTargets =
                    NestedClassReferencesModelClasses.GetLinksToNestedClasses(cls);
                foreach (NestedClassReferencesModelClasses relationship in nestingTargets)
                {
                    classDeclaration.GenerateNestingRelationFromRelationship(codeGenerationContext, nameSpace, relationship);
                }

                // TODO: Other relation types (any etc)

                return classDeclaration;
            }
            
            return codeGenerationContext.GetTypeDeclaration(cls);
        }

        public static CodeExpression GetBinaryOr(ArrayList list, int indexOfLeft)
        {
            if (indexOfLeft + 1 == list.Count)
                return (CodeExpression)list[0];
            
            if (indexOfLeft + 2 == list.Count)
                return
                    new CodeBinaryOperatorExpression((CodeExpression)list[indexOfLeft],
                                                     CodeBinaryOperatorType.BitwiseOr,
                                                     (CodeExpression)list[indexOfLeft + 1]);
            
            return
                new CodeBinaryOperatorExpression((CodeExpression)list[indexOfLeft],
                                                 CodeBinaryOperatorType.BitwiseOr,
                                                 GetBinaryOr(list, indexOfLeft + 1));
        }

        private void CallQueryGenerator(Assembly assembly)
        {
            Model model = _codeGenerationContext.Model;

            System.Diagnostics.Process nhqg = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            string tempFilePath = Path.Combine(_codeGenerationContext.TemporaryPath, "ActiveWriterHbmOutput");
            DirectoryInfo tempFileFolder = null;

            try
            {
                if (Directory.Exists(tempFilePath))
                {
                    tempFileFolder = new DirectoryInfo(tempFilePath);
                    foreach (var file in tempFileFolder.GetFiles())
                    {
                        file.Delete();
                    }
                }
                else
                    tempFileFolder = Directory.CreateDirectory(tempFilePath);

                startInfo.FileName = model.NHQGExecutable;
                startInfo.WorkingDirectory = Path.GetDirectoryName(model.NHQGExecutable);
                string[] args = new string[4];
                args[0] = "/lang:" + (_codeGenerationContext.OutputVisualBasic ? "VB" : "CS");
                args[1] = "/files:\"" + assembly.Location + "\"";
                args[2] = "/out:\"" + tempFileFolder.FullName + "\"";
                args[3] = "/ns:" + _codeGenerationContext.Namespace;
                startInfo.Arguments = string.Join(" ", args);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.ErrorDialog = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                nhqg.StartInfo = startInfo;

                InvokeLogMessageEvent("Running NHQG with parameters: " + String.Join(" ", args));

                nhqg.Start();
                StreamReader output = nhqg.StandardOutput;
                nhqg.WaitForExit(); // Timeout?

                if (nhqg.ExitCode != 0)
                {
                    throw new TargetException("NHQG exited with code " + nhqg.ExitCode);
                }
                else
                {
                    string consoleOut = output.ReadToEnd();
                    if (!string.IsNullOrEmpty(consoleOut) && consoleOut.StartsWith("An error occured:"))
                    {
                        throw new TargetException("NHQG exited with the following error:\n\n" + consoleOut);
                    }
                    else
                    {
                        foreach (var file in tempFileFolder.GetFiles())
                        {
                            string filePath = _codeGenerationContext.FullPath(file.Name);
                            file.CopyTo(filePath , true);
                            InvokeCodeFileCreatedEvent(filePath);
                        }
                    }
                }
            }
            finally
            {
                if (tempFileFolder != null)
                    tempFileFolder.Delete(true);
            }
        }

        private void InvokeLogMessageEvent(string logMessage)
        {
            if (Log != null)
            {
                Log(this, new LogMessageEventArgs(logMessage));
            }
        }

        private void InvokeCodeFileCreatedEvent(string path)
        {
            if (CodeFileCreated != null)
            {
                CodeFileCreated(this, new FileCreatedEventArgs(path));
            }
        }

        private Assembly GenerateARAssembly(CodeCompileUnit compileUnit, bool generateInMemory)
        {
            Model model = _codeGenerationContext.Model;

            List<string> addedAssemblies = new List<string>();
            string assemblyName = "ActiveWriter.Temp.Assembly" + Guid.NewGuid().ToString("N") + ".dll";
            CompilerParameters parameters = new CompilerParameters
                                                {
                                                    GenerateInMemory = generateInMemory,
                                                    GenerateExecutable = false
                                                };

            Assembly activeRecord = Assembly.Load(model.ActiveRecordAssemblyName);
            parameters.ReferencedAssemblies.Add(activeRecord.Location);
            Assembly nHibernate = Assembly.Load(model.NHibernateAssemblyName);
            parameters.ReferencedAssemblies.Add(nHibernate.Location);
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            addedAssemblies.Add("castle.activerecord.dll");
            addedAssemblies.Add("nhibernate.dll");
            addedAssemblies.Add("system.dll");
            addedAssemblies.Add("mscorlib.dll");

            // also add references to assemblies referenced by this project
            foreach (string reference in _codeGenerationContext.References.Keys)
            {
                if (!addedAssemblies.Contains(Path.GetFileName(reference).ToLowerInvariant()))
                {
                    parameters.ReferencedAssemblies.Add(reference);
                    addedAssemblies.Add(Path.GetFileName(reference).ToLowerInvariant());
                }
            }

            parameters.OutputAssembly = generateInMemory
                                            ? assemblyName
                                            : Path.Combine(
                                                  _codeGenerationContext.TemporaryPath, assemblyName);

            CompilerErrorCollection errorCollection;
            Assembly assembly = _codeGenerationContext.Compile(parameters, compileUnit, out errorCollection);

            if (assembly != null)
                return assembly;

            InvokeCompilerErrorsEvent(errorCollection);

            throw new ModelingException("Cannot compile in-memory ActiveRecord assembly due to errors. Please check that all the information required, such as imports, to compile the generated code in-memory is provided. An ActiveRecord assembly is generated in-memory to support NHibernate .hbm.xml generation and NHQG integration.");

        }

        private void InvokeCompilerErrorsEvent(CompilerErrorCollection errorCollection)
        {
            if (CompilerErrors != null)
            {
                CompilerErrors(this, new CompilerErrorsEventArgs(errorCollection));
            }
        }

        // Actually: OnARModelCreated(ActiveRecordModelCollection, IConfigurationSource)
        // We're not using it typed since we use this through reflection.
        public void OnARModelCreated(object models, object source)
        {
            nHibernateConfigs.Clear();
            if (models != null)
            {
                Type modelCollection = _activeRecord.GetType("Castle.ActiveRecord.Framework.Internal.ActiveRecordModelCollection");
                if ((int)modelCollection.GetProperty("Count").GetValue(models, null) > 0)
                {
                    string actualAssemblyName = ", " + _codeGenerationContext.AssemblyName;
                    IEnumerator enumerator = (IEnumerator)modelCollection.InvokeMember("GetEnumerator", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance, null, models, null);
                    while (enumerator.MoveNext())
                    {
                        Type visitor = _activeRecord.GetType("Castle.ActiveRecord.Framework.Internal.XmlGenerationVisitor");
                        object generationVisitor = Activator.CreateInstance(visitor);
                        visitor.InvokeMember("CreateXml", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null,
                                             generationVisitor, new object[] { enumerator.Current });

                        string xml = (string)visitor.GetProperty("Xml").GetValue(generationVisitor, null);
                        XmlDocument document = new XmlDocument();
                        document.PreserveWhitespace = true;
                        document.LoadXml(xml);
                        XmlNodeList nodeList = document.GetElementsByTagName("class");
                        if (nodeList.Count > 0)
                        {
                            XmlAttribute attribute = (XmlAttribute) nodeList[0].Attributes.GetNamedItem("name");



                            if (attribute != null)
                            {
                                string tempAssemblyName = attribute.Value.Substring(attribute.Value.LastIndexOf(','));

                                string name = attribute.Value;

                                name = name.Substring(0, name.LastIndexOf(','));

                                string newValue = name;
                                // if name isn't a fully-qualified namespace, then prepend project default
                                if ((name.IndexOf('.') < 0) && !string.IsNullOrEmpty(_codeGenerationContext.DefaultNamespace))
                                    newValue = _codeGenerationContext.DefaultNamespace + "." + name;

                                // append assembly name
                                attribute.Value = newValue + actualAssemblyName;

                                // also fix any class attributes
                                XmlNodeList ClassAttributes = document.SelectNodes("//@class");

                                foreach (XmlAttribute ClassAttribute in ClassAttributes)
                                {
                                    UpdateClassName(actualAssemblyName, tempAssemblyName, ClassAttribute);
                                }

                                xml = document.OuterXml;
                                nHibernateConfigs.Add(name, xml);
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fix class attributes that were referencing a temporary assembly name by prefixing with class namespace and using
        /// correct assembly name
        /// </summary>
        /// <param name="actualAssemblyName"></param>
        /// <param name="tempAssemblyName"></param>
        /// <param name="attribute"></param>
        private void UpdateClassName(string actualAssemblyName, string tempAssemblyName, XmlAttribute attribute)
        {
            //remove temporary assembly name
            string name = attribute.Value.Replace(tempAssemblyName, String.Empty);

            if (attribute.Value.Length != name.Length) {

                // if name isn't a fully-qualified namespace, then prepend project default
                if ((name.IndexOf('.') < 0) && !string.IsNullOrEmpty(_codeGenerationContext.DefaultNamespace))
                    name = _codeGenerationContext.DefaultNamespace + "." + name;

                // append assembly name
                attribute.Value = name + actualAssemblyName;
            }
        }

        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName name = new AssemblyName(args.Name);
            if (name.Name == "Castle.ActiveRecord" || name.Name == "Iesi.Collections" || name.Name == "log4net" || name.Name == "NHibernate")
            {
                // If this line is reached, these assemblies are not in GAC. Load from designated place.
                string assemblyPath = Path.Combine(_codeGenerationContext.Model.AssemblyPath, name.Name + ".dll");

                // try project references
                if (!File.Exists(assemblyPath))
                {
                    foreach (KeyValuePair<string, string> reference in _codeGenerationContext.References)
                    {
                        if (reference.Value == name.Name)
                        {
                            assemblyPath = reference.Key;
                            break;
                        }
                    }

                }

                return Assembly.LoadFrom(assemblyPath);
            }

            return null;
        }

        private void ClearARAttributes(CodeCompileUnit unit)
        {
            foreach (CodeNamespace ns in unit.Namespaces)
            {
                List<CodeNamespaceImport> imports = new List<CodeNamespaceImport>();
                foreach (CodeNamespaceImport import in ns.Imports)
                {
                    if (!(import.Namespace.IndexOf("Castle") > -1 || import.Namespace.IndexOf("Nullables") > -1))
                        imports.Add(import);
                }
                ns.Imports.Clear();
                ns.Imports.AddRange(imports.ToArray());

                foreach (CodeTypeDeclaration type in ns.Types)
                {
                    List<CodeAttributeDeclaration> attributesToRemove = new List<CodeAttributeDeclaration>();
                    foreach (CodeAttributeDeclaration attribute in type.CustomAttributes)
                    {
                        if (Array.FindIndex(Common.ARAttributes, name => name == attribute.Name) > -1)
                            attributesToRemove.Add(attribute);
                    }
                    foreach (CodeAttributeDeclaration declaration in attributesToRemove)
                    {
                        type.CustomAttributes.Remove(declaration);
                    }


                    foreach (CodeTypeMember member in type.Members)
                    {
                        List<CodeAttributeDeclaration> memberAttributesToRemove = new List<CodeAttributeDeclaration>();
                        foreach (CodeAttributeDeclaration attribute in member.CustomAttributes)
                        {
                            if (Array.FindIndex(Common.ARAttributes, name => name == attribute.Name) > -1)
                                memberAttributesToRemove.Add(attribute);
                        }
                        foreach (CodeAttributeDeclaration declaration in memberAttributesToRemove)
                        {
                            member.CustomAttributes.Remove(declaration);
                        }
                    }
                }
            }
        }

        private string RemoveNamespaceFromStart(string name)
        {
            if (!string.IsNullOrEmpty(_codeGenerationContext.Namespace) && name.StartsWith(_codeGenerationContext.Namespace))
                return name.Remove(0, _codeGenerationContext.Namespace.Length + 1);

            return name;
        }

        private void GenerateMetaData(CodeNamespace nameSpace)
        {
            foreach (CodeTypeDeclaration type in nameSpace.Types)
            {
                List<CodeTypeMember> properties = new List<CodeTypeMember>();
                foreach (CodeTypeMember member in type.Members)
                {
                    if ((member is CodeMemberProperty || member is CodeMemberField) && ModelProperty.IsMetaDataGeneratable(member))
                    {
                        properties.Add(member);
                    }
                }

                Model model = _codeGenerationContext.Model;

                if (properties.Count > 0)
                {
                    if (model.GenerateMetaData == MetaDataGeneration.InClass)
                    {
                        GenerateInClassMetaData(type, properties);
                    }
                    else if (model.GenerateMetaData == MetaDataGeneration.InSubClass)
                    {
                        GenerateSubClassMetaData(type, properties);
                    }
                }
            }
        }

        private void GenerateInClassMetaData(CodeTypeDeclaration type, IEnumerable<CodeTypeMember> members)
        {
            foreach (CodeTypeMember member in members)
            {
                type.Members.Add(GenerateMetaDataField(member, member.Name + "Property"));
            }
        }

        private void GenerateSubClassMetaData(CodeTypeDeclaration type, IEnumerable<CodeTypeMember> members)
        {
            CodeTypeDeclaration subClass = new ClassAdapter("Properties");
            foreach (var member in members)
            {
                subClass.Members.Add(GenerateMetaDataField(member, member.Name));
            }

            type.Members.Add(subClass);
        }

        private CodeMemberField GenerateMetaDataField(CodeTypeMember member, string nm)
        {
            return new FieldAdapter(Accessor.Public)
                       {
                           Static = true,
                           Name = NamingHelper.GetName(nm, PropertyAccess.Property, FieldCase.Pascalcase),
                           Type = new CodeTypeReference(typeof (string)),
                           InitExpression = new CodePrimitiveExpression(member.Name)
                       };
        }

        #endregion

        #endregion
    }

    public class CompilerErrorsEventArgs : EventArgs
    {
        private readonly CompilerErrorCollection _errorCollection;

        public CompilerErrorsEventArgs(CompilerErrorCollection errorCollection)
        {
            _errorCollection = errorCollection;
        }

        public CompilerErrorCollection ErrorCollection
        {
            get { return _errorCollection; }
        }

        public List<string> ErrorStrings
        {
            get { return GetErrorStrings(null); }
        }

        public List<string> GetErrorStrings(string format)
        {
            List<string> errors = new List<string>();
            foreach (CompilerError error in _errorCollection)
            {
                string errorFormat =
                    format ?? (string.IsNullOrEmpty(error.FileName) ?
                        "{2} {1}: {0}" :
                        "{3}({4},{5}): {2} {1}: {0}");
                
                errors.Add(string.Format(errorFormat,
                    error.ErrorText,
                    error.ErrorNumber,
                    error.IsWarning ? "warning" : "error",
                    error.FileName,
                    error.Line,
                    error.Column
                ));
            }
            return errors;
        }
    }
}
