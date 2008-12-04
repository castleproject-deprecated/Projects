using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeGeneration.Adapters
{
    public class ClassAdapter : CodeTypeDeclaration
    {
        private const string GenericListInterface = "IList";
        private const string GenericListClass = "List";

        // CreateClass
        public ClassAdapter(string name)
        {
            InitClass(name);
        }

        // GetClassDeclaration
        public ClassAdapter(CodeGenerationContext codeGenerationContext, ModelClass cls, CodeNamespace nameSpace)
        {
            if (cls == null)
                throw new ArgumentException("Class not supplied.", "cls");
            if (String.IsNullOrEmpty(cls.Name))
                throw new ArgumentException("Class name cannot be blank.", "cls");

            InitClass(cls.Name);

            if (cls.Model.UseBaseClass)
            {
                SetBaseType(cls);
            }

            AddPropertyNotifiers(
                cls.DoesImplementINotifyPropertyChanged(),
                cls.DoesImplementINotifyPropertyChanging());

            if (!String.IsNullOrEmpty(cls.Description))
            {
                Comments.AddRange(new CommentAdapter(cls.Description));
            }

            CustomAttributes.Add(cls.GetActiveRecordAttribute());
            if (cls.Model.UseGeneratedCodeAttribute)
            {
                CustomAttributes.Add(AttributeHelper.GetGeneratedCodeAttribute());
            }

            // Make a note as "generated" to prevent recursive generation attempts
            codeGenerationContext.RegisterClass(cls, this);

            nameSpace.Types.Add(this);
        }

        // GetNestedClassDeclaration
        public ClassAdapter(CodeGenerationContext codeGenerationContext, NestedClass cls, CodeNamespace nameSpace)
        {
            if (cls == null)
                throw new ArgumentException("Nested class not supplied.", "cls");
            if (String.IsNullOrEmpty(cls.Name))
                throw new ArgumentException("Nested class name cannot be blank.", "cls");

            InitClass(cls.Name);

            AddPropertyNotifiers(
                cls.DoesImplementINotifyPropertyChanged(),
                cls.DoesImplementINotifyPropertyChanging());

            if (!String.IsNullOrEmpty(cls.Description))
            {
                Comments.AddRange(new CommentAdapter(cls.Description));
            }

            if (cls.Model.UseGeneratedCodeAttribute)
            {
                CustomAttributes.Add(AttributeHelper.GetGeneratedCodeAttribute());
            }
            // Make a note as "generated" to prevent recursive generation attempts
            codeGenerationContext.RegisterClass(cls, this);

            nameSpace.Types.Add(this);
        }

        // GetCompositeClassDeclaration
        public ClassAdapter(CodeGenerationContext codeGenerationContext,
                            CodeNamespace nameSpace,
                            CodeTypeDeclaration parentClass,
                            List<ModelProperty> keys, bool implementINotifyPropertyChanged)
        {
            if (keys == null || keys.Count <= 1)
                throw new ArgumentException("Composite keys must consist of two or more properties.", "keys");

            string className = GetClassName(keys, parentClass);

            InitClass(className);

            AddPropertyNotifiers(implementINotifyPropertyChanged, false);

            CustomAttributes.Add(new CodeAttributeDeclaration("Serializable"));

            if (keys[0].ModelClass.Model.UseGeneratedCodeAttribute)
            {
                CustomAttributes.Add(AttributeHelper.GetGeneratedCodeAttribute());
            }

            List<CodeMemberField> fields = GetDescriptionFields(codeGenerationContext, keys);

            Members.Add(MethodAdapter.GetCompositeClassToStringMethod(fields));
            Members.Add(MethodAdapter.GetCompositeClassEqualsMethod(className, fields));
            Members.AddRange(MethodAdapter.GetCompositeClassGetHashCodeMethods(fields, codeGenerationContext.OutputVisualBasic));

            nameSpace.Types.Add(this);
        }

        private List<CodeMemberField> GetDescriptionFields(CodeGenerationContext codeGenerationContext, List<ModelProperty> keys)
        {
            List<CodeMemberField> fields = new List<CodeMemberField>();
            List<string> descriptions = new List<string>();
            foreach (ModelProperty property in keys)
            {
                CodeMemberField memberField = new FieldAdapter(codeGenerationContext, property, Accessor.Private);
                Members.Add(memberField);
                fields.Add(memberField);

                Members.Add(new KeyPropertyAdapter(codeGenerationContext, memberField, property));

                if (!String.IsNullOrEmpty(property.Description))
                    descriptions.Add(property.Description);
            }

            if (descriptions.Count > 0)
            {
                Comments.AddRange(new CommentAdapter(descriptions.ToArray()));
            }

            return fields;
        }

        private string GetClassName(List<ModelProperty> keys, CodeTypeDeclaration parentClass)
        {
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
            {
                className = parentClass.Name + Common.CompositeClassNameSuffix;
            }
            return className;
        }


        private void InitClass(string name)
        {
            Name = name;
            TypeAttributes = System.Reflection.TypeAttributes.Public;
            IsPartial = true;
        }

        private void AddPropertyNotifiers(bool addChangedNotifier, bool addChangingNotifier)
        {
            if (addChangedNotifier)
            {
                BaseTypes.Add(new CodeTypeReference(Common.INotifyPropertyChangedType));
                AddINotifyPropertyChangedRegion();
            }
            if (addChangingNotifier)
            {
                BaseTypes.Add(new CodeTypeReference(Common.INotifyPropertyChangingType));
                AddINotifyPropertyChangingRegion();
            }
        }

        private void SetBaseType(ModelClass cls)
        {
            bool withValidator = cls.Properties.Exists(delegate(ModelProperty property)
            {
                return property.IsValidatorSet();
            });

            CodeTypeReference type = GetTypeReference(cls, withValidator);

            if (cls.IsGeneric())
                type.TypeArguments.Add(Name);

            BaseTypes.Add(type);
        }

        private static CodeTypeReference GetTypeReference(ModelClass cls, bool withValidator)
        {
            string typeName;

            // base class for every modelclass. If left empty then baseclass from model if left empty ...etc
            if (!string.IsNullOrEmpty(cls.BaseClassName))
            {
                typeName = cls.BaseClassName;
            }
            else if (!string.IsNullOrEmpty(cls.Model.BaseClassName))
            {
                typeName = cls.Model.BaseClassName;
            }
            else
            {
                typeName = withValidator ? Common.DefaultValidationBaseClass : Common.DefaultBaseClass;
            }

            return new CodeTypeReference(typeName);
        }

        private void AddINotifyPropertyChangedRegion()
        {
            CodeMemberEvent memberEvent = new CodeMemberEvent
            {
                Attributes = MemberAttributes.Public,
                Name = "PropertyChanged",
                Type = new CodeTypeReference("PropertyChangedEventHandler")
            };
            memberEvent.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "INotifyPropertyChanged Members"));
            Members.Add(memberEvent);

            CodeMemberMethod propertyChangedMethod = new CodeMemberMethod
            {
                Attributes = MemberAttributes.Family,
                Name = Common.PropertyChangedInternalMethod
            };
            propertyChangedMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));
            propertyChangedMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "information"));
            Members.Add(propertyChangedMethod);

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

        private void AddINotifyPropertyChangingRegion()
        {
            CodeMemberEvent memberEvent = new CodeMemberEvent
            {
                Attributes = MemberAttributes.Public,
                Name = "PropertyChanging",
                Type = new CodeTypeReference("PropertyChangingEventHandler")
            };
            memberEvent.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "INotifyPropertyChanging Members"));
            Members.Add(memberEvent);

            CodeMemberMethod propertyChangingMethod = new CodeMemberMethod
            {
                Attributes = MemberAttributes.Family,
                Name = Common.PropertyChangingInternalMethod
            };
            propertyChangingMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "information"));
            propertyChangingMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));
            Members.Add(propertyChangingMethod);

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

        #region Relation

        #region HasMany

        public void GenerateHasManyRelation(CodeGenerationContext codeGenerationContext,
                                            CodeNamespace nameSpace,
                                            ManyToOneRelation relationship)
        {
            CodeTypeDeclaration sourceClass = new ClassAdapter(codeGenerationContext, relationship.Source, nameSpace);

            string propertyName = String.IsNullOrEmpty(relationship.TargetPropertyName)
                                      ? NamingHelper.GetPlural(sourceClass.Name)
                                      : NamingHelper.GetPlural(relationship.TargetPropertyName);
            string propertyType = String.IsNullOrEmpty(relationship.TargetPropertyType)
                                      ? GenericListInterface
                                      : relationship.TargetPropertyType;

            CodeMemberField memberField;
            if (!relationship.Source.AreRelationsGeneric())
                memberField = new FieldAdapter(codeGenerationContext, propertyName, propertyType, Accessor.Private, relationship.TargetAccess);
            else
                memberField = new FieldAdapter(codeGenerationContext, sourceClass.Name, propertyName, propertyType, Accessor.Private, relationship.TargetAccess);
            Members.Add(memberField);

            Model model = codeGenerationContext.Model;

            // initalize field
            if (model.InitializeIListFields && propertyType == GenericListInterface)
            {
                CodeObjectCreateExpression fieldCreator = new CodeObjectCreateExpression();

                CodeTypeReference fieldCreatorType = new CodeTypeReference(GenericListClass);

                fieldCreatorType.TypeArguments.Add(sourceClass.Name);
                fieldCreator.CreateType = fieldCreatorType;

                memberField.InitExpression = fieldCreator;
            }

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.TargetDescription))
                memberProperty = new PropertyAdapter(codeGenerationContext, memberField, propertyName, true, true, relationship.Target.DoesImplementINotifyPropertyChanged(), relationship.Target.DoesImplementINotifyPropertyChanging(), null);
            else
                memberProperty =
                    new PropertyAdapter(codeGenerationContext, memberField, propertyName, true, true, relationship.Target.DoesImplementINotifyPropertyChanged(), relationship.Target.DoesImplementINotifyPropertyChanging(),
                                      relationship.TargetDescription);
            Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(relationship.GetHasManyAttribute(codeGenerationContext));
        }

        #endregion

        #region BelongsTo

        public void GenerateBelongsToRelation(CodeGenerationContext codeGenerationContext,
                                              CodeNamespace nameSpace,
                                              ManyToOneRelation relationship)
        {
            if (!String.IsNullOrEmpty(relationship.TargetColumnKey) && (!String.IsNullOrEmpty(relationship.SourceColumn)) &&
                !relationship.SourceColumn.ToUpperInvariant().Equals(relationship.TargetColumnKey.ToUpperInvariant()))
                throw new ArgumentException(
                    String.Format(
                        "Class {0} column name does not match with column key {1} on it's many to one relation to class {2}",
                        relationship.Source.Name, relationship.TargetColumnKey, relationship.Target.Name));

            CodeTypeDeclaration targetClass = new ClassAdapter(codeGenerationContext, relationship.Target, nameSpace);

            string propertyName = String.IsNullOrEmpty(relationship.SourcePropertyName)
                                      ? targetClass.Name
                                      : relationship.SourcePropertyName;

            // We use PropertyAccess.Property to default it to camel case underscore
            CodeMemberField memberField = new FieldAdapter(codeGenerationContext, propertyName, targetClass.Name, Accessor.Private, PropertyAccess.Property);
            Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.SourceDescription))
                memberProperty = new PropertyAdapter(codeGenerationContext, memberField, propertyName, true, true, relationship.Source.DoesImplementINotifyPropertyChanged(), relationship.Source.DoesImplementINotifyPropertyChanging(), null);
            else
                memberProperty =
                    new PropertyAdapter(codeGenerationContext, memberField, propertyName, true, true, relationship.Source.DoesImplementINotifyPropertyChanged(), relationship.Source.DoesImplementINotifyPropertyChanging(),
                                      relationship.SourceDescription);
            Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(relationship.GetBelongsToAttribute());
        }

        #endregion

        #region HasAndBelongsToRelation

        public void GenerateHasAndBelongsToRelationFromTargets(CodeGenerationContext codeGenerationContext,
                                                               CodeNamespace nameSpace,
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

            CodeTypeDeclaration targetClass = new ClassAdapter(codeGenerationContext, relationship.Target, nameSpace);

            string propertyName = String.IsNullOrEmpty(relationship.SourcePropertyName)
                                      ? NamingHelper.GetPlural(relationship.Source.Name)
                                      : NamingHelper.GetPlural(relationship.SourcePropertyName);

            string propertyType = String.IsNullOrEmpty(relationship.TargetPropertyType)
                                      ? GenericListInterface
                                      : relationship.TargetPropertyType;

            CodeMemberField memberField;
            if (!relationship.Source.AreRelationsGeneric())
                memberField = new FieldAdapter(codeGenerationContext, propertyName, propertyType, Accessor.Private, relationship.TargetAccess);
            else
                memberField = new FieldAdapter(codeGenerationContext, targetClass.Name, propertyName, propertyType, Accessor.Private, relationship.TargetAccess);

            Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.SourceDescription))
                memberProperty = new PropertyAdapter(codeGenerationContext, memberField, NamingHelper.GetPlural(propertyName), true, true, relationship.Target.DoesImplementINotifyPropertyChanged(), relationship.Target.DoesImplementINotifyPropertyChanging(), null);
            else
                memberProperty =
                    new PropertyAdapter(codeGenerationContext, memberField, NamingHelper.GetPlural(propertyName), true, true, relationship.Target.DoesImplementINotifyPropertyChanged(), relationship.Target.DoesImplementINotifyPropertyChanging(),
                                      relationship.SourceDescription);
            Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(relationship.GetHasAndBelongsToAttributeFromSource());
        }

        public void GenerateHasAndBelongsToRelationFromSources(CodeGenerationContext codeGenerationContext,
                                                               CodeNamespace nameSpace,
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

            CodeTypeDeclaration sourceClass = new ClassAdapter(codeGenerationContext, relationship.Source, nameSpace);

            string propertyName = String.IsNullOrEmpty(relationship.TargetPropertyName)
                          ? NamingHelper.GetPlural(relationship.Target.Name)
                          : relationship.TargetPropertyName;

            string propertyType = String.IsNullOrEmpty(relationship.SourcePropertyType)
                                      ? GenericListInterface
                                      : relationship.SourcePropertyType;

            CodeMemberField memberField;
            if (!relationship.Source.AreRelationsGeneric())
                memberField = new FieldAdapter(codeGenerationContext, propertyName, propertyType, Accessor.Private, relationship.SourceAccess);
            else
                memberField = new FieldAdapter(codeGenerationContext, sourceClass.Name, propertyName, propertyType, Accessor.Private, relationship.SourceAccess);

            Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.TargetDescription))
                memberProperty = new PropertyAdapter(codeGenerationContext, memberField, propertyName, true, true, relationship.Source.DoesImplementINotifyPropertyChanged(), relationship.Source.DoesImplementINotifyPropertyChanging(), null);
            else
                memberProperty =
                    new PropertyAdapter(codeGenerationContext, memberField, propertyName, true, true, relationship.Source.DoesImplementINotifyPropertyChanged(), relationship.Source.DoesImplementINotifyPropertyChanging(),
                                      relationship.TargetDescription);
            Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(relationship.GetHasAndBelongsToAttributeFromTarget());
        }

        #endregion

        #region OneToOne

        public void GenerateOneToOneRelationFromTarget(CodeGenerationContext codeGenerationContext,
                                                       CodeNamespace nameSpace,
                                                       OneToOneRelation relationship)
        {
            CodeTypeDeclaration targetClass = new ClassAdapter(codeGenerationContext, relationship.Target, nameSpace);

            CodeMemberField memberField = new FieldAdapter(codeGenerationContext, targetClass.Name, targetClass.Name, Accessor.Private, relationship.TargetAccess);
            Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.SourceDescription))
                memberProperty = new PropertyAdapter(codeGenerationContext, memberField, targetClass.Name, true, true, relationship.Target.DoesImplementINotifyPropertyChanged(), relationship.Target.DoesImplementINotifyPropertyChanging(), null);
            else
                memberProperty =
                    new PropertyAdapter(codeGenerationContext, memberField, targetClass.Name, true, true, relationship.Target.DoesImplementINotifyPropertyChanged(), relationship.Target.DoesImplementINotifyPropertyChanging(),
                                      relationship.SourceDescription);
            Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(relationship.GetOneToOneAttributeForSource());
        }

        public void GenerateOneToOneRelationFromSources(CodeGenerationContext codeGenerationContext,
                                                        CodeNamespace nameSpace,
                                                        OneToOneRelation relationship)
        {
            CodeTypeDeclaration sourceClass = new ClassAdapter(codeGenerationContext, relationship.Source, nameSpace);

            CodeMemberField memberField = new FieldAdapter(codeGenerationContext, sourceClass.Name, sourceClass.Name, Accessor.Private, relationship.SourceAccess);
            Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.TargetDescription))
                memberProperty = new PropertyAdapter(codeGenerationContext, memberField, sourceClass.Name, true, true, relationship.Source.DoesImplementINotifyPropertyChanged(), relationship.Source.DoesImplementINotifyPropertyChanging(), null);
            else
                memberProperty =
                    new PropertyAdapter(codeGenerationContext, memberField, sourceClass.Name, true, true, relationship.Source.DoesImplementINotifyPropertyChanged(), relationship.Source.DoesImplementINotifyPropertyChanging(),
                                      relationship.TargetDescription);
            Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(relationship.GetOneToOneAttributeForTarget());
        }

        #endregion

        #region Nested

        public void GenerateNestingRelationFromRelationship(CodeGenerationContext codeGenerationContext,
                                                            CodeNamespace nameSpace,
                                                            NestedClassReferencesModelClasses relationship)
        {
            CodeTypeDeclaration sourceClass = new ClassAdapter(codeGenerationContext, relationship.NestedClass, nameSpace);

            string propertyName = String.IsNullOrEmpty(relationship.PropertyName)
                          ? sourceClass.Name
                          : relationship.PropertyName;

            CodeMemberField memberField = new FieldAdapter(codeGenerationContext, propertyName, sourceClass.Name, Accessor.Private, PropertyAccess.Property);
            Members.Add(memberField);

            CodeMemberProperty memberProperty;
            if (String.IsNullOrEmpty(relationship.Description))
                memberProperty = new PropertyAdapter(codeGenerationContext, memberField, propertyName, true, true, relationship.ModelClass.DoesImplementINotifyPropertyChanged(), relationship.ModelClass.DoesImplementINotifyPropertyChanging(), null);
            else
                memberProperty =
                    new PropertyAdapter(codeGenerationContext, memberField, propertyName, true, true, relationship.ModelClass.DoesImplementINotifyPropertyChanged(), relationship.ModelClass.DoesImplementINotifyPropertyChanging(),
                                      relationship.Description);
            Members.Add(memberProperty);

            memberProperty.CustomAttributes.Add(relationship.GetNestedAttribute());
        }

        #endregion

        #endregion
    }
}
