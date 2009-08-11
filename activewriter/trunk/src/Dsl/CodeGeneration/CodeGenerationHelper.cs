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

using Altinoren.ActiveWriter.CodeDomExtensions;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Visitors;

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
    using Microsoft.VisualBasic;
    using EnvDTE;
    using Microsoft.VisualStudio.Modeling;
    using ServerExplorerSupport;
    using VSLangProj;
    using CodeNamespace = System.CodeDom.CodeNamespace;

    public class CodeGenerationHelper
    {
        #region Private Variables

        private CodeGenerationContext Context { get; set; }

        private Dictionary<string, string> _nHibernateConfigs = new Dictionary<string, string>();
        private Assembly _activeRecord;

        #endregion

        #region ctors

        public CodeGenerationHelper(CodeGenerationContext context)
        {
            Context = context;
        }

        #endregion

        #region Public Methods

        public void Generate()
        {
            if (Context.Language == CodeLanguage.VB)
            {
                // turn option strict on for VB if in project settings

                VSProject project = (VSProject) Context.ProjectItem.ContainingProject.Object;
                Property prop = project.Project.Properties.Item("OptionExplicit");

                if ((prjOptionStrict) prop.Value == prjOptionStrict.prjOptionStrictOn)
                {
                    Context.CompileUnit.UserData.Add("AllowLateBound", false);
                }
            }

            CodeNamespace nameSpace = new CodeNamespace(Context.Namespace);
            nameSpace.Imports.AddRange(Context.Model.NamespaceImports.ToArray());
            Context.CompileUnit.Namespaces.Add(nameSpace);

            TemplateMemberGenerator templateMemberGenerator = new TemplateMemberGenerator(Context);
            templateMemberGenerator.AddTemplateUsings();

            foreach (ModelClass cls in Context.Model.Classes)
            {
                GenerateClass(cls, nameSpace, templateMemberGenerator);
            }

            foreach (NestedClass cls in Context.Model.NestedClasses)
            {
                GenerateNestedClass(cls, nameSpace);
            }

            if (Context.Model.GenerateMetaData != MetaDataGeneration.False)
            {
                GenerateMetaData(nameSpace);
            }

            GenerateInternalPropertyAccessor(Context, Context.Model, nameSpace);

            GenerateHelperClass(nameSpace);

            if (Context.Model.Target == CodeGenerationTarget.ActiveRecord)
            {
                Context.PrimaryOutput = GenerateCode(Context.CompileUnit);

                if (Context.Model.UseNHQG)
                {
                    try
                    {
                        AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
                        Assembly assembly = GenerateARAssembly(Context.CompileUnit, false);
                        UseNHQG(assembly);
                    }
                    finally
                    {
                        AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
                    }
                }
            }
            else
            {
                Type starter = null;
                Assembly assembly = null;

                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
                try
                {
                    _activeRecord = Assembly.Load(Context.Model.ActiveRecordAssemblyName);

                    // Code below means: ActiveRecordStarter.ModelsCreated += new ModelsCreatedDelegate(OnARModelCreated);
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

                    assembly = GenerateARAssembly(Context.CompileUnit, !Context.Model.UseNHQG);
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
                ClearARAttributes(Context.CompileUnit);
                Context.PrimaryOutput = GenerateCode(Context.CompileUnit);

                foreach (KeyValuePair<string, string> pair in _nHibernateConfigs)
                {
                    string path = Path.Combine(Context.ModelFilePath, RemoveNamespaceFromStart(pair.Key) + ".hbm.xml");
                    using (StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode))
                    {
                        writer.Write(pair.Value);
                    }

                    AddToProject(path, prjBuildAction.prjBuildActionEmbeddedResource);
                }

                if (Context.Model.UseNHQG)
                {
                   UseNHQG(assembly);
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

            string className = cls.Name;
            if (cls.Model.GeneratesDoubleDerived)
                className += cls.Model.DoubleDerivedNameSuffix;

            CodeTypeDeclaration classDeclaration = CreateClass(className);

            if (cls.Model.AutomaticAssociations)
                classDeclaration.CreateEmptyPublicConstructor();

            if (cls.Model.GeneratesDoubleDerived)
                classDeclaration.TypeAttributes |= TypeAttributes.Abstract;

            if (cls.ClassParent != null)
            {
                classDeclaration.BaseTypes.Add(new CodeTypeReference(cls.ClassParent.Name));
            }
            else if (cls.Model.UseBaseClass)
            {
                bool withValidator = cls.Properties.FindAll(delegate(ModelProperty property)
                {
                    return property.IsValidatorSet();
                }).Count > 0;

                CodeTypeReference type;
                // base class for every modelclass. If left empty then baseclass from model if left empty ...etc
                if (!string.IsNullOrEmpty(cls.BaseClassName))
                    type = new CodeTypeReference(cls.BaseClassName);
                else if (!string.IsNullOrEmpty(cls.Model.BaseClassName))
                    type = new CodeTypeReference(cls.Model.BaseClassName);
                else if (withValidator)
                    type = new CodeTypeReference(Common.DefaultValidationBaseClass);
                else
                    type = new CodeTypeReference(Common.DefaultBaseClass);

                if (cls.IsGeneric())
                    type.TypeArguments.Add(cls.Name);
                classDeclaration.BaseTypes.Add(type);
            }

            if (cls.DoesImplementINotifyPropertyChanged() && cls.ClassParent == null && !cls.Model.PropertyChangedDefinedInBaseClass)
            {
                classDeclaration.BaseTypes.Add(new CodeTypeReference(Common.INotifyPropertyChangedType));
                AddINotifyPropertyChangedRegion(classDeclaration, cls.Lazy | Context.Model.UseVirtualProperties);
            }
            if (cls.DoesImplementINotifyPropertyChanging() && cls.ClassParent == null && !cls.Model.PropertyChangingDefinedInBaseClass)
            {
                classDeclaration.BaseTypes.Add(new CodeTypeReference(Common.INotifyPropertyChangingType));
                AddINotifyPropertyChangingRegion(classDeclaration, cls.Lazy | Context.Model.UseVirtualProperties);
            }

            if (!String.IsNullOrEmpty(cls.Description))
                classDeclaration.Comments.AddRange(GetSummaryComment(cls.Description));

            if (!cls.Model.GeneratesDoubleDerived)
                cls.AddActiveRecordAttributes(classDeclaration);
            if (cls.Model.UseGeneratedCodeAttribute)
                classDeclaration.CustomAttributes.Add(AttributeHelper.GetGeneratedCodeAttribute());

            nameSpace.Types.Add(classDeclaration);
            return classDeclaration;
        }

        private CodeTypeDeclaration GetNestedClassDeclaration(NestedClass cls, CodeNamespace nameSpace)
        {
            if (cls == null)
                throw new ArgumentException("Nested class not supplied.", "cls");
            if (String.IsNullOrEmpty(cls.Name))
                throw new ArgumentException("Nested class name cannot be blank.", "cls");

            CodeTypeDeclaration classDeclaration = CreateClass(cls.Name);
            if (cls.DoesImplementINotifyPropertyChanged())
            {
                classDeclaration.BaseTypes.Add(new CodeTypeReference(Common.INotifyPropertyChangedType));
                AddINotifyPropertyChangedRegion(classDeclaration, Context.Model.UseVirtualProperties);
            }
            if (cls.DoesImplementINotifyPropertyChanging())
            {
                classDeclaration.BaseTypes.Add(new CodeTypeReference(Common.INotifyPropertyChangingType));
                AddINotifyPropertyChangingRegion(classDeclaration, Context.Model.UseVirtualProperties);
            }
            if (!String.IsNullOrEmpty(cls.Description))
                classDeclaration.Comments.AddRange(GetSummaryComment(cls.Description));

            if (cls.Model.UseGeneratedCodeAttribute)
                classDeclaration.CustomAttributes.Add(AttributeHelper.GetGeneratedCodeAttribute());

            nameSpace.Types.Add(classDeclaration);
            return classDeclaration;
        }

        private CodeTypeDeclaration GetCompositeClassDeclaration(CodeNamespace nameSpace,
                                                                 CodeTypeDeclaration parentClass,
                                                                 List<ModelProperty> keys, bool implementINotifyPropertyChanged)
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

            CodeTypeDeclaration classDeclaration = CreateClass(className);

            if (implementINotifyPropertyChanged)
            {
                classDeclaration.BaseTypes.Add(new CodeTypeReference(Common.INotifyPropertyChangedType));
                AddINotifyPropertyChangedRegion(classDeclaration, Context.Model.UseVirtualProperties);
            }

            classDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration("Serializable"));
            if (keys[0].ModelClass.Model.UseGeneratedCodeAttribute)
                classDeclaration.CustomAttributes.Add(AttributeHelper.GetGeneratedCodeAttribute());

            List<CodeMemberField> fields = new List<CodeMemberField>();

            List<string> descriptions = new List<string>();

            foreach (ModelProperty property in keys)
            {
                PropertyData propertyData = new PropertyData(property);

                CodeMemberField memberField = GetMemberFieldOfProperty(propertyData, Accessor.Private);
                classDeclaration.Members.Add(memberField);
                fields.Add(memberField);

                classDeclaration.Members.Add(GetActiveRecordMemberKeyProperty(memberField, propertyData));

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


        private CodeTypeDeclaration CreateClass(string name)
        {
            CodeTypeDeclaration classDeclaration = new CodeTypeDeclaration(name);
            classDeclaration.TypeAttributes = TypeAttributes.Public;
            classDeclaration.IsPartial = true;
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
            if (Context.Provider is VBCodeProvider)
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
            CodeMemberMethod equals = new CodeMemberMethod
                                          {
                                              Attributes = (MemberAttributes.Public | MemberAttributes.Override),
                                              ReturnType = new CodeTypeReference(typeof (Boolean)),
                                              Name = "Equals"
                                          };

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
            CodeMemberMethod toString = new CodeMemberMethod
                                            {
                                                Attributes = (MemberAttributes.Public | MemberAttributes.Override),
                                                ReturnType = new CodeTypeReference(typeof (String)),
                                                Name = "ToString"
                                            };

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

        // TODO: All this property generation is a total mess. Lots of similar methods. Hard to find what you're looking for. Hard to change.

        private CodeTypeMember GetActiveRecordMemberCompositeKeyProperty(CodeTypeDeclaration compositeClass,
                                                                         CodeMemberField memberField, bool implementsINotifyPropertyChanged, bool implementsINotifyPropertyChanging)
        {
            // TODO: Composite key generation omits UnsavedValue property. All the properties which are parts of the composite key
            // should have the same UnsavedValue, by the way.
            CodeMemberProperty memberProperty =
                GetMemberProperty(memberField, compositeClass.Name, true, true, implementsINotifyPropertyChanged, implementsINotifyPropertyChanging, null);

            memberProperty.CustomAttributes.Add(new CodeAttributeDeclaration("CompositeKey"));

            return memberProperty;
        }

        private CodeMemberProperty GetActiveRecordMemberKeyProperty(CodeMemberField memberField,
                                                                    PropertyData property)
        {
            CodeMemberProperty memberProperty =
                GetMemberProperty(memberField, property.Name, property.ColumnType, null, property.NotNull, true, true, property.ImplementsINotifyPropertyChanged(), property.ImplementsINotifyPropertyChanging(), property.Description);
            memberProperty.CustomAttributes.Add(property.GetKeyPropertyAttribute());

            return memberProperty;
        }

        private CodeMemberProperty GetActiveRecordMemberProperty(CodeMemberField memberField,
                                                                 PropertyData property)
        {
            CodeMemberProperty memberProperty =
                GetMemberProperty(memberField, property.Name, property.ColumnType,
								  property.CustomMemberType,
								  property.NotNull,
                                  true,
                                  true,
                                  property.ImplementsINotifyPropertyChanged(),
                                  property.ImplementsINotifyPropertyChanging(),
                                  property.Description);
            CodeAttributeDeclaration attributeDecleration = null;

            switch (property.KeyType)
            {
                // Composite keys must be handled in upper levels
                case KeyType.None:
                    attributeDecleration = property.GetPropertyAttribute();
                    break;
                case KeyType.PrimaryKey:
                    attributeDecleration = property.GetPrimaryKeyAttribute();
                    break;
            }

            memberProperty.CustomAttributes.Add(attributeDecleration);

            return memberProperty;
        }

        private CodeMemberProperty GetActiveRecordMemberVersion(CodeMemberField memberField,
                                                                PropertyData property)
        {
            CodeMemberProperty memberProperty =
                GetMemberProperty(memberField, property.Name, property.ColumnType, null, property.NotNull, true, true, property.ImplementsINotifyPropertyChanged(), property.ImplementsINotifyPropertyChanging(),
                                  property.Description);
            memberProperty.CustomAttributes.Add(property.GetVersionAttribute());

            return memberProperty;
        }

        private CodeMemberProperty GetActiveRecordMemberTimestamp(CodeMemberField memberField,
                                                                  PropertyData property)
        {
            CodeMemberProperty memberProperty =
                GetMemberProperty(memberField, property.Name, property.ColumnType, null, property.NotNull, true, true, property.ImplementsINotifyPropertyChanged(), property.ImplementsINotifyPropertyChanging(),
                                  property.Description);
            memberProperty.CustomAttributes.Add(property.GetTimestampAttribute());

            return memberProperty;
        }

		private CodeMemberProperty GetMemberProperty(CodeMemberField memberField, string propertyName,
												NHibernateType propertyType, string propertyCustomMemberType, bool propertyNotNull,
                                                bool get, bool set, bool implementINotifyPropertyChanged, bool implementINotifyPropertyChanging, params string[] description)
		{
		    CodeTypeReference memberPropertyType;
            if (Context.Model.UseNullables != NullableUsage.No && TypeHelper.IsNullable(propertyType) && !propertyNotNull)
            {
                if (Context.Model.UseNullables == NullableUsage.WithHelperLibrary)
                    memberPropertyType = TypeHelper.GetNullableTypeReferenceForHelper(propertyType);
                else
                    memberPropertyType = TypeHelper.GetNullableTypeReference(propertyType);
            }
            else
                memberPropertyType = new CodeTypeReference(TypeHelper.GetSystemType(propertyType, propertyCustomMemberType));

            CodeMemberProperty memberProperty =
                GetMemberProperty(memberField, memberPropertyType, propertyName, null, null, get, set, implementINotifyPropertyChanged, implementINotifyPropertyChanging, description);

            return memberProperty;
        }

        private CodeMemberProperty GetMemberProperty(CodeMemberField memberField, string propertyName,
                                                     bool get, bool set, bool implementINotifyPropertyChanged, bool implementINotifyPropertyChanging, params string[] description)
        {
            return GetMemberProperty(memberField, propertyName, null, null, get, set, implementINotifyPropertyChanged, implementINotifyPropertyChanging, description);
        }

        private CodeMemberProperty GetMemberProperty(CodeMemberField memberField, string propertyName, string automaticAssociationCollectionName, string automaticAssociationCollectionItemType,
                                                     bool get, bool set, bool implementINotifyPropertyChanged, bool implementINotifyPropertyChanging, params string[] description)
        {
            CodeMemberProperty memberProperty =
                GetMemberProperty(memberField, memberField.Type, propertyName, automaticAssociationCollectionName, automaticAssociationCollectionItemType, get, set, implementINotifyPropertyChanged, implementINotifyPropertyChanging, description);

            return memberProperty;
        }

        private CodeMemberProperty GetMemberProperty(CodeMemberField memberField, CodeTypeReference propertyType, string propertyName, string automaticAssociationCollectionName, string automaticAssociationCollectionItemType,
                                                                bool get, bool set, bool implementINotifyPropertyChanged, bool implementINotifyPropertyChanging, params string[] description)
        {
            CodeMemberProperty memberProperty = new CodeMemberProperty();

            memberProperty.Name = propertyName;
            memberProperty.Type = propertyType;

            if (Context.Model.UseVirtualProperties)
                memberProperty.Attributes = MemberAttributes.Public;
            else
                memberProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            if (get)
                memberProperty.GetStatements.Add(new CodeMethodReturnStatement(
                                                     new CodeFieldReferenceExpression(
                                                         new CodeThisReferenceExpression(), memberField.Name)));
            if (set)
            {
                var assignValue = new CodeAssignStatement(
                                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), memberField.Name),
                                    new CodeArgumentReferenceExpression("value")
                                    );

                if (!implementINotifyPropertyChanged && !implementINotifyPropertyChanging && automaticAssociationCollectionName == null)
                {
                    memberProperty.SetStatements.Add(assignValue);
                }
                else
                {
                    var memberFieldExpression = new CodeFieldReferenceExpression(null, memberField.Name);
                    var valueExpression = new CodeArgumentReferenceExpression("value");

                    var equalityCheck = InequalityExpression(memberFieldExpression, valueExpression);

                    var assignment = new CodeConditionStatement(equalityCheck);
                    memberProperty.SetStatements.Add(assignment);

                    if (automaticAssociationCollectionName != null)
                    {
                        assignment.TrueStatements.Add(
                            new CodeVariableDeclarationStatement(propertyType, "oldValue", memberFieldExpression));
                    }

                    assignment.TrueStatements.Add(assignValue);

                    if (automaticAssociationCollectionName != null)
                    {
                        var thisExpression = new CodeCastExpression(automaticAssociationCollectionItemType, new CodeThisReferenceExpression());
                        var nullExpression = new CodePrimitiveExpression(null);
                        var oldValueExpression = new CodeVariableReferenceExpression("oldValue");

                        assignment.TrueStatements.Add(
                            new CodeConditionStatement(
                                InequalityExpression(oldValueExpression, nullExpression),
                                new CodeConditionStatement(
                                    new CodeMethodInvokeExpression(
                                        new CodePropertyReferenceExpression(oldValueExpression, automaticAssociationCollectionName),
                                        "Contains", thisExpression),
                                    new CodeExpressionStatement(
                                        new CodeMethodInvokeExpression(
                                            new CodePropertyReferenceExpression(oldValueExpression, automaticAssociationCollectionName),
                                            "Remove", thisExpression)))));

                        assignment.TrueStatements.Add(
                            new CodeConditionStatement(
                                InequalityExpression(valueExpression, nullExpression),
                                new CodeConditionStatement(
                                    new CodeBinaryOperatorExpression(
                                        new CodeMethodInvokeExpression(
                                            new CodePropertyReferenceExpression(valueExpression, automaticAssociationCollectionName),
                                            "Contains", thisExpression),
                                        CodeBinaryOperatorType.ValueEquality,
                                        new CodePrimitiveExpression(false)
                                    ),
                                    new CodeExpressionStatement(
                                        new CodeMethodInvokeExpression(
                                            new CodePropertyReferenceExpression(valueExpression, automaticAssociationCollectionName),
                                            "Add", thisExpression)))));
                    }

                    if (implementINotifyPropertyChanged)
                    {
                        assignment.TrueStatements.Add(
                            new CodeExpressionStatement(
                                new CodeMethodInvokeExpression(
                                    new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),
                                                                             Context.Model.PropertyChangedMethodName),
                                           new CodePrimitiveExpression(propertyName)
                                           ))
                            );
                    }
                    if (implementINotifyPropertyChanging)
                    {
                        assignment.TrueStatements.Insert(0,
                            new CodeExpressionStatement(
                                new CodeMethodInvokeExpression(
                                    new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),
                                                                             Context.Model.PropertyChangingMethodName),
                                           new CodePrimitiveExpression(propertyName)
                                           ))
                            );
                    }
                }
            }

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

        private CodeMemberField GetMemberFieldOfProperty(PropertyData property, Accessor accessor)
        {
            return GetMemberField(property.Name, GetPropertyType(property), accessor, property.EffectiveAccess);
        }

        private CodeTypeReference GetPropertyType(PropertyData property)
        {
            if (Context.Model.UseNullables != NullableUsage.No && TypeHelper.IsNullable(property.ColumnType) && !property.NotNull)
            {
                if (Context.Model.UseNullables == NullableUsage.WithHelperLibrary)
                    return TypeHelper.GetNullableTypeReferenceForHelper(property.ColumnType);

                return TypeHelper.GetNullableTypeReference(property.ColumnType);
            }

            return new CodeTypeReference(TypeHelper.GetSystemType(property.ColumnType, property.CustomMemberType));
        }

        private void AddMemberField(CodeTypeDeclaration classDeclaration, PropertyData property)
        {
            if (property.IsJoinedKey)
            {
                classDeclaration.Members.Add(GetJoinedKeyProperty(property));
            }
            else
            {
                CodeMemberField memberField = null;
                // Soooo ugly.
                switch (property.PropertyType)
                {
                    case PropertyType.Property:
                        memberField = GetMemberFieldOfProperty(property, Accessor.Private);
                        CodeMemberProperty memberProperty = GetActiveRecordMemberProperty(memberField, property);
                        classDeclaration.Members.Add(memberProperty);
                        if (property.IsValidatorSet)
                            memberProperty.CustomAttributes.AddRange(property.GetValidationAttributes());
                        break;
                    case PropertyType.Field:
                        memberField = GetMemberFieldOfProperty(property, property.Accessor);
                        memberField.CustomAttributes.Add(property.GetFieldAttribute());
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
            }
        }

        private CodeMemberProperty GetJoinedKeyProperty(PropertyData property)
        {
            string basePropertyName = property.ModelClass.PrimaryKey.Name;
            CodeTypeReference basePropertyType = GetPropertyType(property.ModelClass.PrimaryKey);

            CodeMemberProperty memberProperty = new CodeMemberProperty();
            memberProperty.CustomAttributes.Add(property.GetPrimaryKeyAttribute());
            memberProperty.Attributes = MemberAttributes.Public;
            if (Context.Model.UseVirtualProperties)
            {
                if (property.Name == basePropertyName)
                    memberProperty.Attributes |= MemberAttributes.Override;
            }
            else
            {
                memberProperty.Attributes |= MemberAttributes.Final;
                if (property.Name == basePropertyName)
                    memberProperty.Attributes |= MemberAttributes.New;
            }

            memberProperty.Name = property.Name;
            memberProperty.Type = basePropertyType;
            memberProperty.HasGet = true;
            memberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeBaseReferenceExpression(), basePropertyName)));
            memberProperty.HasSet = true;
            memberProperty.SetStatements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeBaseReferenceExpression(), basePropertyName), new CodeVariableReferenceExpression("value")));

            return memberProperty;
        }

        private CodeMemberField GetMemberField(string name, CodeTypeReference fieldType, Accessor accessor, PropertyAccess access)
        {
            CodeMemberField memberField = GetMemberFieldWithoutType(name, accessor, access);

            memberField.Type = fieldType;

            return memberField;
        }

        private CodeMemberField GetMemberField(string name, string fieldType, Accessor accessor, PropertyAccess access)
        {
            return GetMemberField(name, new CodeTypeReference(fieldType), accessor, access);
        }

        private CodeMemberField GetGenericMemberField(string typeName, string name, string fieldType, Accessor accessor, PropertyAccess access)
        {
            CodeMemberField memberField = GetMemberFieldWithoutType(name, accessor, access);

            CodeTypeReference type = new CodeTypeReference(fieldType);
            if (!TypeHelper.ContainsGenericDecleration(fieldType, Context.Language))
            {
                type.TypeArguments.Add(typeName);
            }
            memberField.Type = type;

            return memberField;
        }

        private CodeMemberField GetMemberFieldWithoutType(string name, Accessor accessor, PropertyAccess access)
        {
            CodeMemberField memberField = GetMemberFieldWithoutTypeAndName(accessor);

            memberField.Name = NamingHelper.GetName(name, access, Context.Model.CaseOfPrivateFields);

            return memberField;
        }

        private CodeMemberField GetMemberFieldWithoutTypeAndName(Accessor accessor)
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
            return memberField;
        }

        private CodeMemberField GetStaticMemberFieldWithoutTypeAndName(Accessor accessor)
        {
            CodeMemberField memberField = new CodeMemberField();

            switch (accessor)
            {
                case Accessor.Public:
                    memberField.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                    break;
                case Accessor.Protected:
                    memberField.Attributes = MemberAttributes.Family | MemberAttributes.Static;
                    break;
                case Accessor.Private:
                    memberField.Attributes = MemberAttributes.Private | MemberAttributes.Static;
                    break;
            }
            return memberField;
        }

        #endregion

        #region Relation

        #region HasMany

        private void GenerateHasManyRelation(CodeTypeDeclaration classDeclaration, CodeNamespace nameSpace,
                                             ManyToOneRelation relationship)
        {
            if (relationship.TargetPropertyGenerated)
                GenerateHasMany(
                    classDeclaration,
                    relationship.Target.Name,
                    relationship.EffectiveTargetPropertyName,
                    relationship.TargetPropertyType,
                    relationship.EffectiveTargetAccess,
                    relationship.TargetDescription,
                    relationship.GetHasManyAttribute(Context),
                    relationship.Source.AreRelationsGeneric(),
                    relationship.Target.DoesImplementINotifyPropertyChanged(),
                    relationship.Target.DoesImplementINotifyPropertyChanging(),
                    relationship.Source.Name,
                    relationship.EffectiveSourcePropertyName,
                    relationship.EffectiveAutomaticAssociations,
                    false,
                    null);
        }

        private void GenerateHasMany(CodeTypeDeclaration classDeclaration, string thisClassName, string propertyName, string customPropertyType, PropertyAccess propertyAccess, string description, CodeAttributeDeclaration attribute, bool genericRelation, bool propertyChanged, bool propertyChanging, string oppositeClassName, string oppositePropertyName, bool automaticAssociationGenerated, bool manyToMany, CodeAttributeDeclaration collectionIdAttribute)
        {
            string propertyType = String.IsNullOrEmpty(customPropertyType)
                                      ? Context.Model.EffectiveListInterface
                                      : customPropertyType;

            string memberType = propertyType;
            if (automaticAssociationGenerated)
                memberType = Context.Model.AutomaticAssociationCollectionImplementation;

            CodeMemberField memberField;
            if (!genericRelation)
                memberField = GetMemberField(propertyName, memberType, Accessor.Private, propertyAccess);
            else
                memberField = GetGenericMemberField(oppositeClassName, propertyName, memberType, Accessor.Private, propertyAccess);
            classDeclaration.Members.Add(memberField);

            // Initializes the collection by assigning a new list instance to the field.
            // Many-to-many relationships never had the initialization code enabled before.
            // Automatic associations initialize their lists in the constructor instead.
            if (Context.Model.InitializeIListFields && propertyType == Context.Model.EffectiveListInterface && !automaticAssociationGenerated)
            {
                CodeObjectCreateExpression fieldCreator = new CodeObjectCreateExpression();
                fieldCreator.CreateType = GetConcreteListType(oppositeClassName);
                memberField.InitExpression = fieldCreator;
            }

            bool createSetter = automaticAssociationGenerated ? false : true;

            if (description == "") description = null;
            CodeMemberProperty memberProperty = GetMemberProperty(memberField, propertyName, true, createSetter, propertyChanged, propertyChanging, description);
            // We need the propertyType with generic arguments added if there are any.
            memberProperty.Type = new CodeTypeReference(propertyType);
            memberProperty.Type.TypeArguments.AddRange(memberField.Type.TypeArguments);
                               
            classDeclaration.Members.Add(memberProperty);

            if (automaticAssociationGenerated)
            {
                AddConstructorForWatchedList(classDeclaration, memberField, propertyName);
                AddInternalWatchedListProperty(classDeclaration, memberProperty.Type, propertyName, memberField.Name, propertyChanged, propertyChanging, propertyAccess);
                AddItemAddedRemovedMethods(classDeclaration, thisClassName, propertyName, oppositeClassName, oppositePropertyName, manyToMany);
            }

            memberProperty.CustomAttributes.Add(attribute);

            if (collectionIdAttribute != null)
                memberProperty.CustomAttributes.Add(collectionIdAttribute);
        }

        #endregion

        #region BelongsTo

        private void GenerateBelongsToRelation(CodeTypeDeclaration classDeclaration, CodeNamespace nameSpace,
                                               ManyToOneRelation relationship)
        {
            if (relationship.SourcePropertyGenerated)
            {
                if (!String.IsNullOrEmpty(relationship.TargetColumnKey) && (!String.IsNullOrEmpty(relationship.EffectiveSourceColumn)) &&
                    !relationship.EffectiveSourceColumn.ToUpperInvariant().Equals(relationship.TargetColumnKey.ToUpperInvariant()))
                    throw new ArgumentException(
                        String.Format(
                            "Class {0} column name does not match with column key {1} on it's many to one relation to class {2}",
                            relationship.Source.Name, relationship.TargetColumnKey, relationship.Target.Name));

                #warning We might use PropertyAccess.Property which uses the model's CaseOfPrivateFields to get the name.  If it's FieldCase.Unchanged, bad things happen.
                CodeMemberField memberField = GetMemberField(relationship.EffectiveSourcePropertyName, relationship.Target.Name, Accessor.Private, relationship.EffectiveSourceAccess);
                classDeclaration.Members.Add(memberField);

                string automaticAssociationCollectionName =
                    relationship.EffectiveAutomaticAssociations
                        ? relationship.EffectiveTargetPropertyName
                        : null;

                CodeMemberProperty memberProperty = GetMemberProperty(memberField, relationship.EffectiveSourcePropertyName, automaticAssociationCollectionName, relationship.Source.Name, true, true, relationship.Source.DoesImplementINotifyPropertyChanged(), relationship.Source.DoesImplementINotifyPropertyChanging(), relationship.SourceDescription == "" ? null : relationship.SourceDescription);
                classDeclaration.Members.Add(memberProperty);

                memberProperty.CustomAttributes.Add(relationship.GetBelongsToAttribute());
            }
        }

        #endregion

        #region HasAndBelongsToRelation

        private void GenerateHasAndBelongsToRelationFromTargets(CodeTypeDeclaration classDeclaration, CodeNamespace nameSpace,
                                               ManyToManyRelation relationship)
        {
            if (relationship.SourcePropertyGenerated)
                GenerateHasMany(
                    classDeclaration,
                    relationship.Source.Name,
                    relationship.EffectiveSourcePropertyName,
                    relationship.SourcePropertyType,
                    relationship.EffectiveSourceAccess,
                    relationship.SourceDescription,
                    relationship.GetHasAndBelongsToAttributeFromSource(Context),
                    relationship.Target.AreRelationsGeneric(),
                    relationship.Source.DoesImplementINotifyPropertyChanged(),
                    relationship.Source.DoesImplementINotifyPropertyChanging(),
                    relationship.Target.Name,
                    relationship.EffectiveTargetPropertyName,
                    relationship.EffectiveAutomaticAssociations,
                    true,
                    relationship.GetCollectionIdAttribute(relationship.EffectiveSourceRelationType));
        }

        private void GenerateHasAndBelongsToRelationFromSources(CodeTypeDeclaration classDeclaration, CodeNamespace nameSpace,
                                               ManyToManyRelation relationship)
        {
            if (relationship.TargetPropertyGenerated)
                GenerateHasMany(
                    classDeclaration,
                    relationship.Target.Name,
                    relationship.EffectiveTargetPropertyName,
                    relationship.TargetPropertyType,
                    relationship.EffectiveTargetAccess,
                    relationship.TargetDescription,
                    relationship.GetHasAndBelongsToAttributeFromTarget(Context),
                    relationship.Source.AreRelationsGeneric(),
                    relationship.Target.DoesImplementINotifyPropertyChanged(),
                    relationship.Target.DoesImplementINotifyPropertyChanging(),
                    relationship.Source.Name,
                    relationship.EffectiveSourcePropertyName,
                    relationship.EffectiveAutomaticAssociations,
                    true,
                    relationship.GetCollectionIdAttribute(relationship.EffectiveTargetRelationType));
        }

        #endregion

        #region OneToOne

        private void GenerateOneToOneRelationFromTarget(CodeTypeDeclaration classDeclaration, CodeNamespace nameSpace,
                                              OneToOneRelation relationship)
        {
            CodeMemberField memberField = GetMemberField(relationship.Target.Name, relationship.Target.Name, Accessor.Private, relationship.EffectiveTargetAccess);
            classDeclaration.Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.SourceDescription))
                memberProperty = GetMemberProperty(memberField, relationship.Target.Name, true, true, relationship.Target.DoesImplementINotifyPropertyChanged(), relationship.Target.DoesImplementINotifyPropertyChanging(), null);
            else
                memberProperty =
                    GetMemberProperty(memberField, relationship.Target.Name, true, true, relationship.Target.DoesImplementINotifyPropertyChanged(), relationship.Target.DoesImplementINotifyPropertyChanging(),
                                      relationship.SourceDescription);
            classDeclaration.Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(relationship.GetOneToOneAttributeForSource());
        }

        private void GenerateOneToOneRelationFromSources(CodeTypeDeclaration classDeclaration, CodeNamespace nameSpace,
                                               OneToOneRelation relationship)
        {
            CodeMemberField memberField = GetMemberField(relationship.Source.Name, relationship.Source.Name, Accessor.Private, relationship.EffectiveSourceAccess);
            classDeclaration.Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.TargetDescription))
                memberProperty = GetMemberProperty(memberField, relationship.Source.Name, true, true, relationship.Source.DoesImplementINotifyPropertyChanged(), relationship.Source.DoesImplementINotifyPropertyChanging(), null);
            else
                memberProperty =
                    GetMemberProperty(memberField, relationship.Source.Name, true, true, relationship.Source.DoesImplementINotifyPropertyChanged(), relationship.Source.DoesImplementINotifyPropertyChanging(),
                                      relationship.TargetDescription);
            classDeclaration.Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(relationship.GetOneToOneAttributeForTarget());
        }

        #endregion

        #region Nested

        private void GenerateNestingRelationFromRelationship(CodeTypeDeclaration classDeclaration, NestedClassReferencesModelClasses relationship)
        {
            string propertyName = String.IsNullOrEmpty(relationship.PropertyName)
                          ? relationship.NestedClass.Name
                          : relationship.PropertyName;

            CodeMemberField memberField = GetMemberField(propertyName, relationship.NestedClass.Name, Accessor.Private, PropertyAccess.Property);
            classDeclaration.Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.Description))
                memberProperty = GetMemberProperty(memberField, propertyName, true, true, relationship.ModelClass.DoesImplementINotifyPropertyChanged(), relationship.ModelClass.DoesImplementINotifyPropertyChanging(), null);
            else
                memberProperty =
                    GetMemberProperty(memberField, propertyName, true, true, relationship.ModelClass.DoesImplementINotifyPropertyChanged(), relationship.ModelClass.DoesImplementINotifyPropertyChanging(),
                                      relationship.Description);
            classDeclaration.Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(relationship.GetNestedAttribute());
        }

        #endregion

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

        private CodeTypeDeclaration GenerateNestedClass(NestedClass cls, CodeNamespace nameSpace)
        {
            if (cls == null)
                throw new ArgumentNullException( "cls", "Nested class not supplied");
            if (nameSpace == null)
                throw new ArgumentNullException("nameSpace", "Namespace not supplied");
            if (String.IsNullOrEmpty(cls.Name))
                throw new ArgumentException("Class name cannot be blank", "cls");

            CodeTypeDeclaration classDeclaration = GetNestedClassDeclaration(cls, nameSpace);

            // Properties and Fields
            foreach (var property in cls.Properties)
            {
                PropertyData propertyData = new PropertyData(property);

                AddMemberField(classDeclaration, propertyData);

                if (propertyData.DebuggerDisplay)
                    classDeclaration.CustomAttributes.Add(propertyData.GetDebuggerDisplayAttribute());
                if (propertyData.DefaultMember)
                    classDeclaration.CustomAttributes.Add(propertyData.GetDefaultMemberAttribute());
            }

            return classDeclaration;
        }

        private CodeTypeDeclaration GenerateClass(ModelClass cls, CodeNamespace nameSpace, TemplateMemberGenerator templateMemberGenerator)
        {
            if (cls == null)
                throw new ArgumentNullException("cls", "Class not supplied");
            if (nameSpace == null)
                throw new ArgumentNullException("nameSpace", "Namespace not supplied");
            if (String.IsNullOrEmpty(cls.Name))
                throw new ArgumentException("Class name cannot be blank", "cls");

            CodeTypeDeclaration classDeclaration = GetClassDeclaration(cls, nameSpace);

            GenerateCommonPrimaryKey(cls, classDeclaration);

            List<ModelProperty> compositeKeys = new List<ModelProperty>();

            // Properties and Fields
            foreach (ModelProperty property in cls.Properties)
            {
                PropertyData propertyData = new PropertyData(property);

                if (property.KeyType != KeyType.CompositeKey)
                {
                    AddMemberField(classDeclaration, propertyData);

                    if (property.DebuggerDisplay)
                        classDeclaration.CustomAttributes.Add(propertyData.GetDebuggerDisplayAttribute());
                    if (property.DefaultMember)
                        classDeclaration.CustomAttributes.Add(propertyData.GetDefaultMemberAttribute());
                }
                else
                    compositeKeys.Add(property);
            }

            if (compositeKeys.Count > 0)
            {
                CodeTypeDeclaration compositeClass =
                    GetCompositeClassDeclaration(nameSpace, classDeclaration, compositeKeys, cls.DoesImplementINotifyPropertyChanged());

                // TODO: All access fields in a composite group assumed to be the same.
                // We have a model validator for this case but the user may save anyway.
                // Check if all access fields are the same.
                CodeMemberField memberField = GetPrivateMemberFieldOfCompositeClass(compositeClass, PropertyAccess.Property);
                classDeclaration.Members.Add(memberField);

                classDeclaration.Members.Add(GetActiveRecordMemberCompositeKeyProperty(compositeClass, memberField, cls.DoesImplementINotifyPropertyChanged(), cls.DoesImplementINotifyPropertyChanging()));
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

            //Nested links
            ReadOnlyCollection<NestedClassReferencesModelClasses> nestingTargets =
                NestedClassReferencesModelClasses.GetLinksToNestedClasses(cls);
            foreach (NestedClassReferencesModelClasses relationship in nestingTargets)
            {
                GenerateNestingRelationFromRelationship(classDeclaration, relationship);
            }

            // TODO: Other relation types (any etc)

            GenerateDerivedClass(cls, nameSpace);

            templateMemberGenerator.AddTemplateMembers(cls, classDeclaration);

            return classDeclaration;
        }

        public static CodeExpression GetBooleanAnd(List<CodeExpression> expressions, int i)
        {
            if (i == expressions.Count - 2)
                return
                    new CodeBinaryOperatorExpression(expressions[i], CodeBinaryOperatorType.BooleanAnd,
                                                     expressions[i + 1]);
            
            return
                new CodeBinaryOperatorExpression(expressions[i], CodeBinaryOperatorType.BooleanAnd,
                                                 GetBooleanAnd(expressions, i + 1));
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

        private static CodeExpression GetXor(IList<CodeExpression> expressions, int i)
        {
            if (i == expressions.Count - 2)
                return new CodeMethodInvokeExpression(null, Common.XorHelperMethod, expressions[i], expressions[i + 1]);
            
            return
                new CodeMethodInvokeExpression(null, Common.XorHelperMethod, expressions[i],
                                               GetXor(expressions, i + 1));
        }

        private string GenerateCode(CodeCompileUnit compileUnit)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                Context.Provider.GenerateCodeFromCompileUnit(compileUnit, sw, new CodeGeneratorOptions());
            }

            return sb.ToString();
        }

        private void UseNHQG(Assembly assembly)
        {
            System.Diagnostics.Process nhqg = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            string tempFilePath = Path.Combine(DTEHelper.GetIntermediatePath(DTEHelper.GetVSProject(Context.ProjectItem)), "ActiveWriterHbmOutput");
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

                string nHQGExecutable = Context.Model.NHQGExecutable;
                if (!Path.IsPathRooted(nHQGExecutable))
                {
                  VSProject project = DTEHelper.GetVSProject(Context.ProjectItem);
                  string projectDirectory = Path.GetDirectoryName(project.Project.FullName);
                  nHQGExecutable = Path.GetFullPath(Path.Combine(projectDirectory, nHQGExecutable));
                }

                startInfo.FileName = nHQGExecutable;
                startInfo.WorkingDirectory = Path.GetDirectoryName(nHQGExecutable);
                string[] args = new string[4];
                args[0] = "/lang:" + (Context.Language == CodeLanguage.CSharp ? "CS" : "VB");
                args[1] = "/files:\"" + assembly.Location + "\"";
                args[2] = "/out:\"" + tempFileFolder.FullName + "\"";
                args[3] = "/ns:" + Context.Namespace;
                startInfo.Arguments = string.Join(" ", args);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.ErrorDialog = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                nhqg.StartInfo = startInfo;

                Log("Running NHQG with parameters: " + String.Join(" ", args));

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
                            string filePath = Path.Combine(Context.ModelFilePath, file.Name);
                            file.CopyTo(filePath , true);
                            AddToProject(filePath, prjBuildAction.prjBuildActionCompile);
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

        private void AddToProject(string path, prjBuildAction buildAction)
        {
            ProjectItem item = null;

            if (Context.Model.RelateWithActiwFile)
                item = Context.ProjectItem.ProjectItems.AddFromFile(path);
            else
                item = Context.DTE.ItemOperations.AddExistingItem(path);

            item.Properties.Item("BuildAction").Value = (int)buildAction;
        }

        private Assembly GenerateARAssembly(CodeCompileUnit compileUnit, bool generateInMemory)
        {
            List<string> addedAssemblies = new List<string>();
            string assemblyName = "ActiveWriter.Temp.Assembly" + Guid.NewGuid().ToString("N") + ".dll";
            CompilerParameters parameters = new CompilerParameters
                                                {
                                                    GenerateInMemory = generateInMemory,
                                                    GenerateExecutable = false
                                                };

            Assembly activeRecord = Assembly.Load(Context.Model.ActiveRecordAssemblyName);
            parameters.ReferencedAssemblies.Add(activeRecord.Location);
            Assembly nHibernate = Assembly.Load(Context.Model.NHibernateAssemblyName);
            parameters.ReferencedAssemblies.Add(nHibernate.Location);
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            addedAssemblies.Add("castle.activerecord.dll");
            addedAssemblies.Add("nhibernate.dll");
            addedAssemblies.Add("system.dll");
            addedAssemblies.Add("mscorlib.dll");

            // also add references to assemblies referenced by this project
            VSProject proj = DTEHelper.GetVSProject(Context.ProjectItem);
            foreach (Reference reference in proj.References)
            {
                if (!addedAssemblies.Contains(Path.GetFileName(reference.Path).ToLowerInvariant()))
                {
                    parameters.ReferencedAssemblies.Add(reference.Path);
                    addedAssemblies.Add(Path.GetFileName(reference.Path).ToLowerInvariant());
                }
            }

            parameters.OutputAssembly = generateInMemory
                                            ? assemblyName
                                            : Path.Combine(
                                                  DTEHelper.GetIntermediatePath(proj), assemblyName);
            
            CompilerResults results = Context.Provider.CompileAssemblyFromDom(parameters, compileUnit);
            if (results.Errors.Count == 0)
            {
                return results.CompiledAssembly;
            }
            
            Context.TextTemplatingHost.LogErrors(results.Errors);
            throw new ModelingException("Cannot compile in-memory ActiveRecord assembly due to errors. Please check that all the information required, such as imports, to compile the generated code in-memory is provided. An ActiveRecord assembly is generated in-memory to support NHibernate .hbm.xml generation and NHQG integration.");
        }

        // Actually: OnARModelCreated(ActiveRecordModelCollection, IConfigurationSource)
        // We're not using it typed since we use this through reflection.
        public void OnARModelCreated(object models, object source)
        {
            _nHibernateConfigs.Clear();
            if (models != null)
            {
                Type modelCollection = _activeRecord.GetType("Castle.ActiveRecord.Framework.Internal.ActiveRecordModelCollection");
                if ((int)modelCollection.GetProperty("Count").GetValue(models, null) > 0)
                {
                    string actualAssemblyName = ", " + DTEHelper.GetAssemblyName(Context.ProjectItem.ContainingProject);
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
                                if ((name.IndexOf('.') < 0) && !string.IsNullOrEmpty(Context.DefaultNamespace))
                                    newValue = Context.DefaultNamespace + "." + name;

                                // append assembly name
                                attribute.Value = newValue + actualAssemblyName;

                                // also fix any class attributes
                                XmlNodeList ClassAttributes = document.SelectNodes("//@class");

                                foreach (XmlAttribute ClassAttribute in ClassAttributes)
                                {
                                    UpdateClassName(actualAssemblyName, tempAssemblyName, ClassAttribute);
                                }

                                xml = document.OuterXml;
                                _nHibernateConfigs.Add(name, xml);
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
                if ((name.IndexOf('.') < 0) && !string.IsNullOrEmpty(Context.DefaultNamespace))
                    name = Context.DefaultNamespace + "." + name;

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
                string assemblyPath = Path.Combine(Context.Model.AssemblyPath, name.Name + ".dll");

                // try project references
                if (!File.Exists(assemblyPath))
                {
                    VSProject proj = (VSProject)Context.ProjectItem.ContainingProject.Object;
                    foreach (Reference reference in proj.References)
                    {
                        if (reference.Name == name.Name)
                        {
                            assemblyPath = reference.Path;
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
            if (!string.IsNullOrEmpty(Context.Namespace) && name.StartsWith(Context.Namespace))
                return name.Remove(0, Context.Namespace.Length + 1);

            return name;
        }

        private void AddINotifyPropertyChangedRegion(CodeTypeDeclaration declaration, bool useVirtual)
        {
            // We used to put this in a nice tidy region, but since we need to support
            // virtual events for lazy classes, and the CodeDOM doesn't support those,
            // we had to generate the events using snippets.  This broke the region
            // support by placing formerly contiguous members in entirely different
            // parts of the generated output.

            CodeTypeMember memberEvent = GetMemberEvent("PropertyChanged", "PropertyChangedEventHandler", useVirtual);
            //memberEvent.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "INotifyPropertyChanged Members"));
            declaration.Members.Add(memberEvent);

            CodeMemberMethod propertyChangedMethod = new CodeMemberMethod
                                                         {
                                                             Attributes = MemberAttributes.Family,
                                                             Name = Context.Model.PropertyChangedMethodName
                                                         };
            //propertyChangedMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));
            propertyChangedMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "information"));
            declaration.Members.Add(propertyChangedMethod);

            CodeConditionStatement ifStatement = new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeFieldReferenceExpression(null, "PropertyChanged"),
                    CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)
                    ), new CodeExpressionStatement(
                new CodeMethodInvokeExpression(null, "PropertyChanged",
                                               new CodeThisReferenceExpression(),
                                               new CodeObjectCreateExpression(
                                                   "PropertyChangedEventArgs",
                                                   new CodeFieldReferenceExpression(null, "information"))))
                );
            propertyChangedMethod.Statements.Add(ifStatement);
        }

        private void AddINotifyPropertyChangingRegion(CodeTypeDeclaration declaration, bool useVirtual)
        {
            // See comment on regions in AddINotifyPropertyChangedRegion.

            CodeTypeMember memberEvent = GetMemberEvent("PropertyChanging", "PropertyChangingEventHandler", useVirtual);
            //memberEvent.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "INotifyPropertyChanging Members"));
            declaration.Members.Add(memberEvent);

            CodeMemberMethod propertyChangingMethod = new CodeMemberMethod
                                                          {
                                                              Attributes = MemberAttributes.Family,
                                                              Name = Context.Model.PropertyChangingMethodName
                                                          };
            //propertyChangingMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));
            propertyChangingMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "information"));
            declaration.Members.Add(propertyChangingMethod);

            CodeConditionStatement ifStatement = new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeFieldReferenceExpression(null, "PropertyChanging"),
                    CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)
                    ), new CodeExpressionStatement(
                new CodeMethodInvokeExpression(null, "PropertyChanging",
                                               new CodeThisReferenceExpression(),
                                               new CodeObjectCreateExpression(
                                                   "PropertyChangingEventArgs",
                                                   new CodeFieldReferenceExpression(null, "information"))))
                );
            propertyChangingMethod.Statements.Add(ifStatement);
        }

        private CodeTypeMember GetMemberEvent(string eventName, string eventType, bool useVirtual)
        {
            if (Context.Language == CodeLanguage.CSharp)
            {
                // This is here because CodeDOM doesn't let us create virtual events.
                // Example: public virtual event PropertyChangedEventHandler PropertyChanged;
                string snippet = "        ";
                snippet += "public ";
                if (useVirtual)
                    snippet += "virtual ";
                snippet += "event ";
                snippet += eventType + " " + eventName + ";";
                return new CodeSnippetTypeMember(snippet);
            }

            if (Context.Language == CodeLanguage.VB)
            {
                // VB is crazy and doesn't allow Overridable (virtual) events.  This means that lazy classes with events will only work properly with C# out of the box.
                // It would be possible to implement additional NHibernate logic to work around this problem, but generating the code like this mirrors what's possible for typical developers out of the box.
                // The following code is the original code, but it has never worked in VB because it's missing the "Implements INotifyPropertyChanged.PropertyChanged" part of the statement.  It wasn't obvious to me how to fix it, so I left it broken.
                return new CodeMemberEvent
                {
                    Attributes = MemberAttributes.Public,
                    Name = "PropertyChanged",
                    Type = new CodeTypeReference("PropertyChangedEventHandler")
                };
            }

            throw new NotImplementedException("Don't know how to generate events for language: " + Context.Language);
        }

        private void GenerateMetaData(CodeNamespace nameSpace)
        {
            foreach (CodeTypeDeclaration type in nameSpace.Types)
            {
                List<CodeTypeMember> properties = new List<CodeTypeMember>();
                foreach (CodeTypeMember member in type.Members)
                {
                    if ((member is CodeMemberProperty || member is CodeMemberField) && PropertyData.IsMetaDataGeneratable(member))
                    {
                        properties.Add(member);
                    }
                }

                if (properties.Count > 0)
                {
                    if (Context.Model.GenerateMetaData == MetaDataGeneration.InClass)
                    {
                        GenerateInClassMetaData(type, properties);
                    }
                    else if (Context.Model.GenerateMetaData == MetaDataGeneration.InSubClass)
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
            CodeTypeDeclaration subClass = CreateClass("Properties");
            foreach (var member in members)
            {
                subClass.Members.Add(GenerateMetaDataField(member, member.Name));
            }

            type.Members.Add(subClass);
        }

        private CodeMemberField GenerateMetaDataField(CodeTypeMember member, string nm)
        {
            CodeMemberField field = new CodeMemberField();
            field.Attributes = MemberAttributes.Public | MemberAttributes.Const;
            field.Name = NamingHelper.GetName(nm, PropertyAccess.Property, FieldCase.Pascalcase);
            field.Type = new CodeTypeReference(typeof(string));
            field.InitExpression = new CodePrimitiveExpression(member.Name);
            return field;
        }

        private void Log(string message)
        {
            Context.Output.Write(string.Format("ActiveWriter: {0}", message));
        }

        private CodeTypeReference GetConcreteListType(string sourceClassName)
        {
            CodeTypeReference fieldCreatorType = new CodeTypeReference(Context.Model.EffectiveListClass);
            fieldCreatorType.TypeArguments.Add(sourceClassName);
            return fieldCreatorType;
        }

        private void GenerateHelperClass(CodeNamespace nameSpace)
        {
            if (Context.Model.Target == CodeGenerationTarget.ActiveRecord)
            {
                CodeTypeDeclaration helperClass = new CodeTypeDeclaration(Context.HelperClassName);
                nameSpace.Types.Add(helperClass);
                CodeMemberMethod getTypesMethod = new CodeMemberMethod();
                helperClass.Members.Add(getTypesMethod);
                getTypesMethod.Name = "GetTypes";
                getTypesMethod.ReturnType = new CodeTypeReference("Type", 1);
                getTypesMethod.Attributes = MemberAttributes.Public | MemberAttributes.Static;

                CodeArrayCreateExpression arrayCreate = new CodeArrayCreateExpression("Type");
                foreach (ModelClass modelClass in Context.Model.Classes)
                    arrayCreate.Initializers.Add(new CodeTypeOfExpression(modelClass.Name));

                getTypesMethod.Statements.Add(new CodeMethodReturnStatement(arrayCreate));
            }
        }

        private void GenerateDerivedClass(ModelClass cls, CodeNamespace nameSpace)
        {
            if (cls.Model.GeneratesDoubleDerived)
            {
                CodeTypeDeclaration derivedClass = new CodeTypeDeclaration(cls.Name);
                derivedClass.IsPartial = true;
                derivedClass.TypeAttributes = TypeAttributes.Public;
                derivedClass.BaseTypes.Add(new CodeTypeReference(cls.Name + cls.Model.DoubleDerivedNameSuffix));
                cls.AddActiveRecordAttributes(derivedClass);

                nameSpace.Types.Add(derivedClass);
            }
        }

        /// <summary>
        /// If automatic associations are enabled, this code generates a class that will be used to access a special internal property.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="nameSpace"></param>
        private void GenerateInternalPropertyAccessor(CodeGenerationContext context, Model model, CodeNamespace nameSpace)
        {
            if (model.AutomaticAssociations)
            {
                /* Generates a class that looks something like this:
                 *     public class InternalPropertyAccessor : IPropertyAccessor
                 *     {
                 *         BasicPropertyAccessor accessor = new BasicPropertyAccessor();
                 *         
                 *         public IGetter GetGetter(Type theClass, string propertyName)
                 *         {
                 *             return accessor.GetGetter(theClass, propertyName + "Internal");
                 *         }
                 *         
                 *         public ISetter GetSetter(Type theClass, string propertyName)
                 *         {
                 *             return accessor.GetSetter(theClass, propertyName + "Internal");
                 *         }
                 *         
                 *         public bool CanAccessTroughReflectionOptimizer
                 *         {
                 *             get { return false; }
                 *         }
                 */

                nameSpace.Imports.Add(new CodeNamespaceImport("NHibernate.Properties"));
                CodeTypeDeclaration accessorClass = new CodeTypeDeclaration(context.InternalPropertyAccessorName);
                nameSpace.Types.Add(accessorClass);
                accessorClass.BaseTypes.Add("IPropertyAccessor");

                CodeMemberField accessorField = new CodeMemberField("BasicPropertyAccessor", "accessor");
                accessorClass.Members.Add(accessorField);
                accessorField.InitExpression = new CodeObjectCreateExpression("BasicPropertyAccessor");

                accessorClass.Members.Add(GetterOrSetterMethod("Getter"));

                accessorClass.Members.Add(GetterOrSetterMethod("Setter"));

                CodeMemberProperty canAccessProperty = new CodeMemberProperty();
                accessorClass.Members.Add(canAccessProperty);
                canAccessProperty.Type = new CodeTypeReference("Boolean");
                canAccessProperty.Name = "CanAccessTroughReflectionOptimizer";
                canAccessProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                canAccessProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(false)));
            }
        }

        private CodeMemberMethod GetterOrSetterMethod(string getterOrSetter)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Get" + getterOrSetter;
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            method.ReturnType = new CodeTypeReference("I" + getterOrSetter);
            method.Parameters.Add(new CodeParameterDeclarationExpression("Type", "theClass"));
            method.Parameters.Add(new CodeParameterDeclarationExpression("String", "propertyName"));
            method.Statements.Add(
                new CodeMethodReturnStatement(
                    new CodeMethodInvokeExpression(
                        new CodeFieldReferenceExpression(null, "accessor"),
                        "Get" + getterOrSetter,
                        new CodeArgumentReferenceExpression("theClass"),
                        new CodeBinaryOperatorExpression(
                            new CodeArgumentReferenceExpression("propertyName"),
                            CodeBinaryOperatorType.Add,
                            new CodePrimitiveExpression("Internal")))));
            return method;
        }

        private void AddItemAddedRemovedMethods(CodeTypeDeclaration typeDeclaration, string thisClassName, string listPropertyName, string itemClassName, string propertyName, bool manyToMany)
        {
            typeDeclaration.Members.Add(MakeAddOrRemoveMethod(listPropertyName, thisClassName, itemClassName, propertyName, manyToMany, true));
            typeDeclaration.Members.Add(MakeAddOrRemoveMethod(listPropertyName, thisClassName, itemClassName, propertyName, manyToMany, false));
        }

        private CodeMemberMethod MakeAddOrRemoveMethod(string listPropertyName, string thisClassName, string itemClassName, string propertyName, bool manyToMany, bool add)
        {
            var method = new CodeMemberMethod();
            method.Name = listPropertyName + (add ? "ItemAdded" : "ItemRemoved");
            method.Attributes = MemberAttributes.Private;
            method.Parameters.Add(new CodeParameterDeclarationExpression(itemClassName, "item"));

            CodeExpression value = new CodeCastExpression(thisClassName, new CodeThisReferenceExpression());

            if (manyToMany)
            {
                if (add)
                {
                    method.Statements.Add(
                        new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(
                                new CodeMethodInvokeExpression(
                                    new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("item"), propertyName),
                                    "Contains", value),
                                CodeBinaryOperatorType.ValueEquality,
                                new CodePrimitiveExpression(false)
                            ),
                            new CodeExpressionStatement(
                                new CodeMethodInvokeExpression(
                                    new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("item"), propertyName),
                                    "Add", value))));
                }
                else
                {
                    method.Statements.Add(
                        new CodeConditionStatement(
                            new CodeMethodInvokeExpression(
                                new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("item"), propertyName),
                                "Contains", value),
                            new CodeExpressionStatement(
                                new CodeMethodInvokeExpression(
                                    new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("item"), propertyName),
                                    "Remove", value))));
                }
            }
            else
            {
                if (!add)
                    value = new CodePrimitiveExpression(null);

                method.Statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodeArgumentReferenceExpression("item"),
                            propertyName),
                        value));
            }

            return method;
        }

        private void AddConstructorForWatchedList(CodeTypeDeclaration typeDeclaration, CodeMemberField memberField, string listPropertyName)
        {
            CodeConstructor constructor = typeDeclaration.FindEmptyConstructor();

            var field = new CodeFieldReferenceExpression(null, memberField.Name);

            CodeTypeReference innerListType = GetConcreteListType(memberField.Type.TypeArguments[0].BaseType);
            CodeObjectCreateExpression innerList = new CodeObjectCreateExpression(innerListType);
            CodeObjectCreateExpression outerList = new CodeObjectCreateExpression(memberField.Type, innerList);
            CodeAssignStatement assignment = new CodeAssignStatement(field, outerList);
            constructor.Statements.Add(assignment);

            var itemAddedCode = new CodeMethodReferenceExpression(null, listPropertyName + "ItemAdded");
            constructor.Statements.Add(new CodeAttachEventStatement(field, "ItemAdded", itemAddedCode));

            var itemRemovedCode = new CodeMethodReferenceExpression(null, listPropertyName + "ItemRemoved");
            constructor.Statements.Add(new CodeAttachEventStatement(field, "ItemRemoved", itemRemovedCode));
        }
 
        private void AddInternalWatchedListProperty(CodeTypeDeclaration typeDeclaration, CodeTypeReference propertyTypeReference, string propertyName, string fieldName, bool implementPropertyChanged, bool implementPropertyChanging, PropertyAccess propertyAccess)
        {
            CodeMemberProperty property = new CodeMemberProperty();
            property.Name = propertyName + "Internal";
            property.Type = propertyTypeReference;

            var list = new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(null, fieldName), "List");

            property.GetStatements.Add(new CodeMethodReturnStatement(list));

            // If the user specifies that they access the data through the property, then we
            // would want the change events to fire as if the property was being set.  If the
            // field is being accessed directly, we want to avoid the change events.

            if (implementPropertyChanging && propertyAccess == PropertyAccess.Property)
                property.SetStatements.Add(new CodeMethodInvokeExpression(null, Context.Model.PropertyChangingMethodName, new CodePrimitiveExpression(propertyName)));

            property.SetStatements.Add(new CodeAssignStatement(list, new CodeArgumentReferenceExpression("value")));

            if (implementPropertyChanged && propertyAccess == PropertyAccess.Property)
                property.SetStatements.Add(new CodeMethodInvokeExpression(null, Context.Model.PropertyChangedMethodName, new CodePrimitiveExpression(propertyName)));

            typeDeclaration.Members.Add(property);
        }

        private CodeBinaryOperatorExpression InequalityExpression(CodeExpression left, CodeExpression right)
        {
            // This used to use ValueEquality to check the members, but it seems to make slightly more sense
            // to use IdentityEquality since it might be useful to change the reference being used even if
            // the values are equivalent. Additionally, CodeDOM makes it easier for us with IdentityEquality.
            return
                new CodeBinaryOperatorExpression(
                    left,
                    CodeBinaryOperatorType.IdentityInequality,
                    right);
        }

        private void GenerateCommonPrimaryKey(ModelClass cls, CodeTypeDeclaration classDeclaration)
        {
            // Generate a primary key property from the common primary key information in the model.
            // We only do so if no primary key is already present.
            if (!cls.PrimaryKeySpecifiedByUser)
            {
                if (cls.PrimaryKey != null)
                {
                    AddMemberField(classDeclaration, cls.PrimaryKey);
                }
            }
        }

        #endregion

        #endregion
    }
}
