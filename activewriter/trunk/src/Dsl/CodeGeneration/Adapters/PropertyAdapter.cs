using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeGeneration.Adapters
{
    public class UntypedPropertyAdapter : CodeMemberProperty
    {
        public UntypedPropertyAdapter(CodeGenerationContext codeGenerationContext, CodeMemberField memberField, string propertyName, bool get, bool set, bool implementINotifyPropertyChanged, bool implementINotifyPropertyChanging, params string[] description)
        //private CodeMemberProperty GetMemberPropertyWithoutType(CodeMemberField memberField, string propertyName,
        //                                                        bool get, bool set, bool implementINotifyPropertyChanged, bool implementINotifyPropertyChanging, params string[] description)
        {
            Name = propertyName;

            if (codeGenerationContext.Model.UseVirtualProperties)
                Attributes = MemberAttributes.Public;
            else
                Attributes = MemberAttributes.Public | MemberAttributes.Final;

            if (get)
            {
                GetStatements.Add(new CodeMethodReturnStatement(
                                      new CodeFieldReferenceExpression(
                                          new CodeThisReferenceExpression(), memberField.Name)));

            }

            if (set)
            {
                var assignValue = new CodeAssignStatement(
                                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), memberField.Name),
                                    new CodeArgumentReferenceExpression("value")
                                    );

                if (!implementINotifyPropertyChanged && !implementINotifyPropertyChanging)
                {
                    SetStatements.Add(assignValue);
                }
                else
                {
                    // There's no ValueInequality in CodeDOM (ridiculous), so we'll use ((a == b) == false) instead.
                    var equalityCheck = new CodeBinaryOperatorExpression(
                                            new CodeBinaryOperatorExpression(
                                                new CodeFieldReferenceExpression(null, memberField.Name),
                                                CodeBinaryOperatorType.ValueEquality,
                                                new CodeArgumentReferenceExpression("value")),
                                            CodeBinaryOperatorType.ValueEquality,
                                            new CodePrimitiveExpression(false)
                                        );

                    var assignment = new CodeConditionStatement(equalityCheck, assignValue);
                    SetStatements.Add(assignment);

                    if (implementINotifyPropertyChanged)
                    {
                        assignment.TrueStatements.Add(
                            new CodeExpressionStatement(
                                new CodeMethodInvokeExpression(
                                    new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),
                                                                             Common.PropertyChangedInternalMethod),
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
                                                                             Common.PropertyChangingInternalMethod),
                                           new CodePrimitiveExpression(propertyName)
                                           ))
                            );
                    }
                }
            }

            if (description != null && description.Length > 0)
            {
                Comments.AddRange(new CommentAdapter(description));
            }
        }
    }

    public class PropertyAdapter : UntypedPropertyAdapter
    {
        #region Property

        // TODO: All this property generation is a total mess. Lots of similar methods. Hard to find what you're looking for. Hard to change.

        public PropertyAdapter(CodeGenerationContext codeGenerationContext, CodeMemberField memberField, string propertyName, bool get, bool set, bool implementINotifyPropertyChanged, bool implementINotifyPropertyChanging, params string[] description)
            : base(codeGenerationContext, memberField, propertyName, get, set, implementINotifyPropertyChanged, implementINotifyPropertyChanging, description)
        {
            Type = memberField.Type;
        }

        public PropertyAdapter(CodeGenerationContext codeGenerationContext, CodeMemberField memberField, string propertyName,
                                                NHibernateType propertyType, string propertyCustomMemberType, bool propertyNotNull,
                                                bool get, bool set, bool implementINotifyPropertyChanged, bool implementINotifyPropertyChanging, params string[] description)
            : base(codeGenerationContext, memberField, propertyName, get, set, implementINotifyPropertyChanged, implementINotifyPropertyChanging, description)
        //private CodeMemberProperty GetMemberProperty(CodeMemberField memberField, string propertyName,
        //                                        NHibernateType propertyType, string propertyCustomMemberType, bool propertyNotNull,
        //                                        bool get, bool set, bool implementINotifyPropertyChanged, bool implementINotifyPropertyChanging, params string[] description)
        {
            Model model = codeGenerationContext.Model;

            if (model.UseNullables != NullableUsage.No && TypeHelper.IsNullable(propertyType) && !propertyNotNull)
            {
                if (model.UseNullables == NullableUsage.WithHelperLibrary)
                {
                    Type = TypeHelper.GetNullableTypeReferenceForHelper(propertyType);
                }
                else
                {
                    Type = TypeHelper.GetNullableTypeReference(propertyType);
                }
            }
            else
            {
                Type = new CodeTypeReference(TypeHelper.GetSystemType(propertyType, propertyCustomMemberType));
            }
        }

        #endregion
    }

    public class MemberPropertyAdapter : PropertyAdapter
    {
        public MemberPropertyAdapter(CodeGenerationContext codeGenerationContext, CodeMemberField memberField, ModelProperty property)
            : base(codeGenerationContext, memberField, property.Name, property.ColumnType,
                   property.CustomMemberType, property.NotNull, true, true,
                    property.ImplementsINotifyPropertyChanged(), property.ImplementsINotifyPropertyChanging(), property.Description)
        {
            CodeAttributeDeclaration attributeDeclaration = null;
            switch (property.KeyType)
            {
                // Composite keys must be handled in upper levels
                case KeyType.None:
                    attributeDeclaration = property.GetPropertyAttribute();
                    break;
                case KeyType.PrimaryKey:
                    attributeDeclaration = property.GetPrimaryKeyAttribute();
                    break;
            }
            if (attributeDeclaration != null)
            {
                CustomAttributes.Add(attributeDeclaration);
            }
        }
    }

    public class KeyPropertyAdapter : PropertyAdapter
    {
        public KeyPropertyAdapter(CodeGenerationContext codeGenerationContext, CodeMemberField memberField, ModelProperty property)
            : base(codeGenerationContext, memberField, property.Name, property.ColumnType,
                   null, property.NotNull, true, true,
                    property.ImplementsINotifyPropertyChanged(), property.ImplementsINotifyPropertyChanging(), property.Description)
        {
            CustomAttributes.Add(property.GetKeyPropertyAttribute());
        }
    }

    public class CompositeKeyPropertyAdapter : PropertyAdapter
    {
        public CompositeKeyPropertyAdapter(CodeGenerationContext codeGenerationContext, CodeTypeDeclaration compositeClass,
                                CodeMemberField memberField, bool implementsINotifyPropertyChanged, bool implementsINotifyPropertyChanging)
            : base(codeGenerationContext, memberField, compositeClass.Name, true, true, implementsINotifyPropertyChanged, implementsINotifyPropertyChanging, null)
        //private CodeTypeMember GetActiveRecordMemberCompositeKeyProperty(CodeTypeDeclaration compositeClass,
        //                                                                 CodeMemberField memberField, bool implementsINotifyPropertyChanged, bool implementsINotifyPropertyChanging)
        {
            // TODO: Composite key generation omits UnsavedValue property. All the properties which are parts of the composite key
            // should have the same UnsavedValue, by the way.

            CustomAttributes.Add(new CodeAttributeDeclaration("CompositeKey"));
        }
    }

    public class TimeStampPropertyAdapter : PropertyAdapter
    {
        public TimeStampPropertyAdapter(CodeGenerationContext codeGenerationContext, CodeMemberField memberField, ModelProperty property)
            : base(codeGenerationContext, memberField, property.Name, property.ColumnType, null, property.NotNull, true, true, property.ImplementsINotifyPropertyChanged(), property.ImplementsINotifyPropertyChanging(),
                              property.Description)
        {
            CustomAttributes.Add(property.GetTimestampAttribute());
        }
    }

    public class VersionPropertyAdapter : PropertyAdapter
    {
        public VersionPropertyAdapter(CodeGenerationContext codeGenerationContext, CodeMemberField memberField, ModelProperty property)
            : base(codeGenerationContext, memberField, property.Name, property.ColumnType, null, property.NotNull, true, true, property.ImplementsINotifyPropertyChanged(), property.ImplementsINotifyPropertyChanging(),
                              property.Description)
        {
            CustomAttributes.Add(property.GetVersionAttribute());
        }
    }
}
