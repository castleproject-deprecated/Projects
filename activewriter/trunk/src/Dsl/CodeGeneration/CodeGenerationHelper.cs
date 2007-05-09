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

using System.Xml;

namespace Altinoren.ActiveWriter.CodeGeneration
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Microsoft.CSharp;
    using Microsoft.VisualBasic;
    using EnvDTE;
    using System.ComponentModel.Design;
    using ARValidators;
    using CodeNamespace = System.CodeDom.CodeNamespace;
    using CodeAttributeArgument = System.CodeDom.CodeAttributeArgument;

    public class CodeGenerationHelper
    {
        #region Private Variables

        private Assembly _activeRecord;
        private Dictionary<string, string> nHibernateConfigs = new Dictionary<string, string>();
        private string _assemblyLoadPath;

        private CodeDomProvider _provider;
        private Model _model;
        private string _namespace;

        private Dictionary<ModelClass, CodeTypeDeclaration> _classDeclarations;
        private List<string> _generatedClassNames;

        private Hashtable _propertyBag = null;
        private DTE _dte = null;

        #endregion

        #region ctors

        public CodeGenerationHelper(Hashtable propertyBag)
        {
            _propertyBag = propertyBag;
            _model = (Model)propertyBag["Generic.Model"];
            if (string.IsNullOrEmpty(_model.Namespace))
                _namespace = propertyBag["Generic.Namespace"].ToString();
            else
                _namespace = _model.Namespace;

            _dte = ServerExplorerSupport.DTEHelper.GetDTE(_propertyBag["Generic.ProcessID"].ToString());
            _propertyBag.Add("Generic.DTE", _dte);

            switch (ServerExplorerSupport.DTEHelper.GetProjectLanguage(_dte.ActiveDocument.ProjectItem.ContainingProject))
            {
                case CodeLanguage.CSharp:
                    _provider = new CSharpCodeProvider();
                    _propertyBag.Add("Generic.Language", CodeLanguage.CSharp);
                    break;
                case CodeLanguage.VB:
                    _provider = new VBCodeProvider();
                    _propertyBag.Add("Generic.Language", CodeLanguage.VB);
                    break;
                default:
                    throw new ArgumentException(
                        "Unsupported project type. ActiveWriter currently supports C# and Visual Basic.NET projects.");
            }

            _classDeclarations = new Dictionary<ModelClass, CodeTypeDeclaration>(_model.Classes.Count);
            _generatedClassNames = new List<string>(_model.Classes.Count);
        }

        #endregion

        #region Public Methods

        public void Generate()
        {
            CodeCompileUnit compileUnit = new CodeCompileUnit();
            _propertyBag.Add("CodeGeneration.CodeCompileUnit", compileUnit);

            CodeNamespace nameSpace = GetDefaultNamespace();
            compileUnit.Namespaces.Add(nameSpace);

            foreach (ModelClass cls in _model.Classes)
            {
                GenerateClass(cls, nameSpace);
            }

            if (_model.Target == CodeGenerationTarget.ActiveRecord)
            {
                string primaryOutput = GenerateCode(compileUnit);
                _propertyBag.Add("CodeGeneration.PrimaryOutput", primaryOutput);
            }
            else
            {
                _assemblyLoadPath = _model.AssemblyPath;
                Type starter = null;
                Assembly assembly = null;

                try
                {
                    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);

                    // Code below means: ActiveRecordStarter.ModelsCreated += new ModelsCreatedDelegate(OnARModelCreated);
                    _activeRecord = Assembly.Load(_model.ActiveRecordAssemblyName);
                    starter = _activeRecord.GetType("Castle.ActiveRecord.ActiveRecordStarter");
                    EventInfo eventInfo = starter.GetEvent("ModelsCreated");
                    Type eventType = eventInfo.EventHandlerType;
                    MethodInfo info =
                        this.GetType().GetMethod("OnARModelCreated", BindingFlags.Public | BindingFlags.Instance);
                    Delegate del = Delegate.CreateDelegate(eventType, this, info);
                    eventInfo.AddEventHandler(this, del);

                    assembly = GenerateARAssembly(compileUnit);
                }
                finally
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyResolve);
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
                string primaryOutput = GenerateCode(compileUnit);
                _propertyBag.Add("CodeGeneration.PrimaryOutput", primaryOutput);

                foreach (KeyValuePair<string, string> pair in nHibernateConfigs)
                {
                    string path = Path.Combine(_dte.ActiveDocument.Path, RemoveNamespaceFromStart(pair.Key) + ".hbm.xml");
                    using (StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode))
                    {
                        writer.Write(pair.Value);
                    }

                    ProjectItem item = null;

                    if (_model.RelateWithActiwFile)
                        item = _dte.ActiveDocument.ProjectItem.ProjectItems.AddFromFile(path);
                    else
                        item = _dte.ItemOperations.AddExistingItem(path);

                    item.Properties.Item("BuildAction").Value = Common.EmbeddedResourceBuildActionIndex;
                }
            }
        }

        #endregion

        #region Private Methods

        #region Class

        private CodeTypeDeclaration GetClassDeclaration(ModelClass cls, CodeNamespace nameSpace)
        {
            if (cls == null)
                throw new ArgumentException("Class not supplied.", "cls");
            if (String.IsNullOrEmpty(cls.Name))
                throw new ArgumentException("Class name cannot be blank.", "cls");

            // TODO: Pluralize the name?
            CodeTypeDeclaration classDeclaration = new CodeTypeDeclaration(cls.Name);

            classDeclaration.TypeAttributes = TypeAttributes.Public;
            classDeclaration.IsPartial = true;

            if (cls.Model.UseBaseClass)
            {
                bool withValidator = cls.Properties.FindAll(delegate(ModelProperty property)
                {
                    return property.IsValidatorSet();
                }).Count > 0;

                CodeTypeReference type;
                // base class for every modelclass. If left empty then baseclass from model if left emprty ...etc
                if (!string.IsNullOrEmpty(cls.BaseClassName))
                    type = type = new CodeTypeReference(cls.BaseClassName);
                else if (!string.IsNullOrEmpty(cls.Model.BaseClassName))
                    type = new CodeTypeReference(cls.Model.BaseClassName);
                else if (withValidator)
                    type = new CodeTypeReference(Common.DefaultValidationBaseClass);
                else
                    type = new CodeTypeReference(Common.DefaultBaseClass);

                if (cls.Model.UseGenerics)
                    type.TypeArguments.Add(classDeclaration.Name);
                classDeclaration.BaseTypes.Add(type);
            }

            if (!String.IsNullOrEmpty(cls.Description))
                classDeclaration.Comments.AddRange(GetSummaryComment(cls.Description));

            classDeclaration.CustomAttributes.Add(GetActiveRecordAttribute(cls));
            if (cls.Model.UseGeneratedCodeAttribute)
                classDeclaration.CustomAttributes.Add(GetGeneratedCodeAttribute());
            // Make a note as "generated" to prevent recursive generation attempts
            _classDeclarations.Add(cls, classDeclaration);
            _generatedClassNames.Add(cls.Name);

            nameSpace.Types.Add(classDeclaration);
            return classDeclaration;
        }

        private CodeTypeDeclaration GetCompositeClassDeclaration(CodeNamespace nameSpace,
                                                                 CodeTypeDeclaration parentClass,
                                                                 List<ModelProperty> keys)
        {
            if (keys == null || keys.Count <= 1)
                throw new ArgumentException("Composite keys must consist of two or more properties.", "keys");

            string className = null;
            foreach (ModelProperty property in keys)
            {
                if (!string.IsNullOrEmpty(property.CompositeKeyName))
                {
                    className = property.CompositeKeyName;
                    break;
                }
            }

            if (string.IsNullOrEmpty(className))
                className = parentClass.Name + Common.CompositeClassNameSuffix;

            CodeTypeDeclaration classDeclaration = new CodeTypeDeclaration(className);

            classDeclaration.TypeAttributes = TypeAttributes.Public;
            classDeclaration.IsPartial = true;

            classDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration("Serializable"));
            if (keys[0].ModelClass.Model.UseGeneratedCodeAttribute)
                classDeclaration.CustomAttributes.Add(GetGeneratedCodeAttribute());

            List<CodeMemberField> fields = new List<CodeMemberField>();

            List<string> descriptions = new List<string>();

            foreach (ModelProperty property in keys)
            {
                CodeMemberField memberField = GetMemberFieldOfProperty(property, Accessor.Private);
                classDeclaration.Members.Add(memberField);
                fields.Add(memberField);

                classDeclaration.Members.Add(GetActiveRecordMemberKeyProperty(memberField, property));

                if (!String.IsNullOrEmpty(property.Description))
                    descriptions.Add(property.Description);
            }

            if (descriptions.Count > 0)
                classDeclaration.Comments.AddRange(GetSummaryComment(descriptions.ToArray()));

            classDeclaration.Members.Add(GetCompositeClassToStringMethod(fields));
            classDeclaration.Members.Add(GetCompositeClassEqualsMethod(className, fields));
            classDeclaration.Members.AddRange(GetCompositeClassGetHashCodeMethods(fields));

            nameSpace.Types.Add(classDeclaration);
            return classDeclaration;
        }

        #endregion

        #region Method

        private CodeTypeMember[] GetCompositeClassGetHashCodeMethods(List<CodeMemberField> fields)
        {
            //public override int GetHashCode()
            //{
            //  return _keyA.GetHashCode() ^ _keyB.GetHashCode();
            //}

            CodeTypeMember[] methods = new CodeTypeMember[2];

            CodeMemberMethod getHashCode = new CodeMemberMethod();
            getHashCode.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            getHashCode.ReturnType = new CodeTypeReference(typeof(Int32));
            getHashCode.Name = "GetHashCode";

            List<CodeExpression> expressions = new List<CodeExpression>();
            foreach (CodeMemberField field in fields)
            {
                expressions.Add(
                    new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(null, field.Name), "GetHashCode"));
            }

            // Now, there's no CodeBinaryOperatorType.XOr or something. We write a helper method instead.
            CodeMemberMethod xor = new CodeMemberMethod();
            xor.Attributes = MemberAttributes.Private;
            xor.ReturnType = new CodeTypeReference(typeof(Int32));
            xor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Int32), "left"));
            xor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Int32), "right"));
            xor.Name = Common.XorHelperMethod;
            if (_provider is VBCodeProvider)
                xor.Statements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("left XOR right")));
            else
                xor.Statements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("left ^ right")));

            CodeExpression expression;
            if (expressions.Count > 2)
                expression =
                    new CodeMethodInvokeExpression(null, Common.XorHelperMethod, expressions[0], GetXor(expressions, 1));
            else
                expression =
                    new CodeMethodInvokeExpression(null, Common.XorHelperMethod, expressions[0], expressions[1]);

            getHashCode.Statements.Add(new CodeMethodReturnStatement(expression));

            methods[0] = getHashCode;
            methods[1] = xor;

            return methods;
        }

        private CodeTypeMember GetCompositeClassEqualsMethod(string className, List<CodeMemberField> fields)
        {
            //public override bool Equals( object obj )
            //{
            //    if( obj == this ) return true;
            //    if( obj == null || obj.GetType() != this.GetType() ) return false;
            //    MyCompositeKey test = ( MyCompositeKey ) obj;
            //    return ( _keyA == test.KeyA || (_keyA != null && _keyA.Equals( test.KeyA ) ) ) &&
            //      ( _keyB == test.KeyB || ( _keyB != null && _keyB.Equals( test.KeyB ) ) );
            //}
            CodeMemberMethod equals = new CodeMemberMethod();
            equals.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            equals.ReturnType = new CodeTypeReference(typeof(Boolean));
            equals.Name = "Equals";

            CodeParameterDeclarationExpression param = new CodeParameterDeclarationExpression(typeof(Object), "obj");
            equals.Parameters.Add(param);

            equals.Statements.Add(new CodeConditionStatement(
                                      new CodeBinaryOperatorExpression(
                                          new CodeFieldReferenceExpression(null, "obj"),
                                          CodeBinaryOperatorType.ValueEquality, new CodeThisReferenceExpression()
                                          ), new CodeMethodReturnStatement(new CodePrimitiveExpression(true))
                                      )
                );

            equals.Statements.Add(new CodeConditionStatement
                                      (
                                      new CodeBinaryOperatorExpression
                                          (
                                          new CodeBinaryOperatorExpression(
                                              new CodeFieldReferenceExpression(null, "obj"),
                                              CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(null)),
                                          CodeBinaryOperatorType.BooleanOr,
                                          new CodeBinaryOperatorExpression(
                                              new CodeMethodInvokeExpression(
                                                  new CodeFieldReferenceExpression(null, "obj"), "GetType"),
                                              CodeBinaryOperatorType.IdentityInequality,
                                              new CodeMethodInvokeExpression(new CodeThisReferenceExpression(),
                                                                             "GetType"))
                                          )
                                      , new CodeMethodReturnStatement(new CodePrimitiveExpression(false))
                                      )
                );

            equals.Statements.Add(
                new CodeVariableDeclarationStatement(new CodeTypeReference(className), "test",
                                                     new CodeCastExpression(new CodeTypeReference(className),
                                                                            new CodeFieldReferenceExpression(null, "obj"))));

            List<CodeExpression> expressions = new List<CodeExpression>();
            foreach (CodeMemberField field in fields)
            {
                expressions.Add(
                    new CodeBinaryOperatorExpression(
                    //_keyA == test.KeyA
                        new CodeBinaryOperatorExpression(
                            new CodeFieldReferenceExpression(null, field.Name),
                            CodeBinaryOperatorType.ValueEquality,
                            new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(null, "test"), field.Name)),
                        CodeBinaryOperatorType.BooleanOr, // ||
                        new CodeBinaryOperatorExpression(
                    //_keyA != null
                            new CodeBinaryOperatorExpression(
                                new CodeFieldReferenceExpression(null, field.Name),
                                CodeBinaryOperatorType.IdentityInequality,
                                new CodePrimitiveExpression(null)
                                ),
                            CodeBinaryOperatorType.BooleanAnd, // &&
                    // _keyA.Equals( test.KeyA )   
                            new CodeMethodInvokeExpression(
                                new CodeFieldReferenceExpression(null, field.Name), "Equals",
                                new CodeFieldReferenceExpression(
                                    new CodeFieldReferenceExpression(null, "test"), field.Name))
                            )
                        )
                    );
            }

            CodeExpression expression;
            if (expressions.Count > 2)
                expression =
                    new CodeBinaryOperatorExpression(expressions[0], CodeBinaryOperatorType.BooleanAnd,
                                                     GetBooleanAnd(expressions, 1));
            else
                expression =
                    new CodeBinaryOperatorExpression(expressions[0], CodeBinaryOperatorType.BooleanAnd, expressions[1]);


            equals.Statements.Add(new CodeMethodReturnStatement(expression));

            return equals;
        }

        private static CodeMemberMethod GetCompositeClassToStringMethod(List<CodeMemberField> fields)
        {
            //public override string ToString()
            //{
            //  return string.Join( ":", new string[] { _keyA, _keyB } );
            //}
            CodeMemberMethod toString = new CodeMemberMethod();
            toString.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            toString.ReturnType = new CodeTypeReference(typeof(String));
            toString.Name = "ToString";

            CodeExpression[] expressions = new CodeExpression[fields.Count];
            for (int i = 0; i < fields.Count; i++)
            {
                expressions[i] =
                    new CodeMethodInvokeExpression(
                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fields[i].Name), "ToString");
            }

            toString.Statements.Add(new CodeMethodReturnStatement(
                                        new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Join",
                                                                       new CodeSnippetExpression("\":\""),
                                                                       new CodeArrayCreateExpression(typeof(String),
                                                                                                     expressions)))
                );
            return toString;
        }

        #endregion

        #region Property

        // TODO: All this property generation is a total mess. Lots of similar methods. Hard to find what you're looking for.

        private CodeTypeMember GetActiveRecordMemberCompositeKeyProperty(CodeTypeDeclaration compositeClass,
                                                                         CodeMemberField memberField)
        {
            // TODO: Composite key generation omits UnsavedValue property. All the properties which are parts of the composite key
            // should have the same UnsavedValue, by the way.
            CodeMemberProperty memberProperty =
                GetMemberProperty(memberField, compositeClass.Name, true, true, null);

            memberProperty.CustomAttributes.Add(new CodeAttributeDeclaration("CompositeKey"));

            return memberProperty;
        }

        private CodeMemberProperty GetActiveRecordMemberKeyProperty(CodeMemberField memberField,
                                                                    ModelProperty property)
        {
            CodeMemberProperty memberProperty =
                GetMemberProperty(memberField, property.Name, property.ColumnType, property.NotNull, true, true, property.Description);
            memberProperty.CustomAttributes.Add(GetKeyPropertyAttribute(property));

            return memberProperty;
        }

        private CodeMemberProperty GetActiveRecordMemberProperty(CodeMemberField memberField,
                                                                 ModelProperty property)
        {
            CodeMemberProperty memberProperty =
                GetMemberProperty(memberField, property.Name, property.ColumnType,
								  property.NotNull,
                                  true,
                                  true,
                                  property.Description);
            CodeAttributeDeclaration attributeDecleration = null;

            switch (property.KeyType)
            {
                // Composite keys must be handled in upper levels
                case KeyType.None:
                    attributeDecleration = GetPropertyAttribute(property);
                    break;
                case KeyType.PrimaryKey:
                    attributeDecleration = GetPrimaryKeyAttribute(property);
                    break;
            }

            memberProperty.CustomAttributes.Add(attributeDecleration);

            return memberProperty;
        }

        private CodeMemberProperty GetActiveRecordMemberVersion(CodeMemberField memberField,
                                                                ModelProperty property)
        {
            CodeMemberProperty memberProperty =
                GetMemberProperty(memberField, property.Name, property.ColumnType, property.NotNull, true, true,
                                  property.Description);
            memberProperty.CustomAttributes.Add(GetVersionAttribute(property));

            return memberProperty;
        }

        private CodeMemberProperty GetActiveRecordMemberTimestamp(CodeMemberField memberField,
                                                                  ModelProperty property)
        {
            CodeMemberProperty memberProperty =
                GetMemberProperty(memberField, property.Name, property.ColumnType, property.NotNull, true, true,
                                  property.Description);
            memberProperty.CustomAttributes.Add(GetTimestampAttribute(property));

            return memberProperty;
        }

		private CodeMemberProperty GetMemberProperty(CodeMemberField memberField, string propertyName,
											 NHibernateType propertyType, bool propertyNotNull, bool get, bool set, params string[] description)
		{
            CodeMemberProperty memberProperty =
                GetMemberPropertyWithoutType(memberField, propertyName, get, set, description);

			if (_model.UseNullables != NullableUsage.No && IsNullable(propertyType) && !propertyNotNull)
            {
                if (_model.UseNullables == NullableUsage.WithHelperLibrary)
                    memberProperty.Type = GetNullableTypeReferenceForHelper(propertyType);
                else
                    memberProperty.Type = GetNullableTypeReference(propertyType);
            }
            else
                memberProperty.Type = new CodeTypeReference(GetSystemType(propertyType));

            return memberProperty;
        }

        private CodeMemberProperty GetMemberProperty(CodeMemberField memberField, string propertyName,
                                                     bool get, bool set, params string[] description)
        {
            CodeMemberProperty memberProperty =
                GetMemberPropertyWithoutType(memberField, propertyName, get, set, description);

            memberProperty.Type = memberField.Type;

            return memberProperty;
        }

        private CodeMemberProperty GetMemberPropertyWithoutType(CodeMemberField memberField, string propertyName,
                                                                bool get, bool set, params string[] description)
        {
            CodeMemberProperty memberProperty = new CodeMemberProperty();

            memberProperty.Name = propertyName;

            if (_model.UseVirtualProperties)
                memberProperty.Attributes = MemberAttributes.Public;
            else
                memberProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            if (get)
                memberProperty.GetStatements.Add(new CodeMethodReturnStatement(
                                                     new CodeFieldReferenceExpression(
                                                         new CodeThisReferenceExpression(), memberField.Name)));
            if (set)
                memberProperty.SetStatements.Add(
                    new CodeAssignStatement(
                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), memberField.Name),
                        new CodeArgumentReferenceExpression("value")
                        )
                    );

            if (description != null && description.Length > 0)
                memberProperty.Comments.AddRange(GetSummaryComment(description));

            return memberProperty;
        }

        #endregion

        #region Field

        private CodeMemberField GetPrivateMemberFieldOfCompositeClass(CodeTypeDeclaration compositeClass, PropertyAccess access)
        {
            return GetMemberField(compositeClass.Name, compositeClass.Name, Accessor.Private, access);
        }

        private CodeMemberField GetMemberFieldOfProperty(ModelProperty property, Accessor accessor)
        {
			if (_model.UseNullables != NullableUsage.No && IsNullable(property.ColumnType) && !property.NotNull)
            {
                if (_model.UseNullables == NullableUsage.WithHelperLibrary)
                    return GetMemberField(property.Name, GetNullableTypeReferenceForHelper(property.ColumnType), accessor, property.Access);
                else
                    return GetMemberField(property.Name, GetNullableTypeReference(property.ColumnType), accessor, property.Access);
            }
            else
                return GetMemberField(property.Name, GetSystemType(property.ColumnType), accessor, property.Access);
        }

        private CodeMemberField GetMemberField(string name, CodeTypeReference fieldType, Accessor accessor, PropertyAccess access)
        {
            CodeMemberField memberField = GetMemberFieldWithoutType(name, accessor, access);

            memberField.Type = fieldType;

            return memberField;
        }

        private CodeMemberField GetMemberField(string name, Type fieldType, Accessor accessor, PropertyAccess access)
        {
            CodeMemberField memberField = GetMemberFieldWithoutType(name, accessor, access);

            memberField.Type = new CodeTypeReference(fieldType);

            return memberField;
        }

        private CodeMemberField GetMemberField(string name, string fieldType, Accessor accessor, PropertyAccess access)
        {
            CodeMemberField memberField = GetMemberFieldWithoutType(name, accessor, access);

            memberField.Type = new CodeTypeReference(fieldType);

            return memberField;
        }

        private CodeMemberField GetGenericMemberField(string typeName, string name, string fieldType, Accessor accessor, PropertyAccess access)
        {
            CodeMemberField memberField = GetMemberFieldWithoutType(name, accessor, access);

            CodeTypeReference type = new CodeTypeReference(fieldType);
            type.TypeArguments.Add(typeName);
            memberField.Type = type;

            return memberField;
        }

        private CodeMemberField GetMemberFieldWithoutType(string name, Accessor accessor, PropertyAccess access)
        {
            CodeMemberField memberField = new CodeMemberField();

            switch (accessor)
            {
                case Accessor.Public:
                    memberField.Attributes = MemberAttributes.Public;
                    break;
                case Accessor.Protected:
                    memberField.Attributes = MemberAttributes.Family;
                    break;
                case Accessor.Private:
                    memberField.Attributes = MemberAttributes.Private;
                    break;
            }

            memberField.Name = GetName(name, access);

            return memberField;
        }

        #endregion

        #region Relation

        #region HasMany

        private void GenerateHasManyRelation(CodeTypeDeclaration classDeclaration, CodeNamespace nameSpace,
                                             ManyToOneRelation relationship)
        {
            CodeTypeDeclaration sourceClass = GenerateClass(relationship.Source, nameSpace);

            string propertyName = String.IsNullOrEmpty(relationship.TargetPropertyName)
                                      ? Common.GetPlural(sourceClass.Name)
                                      : Common.GetPlural(relationship.TargetPropertyName);
            string propertyType = String.IsNullOrEmpty(relationship.TargetPropertyType)
                                      ? "IList"
                                      : relationship.TargetPropertyType;

            CodeMemberField memberField;
            if (!relationship.Source.Model.UseGenerics)
                memberField = GetMemberField(propertyName, propertyType, Accessor.Private, relationship.TargetAccess);
            else
                memberField = GetGenericMemberField(sourceClass.Name, propertyName, propertyType, Accessor.Private, relationship.TargetAccess);
            classDeclaration.Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.TargetDescription))
                memberProperty = GetMemberProperty(memberField, propertyName, true, true, null);
            else
                memberProperty =
                    GetMemberProperty(memberField, propertyName, true, true,
                                      relationship.TargetDescription);
            classDeclaration.Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(GetHasManyAttribute(relationship));
        }

        #endregion

        #region BelongsTo

        private void GenerateBelongsToRelation(CodeTypeDeclaration classDeclaration, CodeNamespace nameSpace,
                                               ManyToOneRelation relationship)
        {
            if (!String.IsNullOrEmpty(relationship.TargetColumnKey) && (!String.IsNullOrEmpty(relationship.SourceColumn)) &&
                !relationship.SourceColumn.ToUpperInvariant().Equals(relationship.TargetColumnKey.ToUpperInvariant()))
                throw new ArgumentException(
                    String.Format(
                        "Class {0} column name does not match with column key {1} on it's many to one relation to class {2}",
                        relationship.Source.Name, relationship.TargetColumnKey, relationship.Target.Name));

            CodeTypeDeclaration targetClass = GenerateClass(relationship.Target, nameSpace);

            string propertyName = String.IsNullOrEmpty(relationship.SourcePropertyName)
                                      ? targetClass.Name
                                      : relationship.SourcePropertyName;

            // We use PropertyAccess.Property to default it to camel case underscore
            CodeMemberField memberField = GetMemberField(propertyName, targetClass.Name, Accessor.Private, PropertyAccess.Property);
            classDeclaration.Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.SourceDescription))
                memberProperty = GetMemberProperty(memberField, propertyName, true, true, null);
            else
                memberProperty =
                    GetMemberProperty(memberField, propertyName, true, true,
                                      relationship.SourceDescription);
            classDeclaration.Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(GetBelongsToAttribute(relationship));
        }

        #endregion

        #region HasAndBelongsToRelation

        private void GenerateHasAndBelongsToRelationFromTargets(CodeTypeDeclaration classDeclaration, CodeNamespace nameSpace,
                                               ManyToManyRelation relationship)
        {
            if (String.IsNullOrEmpty(relationship.Table))
                throw new ArgumentNullException(
                    String.Format("Class {0} does not have a table name on it's many to many relation to class {1}",
                                  relationship.Source.Name, relationship.Target.Name));
            if (String.IsNullOrEmpty(relationship.SourceColumn))
                throw new ArgumentNullException(
                    String.Format("Class {0} does not have a source column name on it's many to many relation to class {1}",
                                  relationship.Source.Name, relationship.Target.Name));
            if (String.IsNullOrEmpty(relationship.TargetColumn))
                throw new ArgumentNullException(
                    String.Format("Class {0} does not have a target column name on it's many to many relation to class {1}",
                                  relationship.Source.Name, relationship.Target.Name));

            CodeTypeDeclaration targetClass = GenerateClass(relationship.Target, nameSpace);

            string propertyName = String.IsNullOrEmpty(relationship.SourcePropertyName)
                                      ? Common.GetPlural(relationship.Source.Name)
                                      : Common.GetPlural(relationship.SourcePropertyName);

            string propertyType = String.IsNullOrEmpty(relationship.TargetPropertyType)
                                      ? "IList"
                                      : relationship.TargetPropertyType;

            CodeMemberField memberField;
            if (!relationship.Source.Model.UseGenerics)
                memberField = GetMemberField(propertyName, propertyType, Accessor.Private, relationship.TargetAccess);
            else
                memberField = GetGenericMemberField(targetClass.Name, propertyName, propertyType, Accessor.Private, relationship.TargetAccess);

            classDeclaration.Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.SourceDescription))
                memberProperty = GetMemberProperty(memberField, Common.GetPlural(propertyName), true, true, null);
            else
                memberProperty =
                    GetMemberProperty(memberField, Common.GetPlural(propertyName), true, true,
                                      relationship.SourceDescription);
            classDeclaration.Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(GetHasAndBelongsToAttribute(relationship, true));
        }

        private void GenerateHasAndBelongsToRelationFromSources(CodeTypeDeclaration classDeclaration, CodeNamespace nameSpace,
                                               ManyToManyRelation relationship)
        {
            if (String.IsNullOrEmpty(relationship.Table))
                throw new ArgumentNullException(
                    String.Format("Class {0} does not have a table name on it's many to many relation to class {1}",
                                  relationship.Target.Name, relationship.Source.Name));
            if (String.IsNullOrEmpty(relationship.TargetColumn))
                throw new ArgumentNullException(
                    String.Format("Class {0} does not have a target column name on it's many to many relation to class {1}",
                                  relationship.Target.Name, relationship.Source.Name));
            if (String.IsNullOrEmpty(relationship.SourceColumn))
                throw new ArgumentNullException(
                    String.Format("Class {0} does not have a source column name on it's many to many relation to class {1}",
                                  relationship.Target.Name, relationship.Source.Name));

            CodeTypeDeclaration sourceClass = GenerateClass(relationship.Source, nameSpace);

            string propertyName = String.IsNullOrEmpty(relationship.TargetPropertyName)
                          ? Common.GetPlural(relationship.Target.Name)
                          : Common.GetPlural(relationship.TargetPropertyName);

            string propertyType = String.IsNullOrEmpty(relationship.SourcePropertyType)
                                      ? "IList"
                                      : relationship.SourcePropertyType;

            CodeMemberField memberField;
            if (!relationship.Source.Model.UseGenerics)
                memberField = GetMemberField(propertyName, propertyType, Accessor.Private, relationship.SourceAccess);
            else
                memberField = GetGenericMemberField(sourceClass.Name, propertyName, propertyType, Accessor.Private, relationship.SourceAccess);

            classDeclaration.Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.TargetDescription))
                memberProperty = GetMemberProperty(memberField, Common.GetPlural(propertyName), true, true, null);
            else
                memberProperty =
                    GetMemberProperty(memberField, Common.GetPlural(propertyName), true, true,
                                      relationship.TargetDescription);
            classDeclaration.Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(GetHasAndBelongsToAttribute(relationship, false));
        }

        #endregion

        #region OneToOne

        private void GenerateOneToOneRelationFromTarget(CodeTypeDeclaration classDeclaration, CodeNamespace nameSpace,
                                              OneToOneRelation relationship)
        {
            CodeTypeDeclaration targetClass = GenerateClass(relationship.Target, nameSpace);

            CodeMemberField memberField = GetMemberField(targetClass.Name, targetClass.Name, Accessor.Private, relationship.TargetAccess);
            classDeclaration.Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.SourceDescription))
                memberProperty = GetMemberProperty(memberField, targetClass.Name, true, true, null);
            else
                memberProperty =
                    GetMemberProperty(memberField, targetClass.Name, true, true,
                                      relationship.SourceDescription);
            classDeclaration.Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(GetOneToOneAttribute(relationship, true));
        }

        private void GenerateOneToOneRelationFromSources(CodeTypeDeclaration classDeclaration, CodeNamespace nameSpace,
                                               OneToOneRelation relationship)
        {
            CodeTypeDeclaration sourceClass = GenerateClass(relationship.Source, nameSpace);

            CodeMemberField memberField = GetMemberField(sourceClass.Name, sourceClass.Name, Accessor.Private, relationship.SourceAccess);
            classDeclaration.Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.TargetDescription))
                memberProperty = GetMemberProperty(memberField, sourceClass.Name, true, true, null);
            else
                memberProperty =
                    GetMemberProperty(memberField, sourceClass.Name, true, true,
                                      relationship.TargetDescription);
            classDeclaration.Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(GetOneToOneAttribute(relationship, false));
        }

        #endregion

        #endregion

        #region Attribute

        #region Helper

        private CodeAttributeArgument GetStringArrayAttributeArgument(string[] values)
        {
            CodeExpression[] exceptions = new CodeExpression[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                exceptions[i] = new CodePrimitiveExpression(values[i]);
            }

            return new CodeAttributeArgument(new CodeArrayCreateExpression(typeof(string), exceptions));
        }

        private CodeAttributeArgument GetPrimitiveAttributeArgument(string name, string value)
        {
            // TODO: Does this support VB.Net?
            return new CodeAttributeArgument(new CodeSnippetExpression(String.Format("\"{0} = {{1}}\"", name, value)));
        }

        private CodeAttributeArgument GetPrimitiveAttributeArgument(object o)
        {
            return new CodeAttributeArgument(new CodePrimitiveExpression(o));
        }

        private CodeAttributeArgument GetPrimitiveTypeAttributeArgument(string type)
        {
            return new CodeAttributeArgument(new CodeTypeOfExpression(type));
        }

        private CodeAttributeArgument GetNamedAttributeArgument(string name, object value)
        {
            return new CodeAttributeArgument(name, new CodePrimitiveExpression(value));
        }

        private CodeAttributeArgument GetNamedEnumAttributeArgument(string name, string type, Enum value)
        {
            return
                new CodeAttributeArgument(name,
                                          new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(type),
                                                                           value.ToString()));
        }

        private CodeAttributeArgument GetPrimitiveEnumAttributeArgument(string type, Enum value)
        {
            return
                new CodeAttributeArgument(
                    new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(type), value.ToString()));
        }

        private CodeAttributeArgument GetNamedTypeAttributeArgument(string name, string type)
        {
            return new CodeAttributeArgument(name, new CodeTypeOfExpression(type));
        }

        #endregion

        #region Validation

        private CodeAttributeDeclaration[] GetValidationAttributes(ModelProperty property)
        {
            ArrayList list = property.GetValidatorsAsArrayList();
            if (list != null && list.Count > 0)
            {
                CodeAttributeDeclaration[] result = new CodeAttributeDeclaration[list.Count];

                for (int i = 0; i < list.Count; i++)
                {
                    Type type = list[i].GetType();
                    if (type == typeof(ValidateConfirmation))
                        result[i] = GetConfirmationAttribute((ValidateConfirmation)list[i]);
                    else if (type == typeof(ValidateCreditCard))
                        result[i] = GetCreditCardAttribute((ValidateCreditCard)list[i]);
                    else if (type == typeof(ValidateEmail))
                        result[i] = GetEmailAttribute((ValidateEmail)list[i]);
                    else if (type == typeof(ValidateIsUnique))
                        result[i] = GetIsUniqueAttribute((ValidateIsUnique)list[i]);
                    else if (type == typeof(ValidateLength))
                        result[i] = GetLengthAttribute((ValidateLength)list[i]);
                    else if (type == typeof(ValidateNotEmpty))
                        result[i] = GetNotEmptyAttribute((ValidateNotEmpty)list[i]);
                    else if (type == typeof(ValidateRegExp))
                        result[i] = GetRegularExpressionAttribute((ValidateRegExp)list[i]);
                }

                return result;
            }

            return null;
        }

        private CodeAttributeDeclaration GetRegularExpressionAttribute(ValidateRegExp validator)
        {
            // No default constructor. Must have the pattern property set.
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ValidateRegExp");

            attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.Pattern));

            if (!string.IsNullOrEmpty(validator.ErrorMessage))
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.ErrorMessage));

            return attribute;
        }

        private CodeAttributeDeclaration GetNotEmptyAttribute(ValidateNotEmpty validator)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ValidateNotEmpty");

            if (!string.IsNullOrEmpty(validator.ErrorMessage))
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.ErrorMessage));

            return attribute;
        }

        private CodeAttributeDeclaration GetLengthAttribute(ValidateLength validator)
        {
            // Order should match one of the constructors.
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ValidateLength");

            if (validator.ExactLength != int.MinValue)
            {
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.ExactLength));

                if (!string.IsNullOrEmpty(validator.ErrorMessage))
                    attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.ErrorMessage));
            }
            else if (validator.MinLength != int.MinValue || validator.MaxLenght != int.MaxValue)
            {
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.MinLength));
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.MaxLenght));

                if (!string.IsNullOrEmpty(validator.ErrorMessage))
                    attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.ErrorMessage));
            }
            else if (!string.IsNullOrEmpty(validator.ErrorMessage))
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.ErrorMessage));

            return attribute;
        }

        private CodeAttributeDeclaration GetIsUniqueAttribute(ValidateIsUnique validator)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ValidateIsUnique");

            if (!string.IsNullOrEmpty(validator.ErrorMessage))
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.ErrorMessage));

            return attribute;
        }

        private CodeAttributeDeclaration GetEmailAttribute(ValidateEmail validator)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ValidateEmail");

            if (!string.IsNullOrEmpty(validator.ErrorMessage))
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.ErrorMessage));

            return attribute;
        }

        private CodeAttributeDeclaration GetCreditCardAttribute(ValidateCreditCard validator)
        {
            // Order should match one of the constructors.
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ValidateCreditCard");

            if (validator.AllowedTypes != CardType.All)
            {
                ArrayList list = new ArrayList();

                if ((validator.AllowedTypes & CardType.MasterCard) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.MasterCard));
                if ((validator.AllowedTypes & CardType.VISA) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.VISA));
                if ((validator.AllowedTypes & CardType.Amex) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.Amex));
                if ((validator.AllowedTypes & CardType.DinersClub) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.DinersClub));
                if ((validator.AllowedTypes & CardType.enRoute) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.enRoute));
                if ((validator.AllowedTypes & CardType.Discover) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.Discover));
                if ((validator.AllowedTypes & CardType.JCB) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.JCB));
                if ((validator.AllowedTypes & CardType.Unknown) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.Unknown));

                attribute.Arguments.Add(new CodeAttributeArgument(GetBinaryOr(list, 0)));

                if (validator.Exceptions != null)
                {
                    // TODO: Add as array initializer 
                    attribute.Arguments.Add(GetStringArrayAttributeArgument(validator.Exceptions));

                }
                if (!string.IsNullOrEmpty(validator.ErrorMessage))
                    attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.ErrorMessage));
            }
            else if (validator.Exceptions != null)
            {
                // TODO add as array init. as above :
                //attribute.Arguments.Add(GetPrimitiveAttributeArgument("CreditCardValidator.CardType", validator.Exceptions));

                if (!string.IsNullOrEmpty(validator.ErrorMessage))
                    attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.ErrorMessage));
            }
            else if (!string.IsNullOrEmpty(validator.ErrorMessage))
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.ErrorMessage));

            return attribute;
        }

        private CodeFieldReferenceExpression GetFieldReferenceForCreditCardEnum(Enum value)
        {
            return new CodeFieldReferenceExpression(
                new CodeTypeReferenceExpression("Castle.ActiveRecord.Framework.Validators.CreditCardValidator.CardType"),
                value.ToString());
        }

        private CodeAttributeDeclaration GetConfirmationAttribute(ValidateConfirmation validator)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ValidateConfirmation");

            attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.ConfirmationFieldOrProperty));
            if (!string.IsNullOrEmpty(validator.ErrorMessage))
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(validator.ErrorMessage));

            return attribute;
        }

        #endregion

        #region GeneratedCode

        private CodeAttributeDeclaration GetGeneratedCodeAttribute()
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("System.CodeDom.Compiler.GeneratedCodeAttribute");

            attribute.Arguments.Add(GetPrimitiveAttributeArgument("Altinoren.ActiveWriter.CustomTool.ActiveWriterTemplatedCodeGenerator"));
            attribute.Arguments.Add(GetPrimitiveAttributeArgument("1.0.0.0")); // TODO: Hardcoded to avoid circular dependency

            return attribute;
        }

        #endregion

        # region DebuggerDisplay

        private CodeAttributeDeclaration GetDebuggerDisplayAttribute(CodeTypeDeclaration classDeclaration, ModelProperty property)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("System.Diagnostics.DebuggerDisplay");

            attribute.Arguments.Add(GetPrimitiveAttributeArgument(property.Name, property.Name));

            return attribute;
        }

        # endregion

        #region ActiveRecord

        private CodeAttributeDeclaration GetActiveRecordAttribute(ModelClass modelClass)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ActiveRecord");

            if (!String.IsNullOrEmpty(modelClass.Table))
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(modelClass.Table));
            if (modelClass.Cache != CacheEnum.Undefined)
                attribute.Arguments.Add(GetNamedEnumAttributeArgument("Cache", "CacheEnum", modelClass.Cache));
            if (!String.IsNullOrEmpty(modelClass.CustomAccess))
                attribute.Arguments.Add(GetNamedAttributeArgument("CustomAccess", modelClass.CustomAccess));
            if (!String.IsNullOrEmpty(modelClass.DiscriminatorColumn))
                attribute.Arguments.Add(GetNamedAttributeArgument("DiscriminatorColumn", modelClass.DiscriminatorColumn));
            if (!String.IsNullOrEmpty(modelClass.DiscriminatorType))
                attribute.Arguments.Add(GetNamedAttributeArgument("DiscriminatorType", modelClass.DiscriminatorType));
            if (!String.IsNullOrEmpty(modelClass.DiscriminatorValue))
                attribute.Arguments.Add(GetNamedAttributeArgument("DiscriminatorValue", modelClass.DiscriminatorValue));
            if (modelClass.Lazy)
                attribute.Arguments.Add(GetNamedAttributeArgument("Lazy", modelClass.Lazy));
            if (!String.IsNullOrEmpty(modelClass.Proxy))
                attribute.Arguments.Add(GetNamedTypeAttributeArgument("Proxy", modelClass.Proxy));
            if (!String.IsNullOrEmpty(modelClass.Schema))
                attribute.Arguments.Add(GetNamedAttributeArgument("Schema", modelClass.Schema));
            if (!String.IsNullOrEmpty(modelClass.Where))
                attribute.Arguments.Add(GetNamedAttributeArgument("Where", modelClass.Where));
            if (modelClass.DynamicInsert)
                attribute.Arguments.Add(GetNamedAttributeArgument("DynamicInsert", modelClass.DynamicInsert));
            if (modelClass.DynamicUpdate)
                attribute.Arguments.Add(GetNamedAttributeArgument("DynamicUpdate", modelClass.DynamicUpdate));
            if (!String.IsNullOrEmpty(modelClass.Persister))
                attribute.Arguments.Add(GetNamedTypeAttributeArgument("Persister", modelClass.Persister));
            if (modelClass.SelectBeforeUpdate)
                attribute.Arguments.Add(GetNamedAttributeArgument("SelectBeforeUpdate", modelClass.SelectBeforeUpdate));
            if (modelClass.Polymorphism != Polymorphism.Implicit)
                attribute.Arguments.Add(GetNamedEnumAttributeArgument("Polymorphism", "Polymorphism", modelClass.Polymorphism));
            if (!modelClass.Mutable)
                attribute.Arguments.Add(GetNamedAttributeArgument("Mutable", modelClass.Mutable));
            if (modelClass.BatchSize != 1)
                attribute.Arguments.Add(GetNamedAttributeArgument("BatchSize", modelClass.BatchSize));
            if (modelClass.Locking != OptimisticLocking.Version)
                attribute.Arguments.Add(GetNamedEnumAttributeArgument("Locking", "OptimisticLocking", modelClass.Locking));
            if (!modelClass.UseAutoImport)
                attribute.Arguments.Add(GetNamedAttributeArgument("UseAutoImport", modelClass.UseAutoImport));

            return attribute;
        }

        #endregion

        #region Property

        private CodeAttributeDeclaration GetFieldAttribute(ModelProperty property)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("Field");

            PopulatePropertyOrFieldAttribute(property, attribute);

            return attribute;
        }

        private CodeAttributeDeclaration GetKeyPropertyAttribute(ModelProperty property)
        {
            // Why KeyPropertyAttribute doesn't have the same constructor signature as it's base class PropertyAttribute?
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("KeyProperty");

            PopulateKeyPropertyAttribute(property, attribute);

            return attribute;
        }

        private CodeAttributeDeclaration GetPropertyAttribute(ModelProperty property)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("Property");

            PopulatePropertyOrFieldAttribute(property, attribute);

            return attribute;
        }

        private CodeAttributeDeclaration GetVersionAttribute(ModelProperty property)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("Version");

            PopulateVersionAttribute(property, attribute);

            return attribute;
        }

        private CodeAttributeDeclaration GetTimestampAttribute(ModelProperty property)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("Timestamp");

            PopulateTimestampAttribute(property, attribute);

            return attribute;
        }

        private void PopulateKeyPropertyAttribute(ModelProperty property, CodeAttributeDeclaration attribute)
        {
            if (!String.IsNullOrEmpty(property.Column))
                attribute.Arguments.Add(GetNamedAttributeArgument("Column", property.Column));
            attribute.Arguments.Add(GetNamedAttributeArgument("ColumnType", property.ColumnType.ToString()));
            if (property.Access != PropertyAccess.Property)
                attribute.Arguments.Add(GetNamedEnumAttributeArgument("Access", "PropertyAccess", property.Access));
            if (!String.IsNullOrEmpty(property.CustomAccess))
                attribute.Arguments.Add(GetNamedAttributeArgument("CustomAccess", property.CustomAccess));
            if (!String.IsNullOrEmpty(property.Formula))
                attribute.Arguments.Add(GetNamedAttributeArgument("Formula", property.Formula));
            if (!property.Insert)
                attribute.Arguments.Add(GetNamedAttributeArgument("Insert", property.Insert));
            if (property.Length > 0)
                attribute.Arguments.Add(GetNamedAttributeArgument("Length", property.Length));
            if (property.NotNull)
                attribute.Arguments.Add(GetNamedAttributeArgument("NotNull", property.NotNull));
            if (property.Unique)
                attribute.Arguments.Add(GetNamedAttributeArgument("Unique", property.Unique));
            if (!String.IsNullOrEmpty(property.UnsavedValue))
                attribute.Arguments.Add(GetNamedAttributeArgument("UnsavedValue", property.UnsavedValue));
            if (!property.Update)
                attribute.Arguments.Add(GetNamedAttributeArgument("Update", property.Update));
        }

        private void PopulatePropertyOrFieldAttribute(ModelProperty property, CodeAttributeDeclaration attribute)
        {
            if (!String.IsNullOrEmpty(property.Column))
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(property.Column));
            attribute.Arguments.Add(GetNamedAttributeArgument("ColumnType", property.ColumnType.ToString()));
            if (property.Access != PropertyAccess.Property)
                attribute.Arguments.Add(GetNamedEnumAttributeArgument("Access", "PropertyAccess", property.Access));
            if (!String.IsNullOrEmpty(property.CustomAccess))
                attribute.Arguments.Add(GetNamedAttributeArgument("CustomAccess", property.CustomAccess));
            if (!String.IsNullOrEmpty(property.Formula))
                attribute.Arguments.Add(GetNamedAttributeArgument("Formula", property.Formula));
            if (!property.Insert)
                attribute.Arguments.Add(GetNamedAttributeArgument("Insert", property.Insert));
            if (property.Length > 0)
                attribute.Arguments.Add(GetNamedAttributeArgument("Length", property.Length));
            if (property.NotNull)
                attribute.Arguments.Add(GetNamedAttributeArgument("NotNull", property.NotNull));
            if (property.Unique)
                attribute.Arguments.Add(GetNamedAttributeArgument("Unique", property.Unique));
            if (!property.Update)
                attribute.Arguments.Add(GetNamedAttributeArgument("Update", property.Update));
            if (!String.IsNullOrEmpty(property.UniqueKey))
                attribute.Arguments.Add(GetNamedAttributeArgument("UniqueKey", property.UniqueKey));
            if (!String.IsNullOrEmpty(property.Index))
                attribute.Arguments.Add(GetNamedAttributeArgument("Index", property.Index));
            if (!String.IsNullOrEmpty(property.SqlType))
                attribute.Arguments.Add(GetNamedAttributeArgument("SqlType", property.SqlType));
            if (!String.IsNullOrEmpty(property.Check))
                attribute.Arguments.Add(GetNamedAttributeArgument("Check", property.Check));
        }

        private void PopulateVersionAttribute(ModelProperty property, CodeAttributeDeclaration attribute)
        {
            if (!String.IsNullOrEmpty(property.Column))
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(property.Column));
            attribute.Arguments.Add(GetNamedAttributeArgument("Type", property.ColumnType.ToString()));
            if (property.Access != PropertyAccess.Property)
                attribute.Arguments.Add(GetNamedEnumAttributeArgument("Access", "PropertyAccess", property.Access));
            if (!String.IsNullOrEmpty(property.CustomAccess))
                attribute.Arguments.Add(GetNamedAttributeArgument("CustomAccess", property.CustomAccess));
        }

        private void PopulateTimestampAttribute(ModelProperty property, CodeAttributeDeclaration attribute)
        {
            if (!String.IsNullOrEmpty(property.Column))
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(property.Column));
            if (property.Access != PropertyAccess.Property)
                attribute.Arguments.Add(GetNamedEnumAttributeArgument("Access", "PropertyAccess", property.Access));
            if (!String.IsNullOrEmpty(property.CustomAccess))
                attribute.Arguments.Add(GetNamedAttributeArgument("CustomAccess", property.CustomAccess));
        }

        #endregion

        #region Primary Key

        private CodeAttributeDeclaration GetPrimaryKeyAttribute(ModelProperty property)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("PrimaryKey");

            attribute.Arguments.Add(GetPrimitiveEnumAttributeArgument("PrimaryKeyType", property.Generator));
            if (!String.IsNullOrEmpty(property.Column))
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(property.Column));
            attribute.Arguments.Add(GetNamedAttributeArgument("ColumnType", property.ColumnType.ToString()));
            if (property.Access != PropertyAccess.Property)
                attribute.Arguments.Add(GetNamedEnumAttributeArgument("Access", "PropertyAccess", property.Access));
            if (!String.IsNullOrEmpty(property.CustomAccess))
                attribute.Arguments.Add(GetNamedAttributeArgument("CustomAccess", property.CustomAccess));
            if (property.Length > 0)
                attribute.Arguments.Add(GetNamedAttributeArgument("Length", property.Length));
            if (!String.IsNullOrEmpty(property.Params))
                attribute.Arguments.Add(GetNamedAttributeArgument("Params", property.Params));
            if (!String.IsNullOrEmpty(property.SequenceName))
                attribute.Arguments.Add(GetNamedAttributeArgument("SequenceName", property.SequenceName));
            if (!String.IsNullOrEmpty(property.UnsavedValue))
                attribute.Arguments.Add(GetNamedAttributeArgument("UnsavedValue", property.UnsavedValue));

            return attribute;
        }

        #endregion

        #region HasMany

        private CodeAttributeDeclaration GetHasManyAttribute(ManyToOneRelation relation)
        {
            if (!_classDeclarations.ContainsKey(relation.Source))
                throw new ArgumentException(
                    "Cannot create HasMany attribute. Cannot find code type decleration for source class.");

            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("HasMany");

            attribute.Arguments.Add(GetPrimitiveTypeAttributeArgument(_classDeclarations[relation.Source].Name));
            if (relation.TargetAccess != PropertyAccess.Property)
                attribute.Arguments.Add(GetNamedEnumAttributeArgument("Access", "PropertyAccess", relation.TargetAccess));
            if (relation.TargetCache != CacheEnum.Undefined)
                attribute.Arguments.Add(GetNamedEnumAttributeArgument("Cache", "CacheEnum", relation.TargetCache));
            if (relation.TargetCascade != ManyRelationCascadeEnum.None)
                attribute.Arguments.Add(GetNamedEnumAttributeArgument("Cascade", "ManyRelationCascadeEnum", relation.TargetCascade));
            if (!String.IsNullOrEmpty(relation.TargetColumnKey))
                attribute.Arguments.Add(GetNamedAttributeArgument("ColumnKey", relation.TargetColumnKey));
            if (!String.IsNullOrEmpty(relation.TargetCustomAccess))
                attribute.Arguments.Add(GetNamedAttributeArgument("CustomAccess", relation.TargetCustomAccess));
            if (relation.TargetRelationType == RelationType.Map)
            {
                // TODO: Index & IndexType ?
            }
            if (relation.TargetInverse)
                attribute.Arguments.Add(GetNamedAttributeArgument("Inverse", relation.TargetInverse));
            if (relation.TargetLazy)
                attribute.Arguments.Add(GetNamedAttributeArgument("Lazy", relation.TargetLazy));
            if (!String.IsNullOrEmpty(relation.TargetMapType))
                attribute.Arguments.Add(GetNamedTypeAttributeArgument("MapType", relation.TargetMapType));
            if (!String.IsNullOrEmpty(relation.TargetOrderBy))
                attribute.Arguments.Add(GetNamedAttributeArgument("OrderBy", relation.TargetOrderBy));
            if (relation.TargetRelationType != RelationType.Guess)
                attribute.Arguments.Add(
                    GetNamedEnumAttributeArgument("RelationType", "RelationType", relation.TargetRelationType));
            if (!String.IsNullOrEmpty(relation.TargetSchema))
                attribute.Arguments.Add(GetNamedAttributeArgument("Schema", relation.TargetSchema));
            if (relation.TargetRelationType == RelationType.Set && !String.IsNullOrEmpty(relation.TargetSort))
                attribute.Arguments.Add(GetNamedAttributeArgument("Sort", relation.TargetSort));
            if (!String.IsNullOrEmpty(relation.TargetTable))
                attribute.Arguments.Add(GetNamedAttributeArgument("Table", relation.TargetTable));
            if (!String.IsNullOrEmpty(relation.TargetWhere))
                attribute.Arguments.Add(GetNamedAttributeArgument("Where", relation.TargetWhere));
            if (relation.TargetNotFoundBehaviour != NotFoundBehaviour.Default)
                attribute.Arguments.Add(
                    GetNamedEnumAttributeArgument("NotFoundBehaviour", "NotFoundBehaviour",
                                                  relation.TargetNotFoundBehaviour));
            if (!String.IsNullOrEmpty(relation.TargetIndexType))
                attribute.Arguments.Add(GetNamedAttributeArgument("IndexType", relation.TargetIndexType));
            if (!String.IsNullOrEmpty(relation.TargetIndex))
                attribute.Arguments.Add(GetNamedAttributeArgument("Index", relation.TargetIndex));
            if (!String.IsNullOrEmpty(relation.TargetElement))
                attribute.Arguments.Add(GetNamedAttributeArgument("Element", relation.TargetElement));

            return attribute;
        }

        #endregion

        #region BelongsTo

        private CodeAttributeDeclaration GetBelongsToAttribute(ManyToOneRelation relation)
        {
            if (!_classDeclarations.ContainsKey(relation.Target))
                throw new ArgumentException(
                    "Cannot create BelongsTo attribute. Cannot find code type declaration for target class.");

            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("BelongsTo");
            if (!string.IsNullOrEmpty(relation.SourceColumn))
                attribute.Arguments.Add(GetPrimitiveAttributeArgument(relation.SourceColumn));
            if (relation.SourceCascade != CascadeEnum.None)
                attribute.Arguments.Add(GetNamedEnumAttributeArgument("Cascade", "CascadeEnum", relation.SourceCascade));
            if (!String.IsNullOrEmpty(relation.SourceCustomAccess))
                attribute.Arguments.Add(GetNamedAttributeArgument("CustomAccess", relation.SourceCustomAccess));
            if (!relation.SourceInsert)
                attribute.Arguments.Add(GetNamedAttributeArgument("Insert", relation.SourceInsert));
            if (!relation.SourceNotNull)
                attribute.Arguments.Add(GetNamedAttributeArgument("NotNull", relation.SourceNotNull));
            if (relation.SourceOuterJoin != OuterJoinEnum.Auto)
                attribute.Arguments.Add(
                    GetNamedEnumAttributeArgument("OuterJoin", "OuterJoinEnum", relation.SourceOuterJoin));
            if (!String.IsNullOrEmpty(relation.SourceType))
                attribute.Arguments.Add(GetNamedTypeAttributeArgument("Type", relation.SourceType));
            if (relation.SourceUnique)
                attribute.Arguments.Add(GetNamedAttributeArgument("Unique", relation.SourceUnique));
            if (!relation.SourceUpdate)
                attribute.Arguments.Add(GetNamedAttributeArgument("Update", relation.SourceUpdate));
            if (relation.SourceNotFoundBehaviour != NotFoundBehaviour.Default)
                attribute.Arguments.Add(GetNamedEnumAttributeArgument("NotFoundBehaviour", "NotFoundBehaviour", relation.SourceNotFoundBehaviour));

            return attribute;
        }

        #endregion

        #region HasAndBelongTo

        private CodeAttributeDeclaration GetHasAndBelongsToAttribute(ManyToManyRelation relation, bool useSource)
        {
            CodeTypeDeclaration otherPart;

            if (useSource)
                otherPart = _classDeclarations[relation.Target];
            else
                otherPart = _classDeclarations[relation.Source];

            if (otherPart == null)
                throw new ArgumentNullException(
                    "Cannot create HasAndBelongsToMany attribute. Cannot find code type declaration for the other part of the relationship.", (useSource ? relation.Source.Name : relation.Target.Name));

            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("HasAndBelongsToMany");

            if (useSource)
            {
                attribute.Arguments.Add(GetPrimitiveTypeAttributeArgument(relation.Target.Name));
                if (relation.SourceAccess != PropertyAccess.Property)
                    attribute.Arguments.Add(GetNamedEnumAttributeArgument("Access", "PropertyAccess", relation.SourceAccess));
                if (relation.SourceCache != CacheEnum.Undefined)
                    attribute.Arguments.Add(GetNamedEnumAttributeArgument("Cache", "CacheEnum", relation.SourceCache));
                if (relation.SourceCascade != ManyRelationCascadeEnum.None)
                    attribute.Arguments.Add(GetNamedEnumAttributeArgument("Cascade", "ManyRelationCascadeEnum", relation.SourceCascade));
                if (!String.IsNullOrEmpty(relation.SourceCustomAccess))
                    attribute.Arguments.Add(GetNamedAttributeArgument("CustomAccess", relation.SourceCustomAccess));

                attribute.Arguments.Add(GetNamedAttributeArgument("ColumnRef", relation.TargetColumn));
                attribute.Arguments.Add(GetNamedAttributeArgument("ColumnKey", relation.SourceColumn));
                if (relation.TargetRelationType == RelationType.Map)
                {
                    // TODO: Index & IndexType
                }
                if (relation.SourceInverse)
                    attribute.Arguments.Add(GetNamedAttributeArgument("Inverse", relation.SourceInverse));
                if (relation.SourceLazy)
                    attribute.Arguments.Add(GetNamedAttributeArgument("Lazy", relation.SourceLazy));
                if (!String.IsNullOrEmpty(relation.SourceMapType))
                    attribute.Arguments.Add(GetNamedTypeAttributeArgument("MapType", relation.SourceMapType));
                if (!String.IsNullOrEmpty(relation.SourceOrderBy))
                    attribute.Arguments.Add(GetNamedAttributeArgument("OrderBy", relation.SourceOrderBy));
                if (relation.SourceRelationType != RelationType.Guess)
                    attribute.Arguments.Add(
                        GetNamedEnumAttributeArgument("RelationType", "RelationType", relation.SourceRelationType));
                if (relation.SourceRelationType == RelationType.Set && !String.IsNullOrEmpty(relation.SourceSort))
                    attribute.Arguments.Add(GetNamedAttributeArgument("Sort", relation.SourceSort));
                if (!String.IsNullOrEmpty(relation.SourceWhere))
                    attribute.Arguments.Add(GetNamedAttributeArgument("Where", relation.SourceWhere));
                if (relation.SourceNotFoundBehaviour != NotFoundBehaviour.Default)
                    attribute.Arguments.Add(
                        GetNamedEnumAttributeArgument("NotFoundBehaviour", "NotFoundBehaviour",
                                                      relation.SourceNotFoundBehaviour));
            }
            else
            {
                attribute.Arguments.Add(GetPrimitiveTypeAttributeArgument(relation.Source.Name));
                if (relation.TargetAccess != PropertyAccess.Property)
                    attribute.Arguments.Add(GetNamedEnumAttributeArgument("Access", "PropertyAccess", relation.TargetAccess));
                if (relation.TargetCache != CacheEnum.Undefined)
                    attribute.Arguments.Add(GetNamedEnumAttributeArgument("Cache", "CacheEnum", relation.TargetCache));
                if (relation.TargetCascade != ManyRelationCascadeEnum.None)
                    attribute.Arguments.Add(GetNamedEnumAttributeArgument("Cascade", "ManyRelationCascadeEnum", relation.TargetCascade));
                if (!String.IsNullOrEmpty(relation.TargetCustomAccess))
                    attribute.Arguments.Add(GetNamedAttributeArgument("CustomAccess", relation.TargetCustomAccess));

                attribute.Arguments.Add(GetNamedAttributeArgument("ColumnRef", relation.SourceColumn));
                attribute.Arguments.Add(GetNamedAttributeArgument("ColumnKey", relation.TargetColumn));
                if (relation.TargetRelationType == RelationType.Map)
                {
                    // TODO: Index & IndexType
                }
                if (relation.TargetInverse)
                    attribute.Arguments.Add(GetNamedAttributeArgument("Inverse", relation.TargetInverse));
                if (relation.TargetLazy)
                    attribute.Arguments.Add(GetNamedAttributeArgument("Lazy", relation.TargetLazy));
                if (!String.IsNullOrEmpty(relation.TargetMapType))
                    attribute.Arguments.Add(GetNamedTypeAttributeArgument("MapType", relation.TargetMapType));
                if (!String.IsNullOrEmpty(relation.TargetOrderBy))
                    attribute.Arguments.Add(GetNamedAttributeArgument("OrderBy", relation.TargetOrderBy));
                if (relation.TargetRelationType != RelationType.Guess)
                    attribute.Arguments.Add(
                        GetNamedEnumAttributeArgument("RelationType", "RelationType", relation.TargetRelationType));
                if (relation.TargetRelationType == RelationType.Set && !String.IsNullOrEmpty(relation.TargetSort))
                    attribute.Arguments.Add(GetNamedAttributeArgument("Sort", relation.TargetSort));
                if (!String.IsNullOrEmpty(relation.TargetWhere))
                    attribute.Arguments.Add(GetNamedAttributeArgument("Where", relation.TargetWhere));
                if (relation.TargetNotFoundBehaviour != NotFoundBehaviour.Default)
                    attribute.Arguments.Add(
                        GetNamedEnumAttributeArgument("NotFoundBehaviour", "NotFoundBehaviour",
                                                      relation.TargetNotFoundBehaviour));
            }

            if (!String.IsNullOrEmpty(relation.Schema))
                attribute.Arguments.Add(GetNamedAttributeArgument("Schema", relation.Schema));
            if (!String.IsNullOrEmpty(relation.Table))
                attribute.Arguments.Add(GetNamedAttributeArgument("Table", relation.Table));

            return attribute;
        }

        #endregion

        #region OneToOne

        private CodeAttributeDeclaration GetOneToOneAttribute(OneToOneRelation relation, bool useSource)
        {
            CodeTypeDeclaration otherPart;

            if (useSource)
                otherPart = _classDeclarations[relation.Target];
            else
                otherPart = _classDeclarations[relation.Source];

            if (otherPart == null)
                throw new ArgumentNullException(
                    "Cannot create OneToOne attribute. Cannot find code type declaration for the other part of the relationship.", (useSource ? relation.Source.Name : relation.Target.Name));

            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("OneToOne");

            if (useSource)
            {
                if (relation.SourceAccess != PropertyAccess.Property)
                    attribute.Arguments.Add(GetNamedEnumAttributeArgument("Access", "PropertyAccess", relation.SourceAccess));
                if (relation.SourceCascade != CascadeEnum.None)
                    attribute.Arguments.Add(GetNamedEnumAttributeArgument("Cascade", "CascadeEnum", relation.SourceCascade));
                if (relation.SourceConstrained)
                    attribute.Arguments.Add(GetNamedAttributeArgument("Constrained", relation.SourceConstrained));
                if (!String.IsNullOrEmpty(relation.SourceCustomAccess))
                    attribute.Arguments.Add(GetNamedAttributeArgument("CustomAccess", relation.SourceCustomAccess));
                if (relation.SourceOuterJoin != OuterJoinEnum.Auto)
                    attribute.Arguments.Add(GetNamedEnumAttributeArgument("OuterJoin", "OuterJoinEnum", relation.SourceOuterJoin));
            }
            else
            {
                if (relation.TargetAccess != PropertyAccess.Property)
                    attribute.Arguments.Add(GetNamedEnumAttributeArgument("Access", "PropertyAccess", relation.TargetAccess));
                if (relation.TargetCascade != CascadeEnum.None)
                    attribute.Arguments.Add(GetNamedEnumAttributeArgument("Cascade", "CascadeEnum", relation.TargetCascade));
                if (relation.TargetConstrained)
                    attribute.Arguments.Add(GetNamedAttributeArgument("Constrained", relation.TargetConstrained));
                if (!String.IsNullOrEmpty(relation.TargetCustomAccess))
                    attribute.Arguments.Add(GetNamedAttributeArgument("CustomAccess", relation.TargetCustomAccess));
                if (relation.TargetOuterJoin != OuterJoinEnum.Auto)
                    attribute.Arguments.Add(GetNamedEnumAttributeArgument("OuterJoin", "OuterJoinEnum", relation.TargetOuterJoin));
            }

            return attribute;
        }

        #endregion

        #endregion

        #region Namespace

        private CodeNamespace GetDefaultNamespace()
        {
            CodeNamespace nameSpace = new CodeNamespace(_namespace);
            nameSpace.Imports.Add(new CodeNamespaceImport(Common.SystemNamespace));
            if (_model.UseGenerics)
                nameSpace.Imports.Add(new CodeNamespaceImport(Common.GenericCollectionsNamespace));
            else
                nameSpace.Imports.Add(new CodeNamespaceImport(Common.CollectionsNamespace));
            nameSpace.Imports.Add(new CodeNamespaceImport(Common.ActiveRecordNamespace));
            if (_model.UseNullables == NullableUsage.WithHelperLibrary)
                nameSpace.Imports.Add(new CodeNamespaceImport(Common.NullablesNamespace));
            if (_model.AdditionalImports != null && _model.AdditionalImports.Count > 0)
            {
                foreach (Import item in _model.AdditionalImports)
                {
                    if (!string.IsNullOrEmpty(item.Name))
                        nameSpace.Imports.Add(new CodeNamespaceImport(item.Name));
                }
            }

            return nameSpace;
        }

        #endregion

        #region Comment

        private CodeCommentStatementCollection GetSummaryComment(params string[] comments)
        {
            CodeCommentStatementCollection collection = new CodeCommentStatementCollection();
            if (comments != null && comments.Length > 0)
            {
                for (int i = 0; i < comments.Length; i++)
                    if (!String.IsNullOrEmpty(comments[i]))
                        collection.Add(new CodeCommentStatement(comments[i], true));
            }

            if (collection.Count == 0)
                return collection;

            collection.Insert(0, new CodeCommentStatement("<summary>", true));
            collection.Add(new CodeCommentStatement("</summary>", true));
            return collection;
        }

        #endregion

        #region Code Generation

        private CodeTypeDeclaration GenerateClass(ModelClass cls, CodeNamespace nameSpace)
        {
            if (cls == null)
                throw new ArgumentNullException("Class not supplied", "cls");
            if (nameSpace == null)
                throw new ArgumentNullException("Namespace not supplied", "namespace");
            if (String.IsNullOrEmpty(cls.Name))
                throw new ArgumentException("Class name cannot be blank", "cls");

            if (!_classDeclarations.ContainsKey(cls))
            {
                if (_generatedClassNames.Contains(cls.Name))
                    throw new ArgumentException(
                        "Ambiguous class name. Code for a class with the same name already generated. Please use a different name.",
                        cls.Name);

                CodeTypeDeclaration classDeclaration = GetClassDeclaration(cls, nameSpace);

                List<ModelProperty> compositeKeys = new List<ModelProperty>();

                // Properties and Fields
                foreach (ModelProperty property in cls.Properties)
                {
                    if (property.KeyType != KeyType.CompositeKey)
                    {
                        CodeMemberField memberField = null;

                        switch (property.PropertyType)
                        {
                            case PropertyType.Property:
                                memberField = GetMemberFieldOfProperty(property, Accessor.Private);
                                CodeMemberProperty memberProperty = GetActiveRecordMemberProperty(memberField, property);
                                classDeclaration.Members.Add(memberProperty);
                                if (property.IsValidatorSet())
                                    memberProperty.CustomAttributes.AddRange(GetValidationAttributes(property));
                                break;
                            case PropertyType.Field:
                                memberField = GetMemberFieldOfProperty(property, property.Accessor);
                                memberField.CustomAttributes.Add(GetFieldAttribute(property));
                                break;
                            case PropertyType.Version:
                                memberField = GetMemberFieldOfProperty(property, Accessor.Private);
                                classDeclaration.Members.Add(GetActiveRecordMemberVersion(memberField, property));
                                break;
                            case PropertyType.Timestamp:
                                memberField = GetMemberFieldOfProperty(property, Accessor.Private);
                                classDeclaration.Members.Add(GetActiveRecordMemberTimestamp(memberField, property));
                                break;
                        }

                        classDeclaration.Members.Add(memberField);

                        if (property.DebuggerDisplay)
                            classDeclaration.CustomAttributes.Add(GetDebuggerDisplayAttribute(classDeclaration, property));
                    }
                    else
                        compositeKeys.Add(property);
                }

                if (compositeKeys.Count > 0)
                {
                    CodeTypeDeclaration compositeClass =
                        GetCompositeClassDeclaration(nameSpace, classDeclaration, compositeKeys);

                    // TODO: All access fields in a composite group assumed to be the same.
                    // We have a model validator for this case but the user may save anyway.
                    // Check if all access fields are the same.
                    CodeMemberField memberField = GetPrivateMemberFieldOfCompositeClass(compositeClass, PropertyAccess.Property);
                    classDeclaration.Members.Add(memberField);

                    classDeclaration.Members.Add(GetActiveRecordMemberCompositeKeyProperty(compositeClass, memberField));
                }

                //ManyToOne links where this class is the target (1-n)
                ReadOnlyCollection<ManyToOneRelation> manyToOneSources = ManyToOneRelation.GetLinksToSources(cls);
                foreach (ManyToOneRelation relationship in manyToOneSources)
                {
                    GenerateHasManyRelation(classDeclaration, nameSpace, relationship);
                }

                //ManyToOne links where this class is the source (n-1)
                ReadOnlyCollection<ManyToOneRelation> manyToOneTargets = ManyToOneRelation.GetLinksToTargets(cls);
                foreach (ManyToOneRelation relationship in manyToOneTargets)
                {
                    GenerateBelongsToRelation(classDeclaration, nameSpace, relationship);
                }

                //ManyToMany links where this class is the source
                ReadOnlyCollection<ManyToManyRelation> manyToManyTargets = ManyToManyRelation.GetLinksToManyToManyTargets(cls);
                foreach (ManyToManyRelation relationship in manyToManyTargets)
                {
                    GenerateHasAndBelongsToRelationFromTargets(classDeclaration, nameSpace, relationship);
                }

                //ManyToMany links where this class is the target
                ReadOnlyCollection<ManyToManyRelation> manyToManySources = ManyToManyRelation.GetLinksToManyToManySources(cls);
                foreach (ManyToManyRelation relationship in manyToManySources)
                {
                    GenerateHasAndBelongsToRelationFromSources(classDeclaration, nameSpace, relationship);
                }

                //OneToOne link where this class is the source
                OneToOneRelation oneToOneTarget = OneToOneRelation.GetLinkToOneToOneTarget(cls);
                if (oneToOneTarget != null)
                {
                    GenerateOneToOneRelationFromTarget(classDeclaration, nameSpace, oneToOneTarget);
                }

                //OneToOne links where this class is the target
                ReadOnlyCollection<OneToOneRelation> oneToOneSources = OneToOneRelation.GetLinksToOneToOneSources(cls);
                foreach (OneToOneRelation relationship in oneToOneSources)
                {
                    GenerateOneToOneRelationFromSources(classDeclaration, nameSpace, relationship);
                }

                // TODO: Other relation types (any etc)

                return classDeclaration;
            }
            else
                return _classDeclarations[cls];
        }

        private bool IsNullable(NHibernateType type)
        {
            switch (type)
            {
                case NHibernateType.Int32:
                case NHibernateType.Boolean:
                case NHibernateType.TrueFalse:
                case NHibernateType.YesNo:
                case NHibernateType.DateTime:
                case NHibernateType.Ticks:
                case NHibernateType.Timestamp:
                case NHibernateType.Decimal:
                case NHibernateType.Double:
                case NHibernateType.Int16:
                case NHibernateType.Int64:
                case NHibernateType.Single:
                case NHibernateType.Byte:
                case NHibernateType.Guid:
                    return true;
                default:
                    return false;
            }
        }

        private CodeTypeReference GetNullableTypeReferenceForHelper(NHibernateType type)
        {
            switch (type)
            {
                case NHibernateType.Int32:
                    return new CodeTypeReference("NullableInt32");
                case NHibernateType.Boolean:
                case NHibernateType.TrueFalse:
                case NHibernateType.YesNo:
                    return new CodeTypeReference("NullableBoolean");
                case NHibernateType.DateTime:
                case NHibernateType.Ticks:
                case NHibernateType.Timestamp:
                    return new CodeTypeReference("NullableDateTime");
                case NHibernateType.Decimal:
                    return new CodeTypeReference("NullableDecimal");
                case NHibernateType.Double:
                    return new CodeTypeReference("NullableDouble");
                case NHibernateType.Int16:
                    return new CodeTypeReference("NullableInt16");
                case NHibernateType.Int64:
                    return new CodeTypeReference("NullableInt64");
                case NHibernateType.Single:
                    return new CodeTypeReference("NullableSingle");
                case NHibernateType.Byte:
                    return new CodeTypeReference("NullableByte");
                case NHibernateType.Guid:
                    return new CodeTypeReference("NullableGuid");
                default:
                    return new CodeTypeReference(GetSystemType(type));
            }
        }

        private CodeTypeReference GetNullableTypeReference(NHibernateType type)
        {
            switch (type)
            {
                case NHibernateType.Int32:
                case NHibernateType.Boolean:
                case NHibernateType.TrueFalse:
                case NHibernateType.YesNo:
                case NHibernateType.DateTime:
                case NHibernateType.Ticks:
                case NHibernateType.Timestamp:
                case NHibernateType.Decimal:
                case NHibernateType.Double:
                case NHibernateType.Int16:
                case NHibernateType.Int64:
                case NHibernateType.Single:
                case NHibernateType.Byte:
                case NHibernateType.Guid:
                    return GetNullableTypeReference(GetSystemType(type));
                default:
                    return new CodeTypeReference(GetSystemType(type));
            }
        }

        private CodeTypeReference GetNullableTypeReference(Type type)
        {
            CodeTypeReference reference = new CodeTypeReference("System.Nullable");
            reference.TypeArguments.Add(type);

            return reference;
        }

        private Type GetSystemType(NHibernateType type)
        {
            switch (type)
            {
                // TODO: Combine and order most likely asc
                case NHibernateType.AnsiChar:
                case NHibernateType.Char:
                    return typeof(string);
                case NHibernateType.Boolean:
                    return typeof(Boolean);
                case NHibernateType.Byte:
                    return typeof(Byte);
                case NHibernateType.DateTime:
                    return typeof(DateTime);
                case NHibernateType.Decimal:
                    return typeof(Decimal);
                case NHibernateType.Double:
                    return typeof(Double);
                case NHibernateType.Guid:
                    return typeof(Guid);
                case NHibernateType.Int16:
                    return typeof(Int16);
                case NHibernateType.Int32:
                    return typeof(Int32);
                case NHibernateType.Int64:
                    return typeof(Int64);
                case NHibernateType.Single:
                    return typeof(Single);
                case NHibernateType.Ticks:
                    return typeof(DateTime);
                case NHibernateType.TimeSpan:
                    return typeof(TimeSpan);
                case NHibernateType.Timestamp:
                    return typeof(DateTime);
                case NHibernateType.TrueFalse:
                    return typeof(Boolean);
                case NHibernateType.YesNo:
                    return typeof(Boolean);
                case NHibernateType.AnsiString:
                    return typeof(String);
                case NHibernateType.CultureInfo:
                    return typeof(CultureInfo);
                case NHibernateType.Binary:
                    return typeof(Byte[]);
                case NHibernateType.Type:
                    return typeof(Type);
                case NHibernateType.String:
                    return typeof(String);
                case NHibernateType.StringClob:
                    return typeof(String);
                case NHibernateType.BinaryBlob:
                    return typeof(Byte[]);
                default:
                    throw new ArgumentException("Unknown NHibernate type", type.ToString());
            }
        }

        private CodeExpression GetBooleanAnd(List<CodeExpression> expressions, int i)
        {
            if (i == expressions.Count - 2)
                return
                    new CodeBinaryOperatorExpression(expressions[i], CodeBinaryOperatorType.BooleanAnd,
                                                     expressions[i + 1]);
            else
                return
                    new CodeBinaryOperatorExpression(expressions[i], CodeBinaryOperatorType.BooleanAnd,
                                                     GetBooleanAnd(expressions, i + 1));
        }

        private CodeExpression GetBinaryOr(ArrayList list, int indexOfLeft)
        {
            if (indexOfLeft + 1 == list.Count)
                return (CodeExpression)list[0];
            else if (indexOfLeft + 2 == list.Count)
                return
                    new CodeBinaryOperatorExpression((CodeExpression)list[indexOfLeft],
                                                     CodeBinaryOperatorType.BitwiseOr,
                                                     (CodeExpression)list[indexOfLeft + 1]);
            else
                return
                    new CodeBinaryOperatorExpression((CodeExpression)list[indexOfLeft],
                                                     CodeBinaryOperatorType.BitwiseOr,
                                                     GetBinaryOr(list, indexOfLeft + 1));
        }

        private CodeExpression GetXor(List<CodeExpression> expressions, int i)
        {
            if (i == expressions.Count - 2)
                return new CodeMethodInvokeExpression(null, Common.XorHelperMethod, expressions[i], expressions[i + 1]);
            else
                return
                    new CodeMethodInvokeExpression(null, Common.XorHelperMethod, expressions[i],
                                                   GetXor(expressions, i + 1));
        }

        private string GetName(string name, PropertyAccess access)
        {
            switch (access)
            {
                case PropertyAccess.Property:
                    switch (_model.CaseOfPrivateFields)
                    {
                        case FieldCase.Unchanged:
                            return name;
                        case FieldCase.Camelcase:
                            return Common.MakeCamel(name);
                        case FieldCase.CamelcaseUnderscore:
                            return "_" + Common.MakeCamel(name);
                        case FieldCase.CamelcaseMUnderscore:
                            return "m_" + Common.MakeCamel(name);
                        case FieldCase.Pascalcase:
                            return Common.MakePascal(name);
                        case FieldCase.PascalcaseUnderscore:
                            return "_" + Common.MakePascal(name);
                        case FieldCase.PascalcaseMUnderscore:
                            return "m_" + Common.MakePascal(name);
                    }
                    break;
                case PropertyAccess.Field:
                    return name;
                case PropertyAccess.FieldCamelcase:
                case PropertyAccess.NosetterCamelcase:
                    return Common.MakeCamel(name);
                case PropertyAccess.FieldCamelcaseUnderscore:
                case PropertyAccess.NosetterCamelcaseUnderscore:
                    return "_" + Common.MakeCamel(name);
                case PropertyAccess.FieldPascalcaseMUnderscore:
                case PropertyAccess.NosetterPascalcaseMUnderscore:
                    return "m_" + Common.MakePascal(name);
                case PropertyAccess.FieldLowercaseUnderscore:
                case PropertyAccess.NosetterLowercaseUnderscore:
                    return "_" + name.ToLowerInvariant();
                case PropertyAccess.NosetterLowercase:
                    return name.ToLowerInvariant();
            }

            return name;
        }

        private string GenerateCode(CodeCompileUnit compileUnit)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                _provider.GenerateCodeFromCompileUnit(compileUnit, sw, new CodeGeneratorOptions());
            }

            return sb.ToString();
        }

        private Assembly GenerateARAssembly(CodeCompileUnit compileUnit)
        {
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.OutputAssembly = Common.InMemoryCompiledAssemblyName + ".dll";
            parameters.GenerateExecutable = false;

            Assembly activeRecord = Assembly.Load(_model.ActiveRecordAssemblyName);
            parameters.ReferencedAssemblies.Add(activeRecord.Location);
            Assembly nHibernate = Assembly.Load(_model.NHibernateAssemblyName);
            parameters.ReferencedAssemblies.Add(nHibernate.Location);
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("mscorlib.dll");

            CompilerResults results = _provider.CompileAssemblyFromDom(parameters, compileUnit);
            if (results.Errors.Count == 0)
            {
                return results.CompiledAssembly;
            }
            else
            {
                ArrayList list = new ArrayList();
                foreach (CompilerError error in results.Errors)
                {
                    list.Add(new Exception(error.ErrorText));
                }
                throw new ExceptionCollection(list);
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
                    IEnumerator enumerator = (IEnumerator)modelCollection.InvokeMember("GetEnumerator", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance, null, models, null);
                    while (enumerator.MoveNext())
                    {
                        Type visitor = _activeRecord.GetType("Castle.ActiveRecord.Framework.Internal.XmlGenerationVisitor");
                        object generationVisitor = Activator.CreateInstance(visitor);
                        visitor.InvokeMember("CreateXml", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null,
                                             generationVisitor, new object[] { enumerator.Current });

                        string xml = (string)visitor.GetProperty("Xml").GetValue(generationVisitor, null);
                        xml = xml.Replace(", " + Common.InMemoryCompiledAssemblyName, string.Empty); // Strips the assembly name from class names
                        XmlDocument document = new XmlDocument();
                        document.LoadXml(xml);
                        XmlNodeList nodeList = document.GetElementsByTagName("class");
                        if (nodeList.Count > 0)
                        {
                            string name = null;

                            foreach (XmlAttribute attribute in nodeList[0].Attributes)
                            {
                                if (attribute.Name == "name")
                                {
                                    name = attribute.Value;
                                    break;
                                }
                            }

                            if (name != null)
                                nHibernateConfigs.Add(name, xml);
                        }
                    }
                }
            }
        }

        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName name = new AssemblyName(args.Name);
            if (name.Name == "Castle.ActiveRecord" || name.Name == "Iesi.Collections" || name.Name == "log4net" || name.Name == "NHibernate")
            {
                // If this line is reached, these assemblies are not in GAC. Load from designated place.
                string assemblyPath = Path.Combine(_assemblyLoadPath, name.Name + ".dll");

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
                        if (Array.FindIndex(Common.ARAttributes, delegate(string name)
                        {
                            return name == attribute.Name;
                        }) > -1)
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
                            if (Array.FindIndex(Common.ARAttributes, delegate(string name)
                            {
                                return name == attribute.Name;
                            }) > -1)
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
            if (name.StartsWith(_namespace))
                return name.Remove(0, _namespace.Length + 1);

            return name;
        }

        #endregion

        #endregion
    }
}
