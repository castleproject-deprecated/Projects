using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeGeneration.Adapters
{
    public class FieldAdapter : CodeMemberField
    {
        #region Field

        public FieldAdapter(CodeGenerationContext codeGenerationContext, CodeTypeDeclaration compositeClass, PropertyAccess access)
            : this(codeGenerationContext, compositeClass.Name, compositeClass.Name, Accessor.Private, access)
        //private CodeMemberField GetPrivateMemberFieldOfCompositeClass(CodeTypeDeclaration compositeClass, PropertyAccess access)
        {
        }

        public FieldAdapter(CodeGenerationContext codeGenerationContext, ModelProperty property, Accessor accessor)
            : this(codeGenerationContext, property.Name, accessor, property.Access)
        //private CodeMemberField GetMemberFieldOfProperty(ModelProperty property, Accessor accessor)
        {
            Model model = codeGenerationContext.Model;

            if (model.UseNullables != NullableUsage.No && TypeHelper.IsNullable(property.ColumnType) && !property.NotNull)
            {
                if (model.UseNullables == NullableUsage.WithHelperLibrary)
                {
                    Type = TypeHelper.GetNullableTypeReferenceForHelper(property.ColumnType);
                }
                else
                {
                    Type = TypeHelper.GetNullableTypeReference(property.ColumnType);
                }
            }
            else
            {
                string customType = TypeHelper.GetSystemType(property.ColumnType, property.CustomMemberType);
                Type = new CodeTypeReference(customType);
            }
        }

        public FieldAdapter(CodeGenerationContext codeGenerationContext, CodeTypeDeclaration classDeclaration, ModelProperty property)
            : this(codeGenerationContext, property, Accessor.Private)
        //private CodeMemberField GetMemberField(CodeTypeDeclaration classDeclaration, ModelProperty property)
        {
            // Soooo ugly.
            switch (property.PropertyType)
            {
                case PropertyType.Property:
                    CodeMemberProperty memberProperty = new MemberPropertyAdapter(codeGenerationContext, this, property);
                    classDeclaration.Members.Add(memberProperty);
                    if (property.IsValidatorSet())
                        memberProperty.CustomAttributes.AddRange(property.GetValidationAttributes());
                    break;
                case PropertyType.Field:
                    CustomAttributes.Add(property.GetFieldAttribute());
                    break;
                case PropertyType.Version:
                    classDeclaration.Members.Add(new VersionPropertyAdapter(codeGenerationContext, this, property));
                    break;
                case PropertyType.Timestamp:
                    classDeclaration.Members.Add(new TimeStampPropertyAdapter(codeGenerationContext, this, property));
                    break;
            }
        }

        public FieldAdapter(CodeGenerationContext codeGenerationContext, string name, CodeTypeReference fieldType, Accessor accessor, PropertyAccess access)
            : this(codeGenerationContext, name, accessor, access)
        //private CodeMemberField GetMemberField(string name, CodeTypeReference fieldType, Accessor accessor, PropertyAccess access)
        {
            Type = fieldType;
        }

        public FieldAdapter(CodeGenerationContext codeGenerationContext, string name, string fieldType, Accessor accessor, PropertyAccess access)
            : this(codeGenerationContext, name, accessor, access)
        //private CodeMemberField GetMemberField(string name, string fieldType, Accessor accessor, PropertyAccess access)
        {
            Type = new CodeTypeReference(fieldType);
        }

        public FieldAdapter(CodeGenerationContext codeGenerationContext, string typeName, string name, string fieldType, Accessor accessor, PropertyAccess access)
            : this(codeGenerationContext, name, accessor, access)
        //private CodeMemberField GetGenericMemberField(string typeName, string name, string fieldType, Accessor accessor, PropertyAccess access)
        {
            CodeTypeReference type = new CodeTypeReference(fieldType);
            if (!TypeHelper.ContainsGenericDecleration(fieldType, codeGenerationContext.OutputVisualBasic))
            {
                type.TypeArguments.Add(typeName);
            }
            Type = type;
        }

        public FieldAdapter(CodeGenerationContext codeGenerationContext, string name, Accessor accessor, PropertyAccess access)
            : this(accessor)
        //private CodeMemberField GetMemberFieldWithoutType(string name, Accessor accessor, PropertyAccess access)
        {
            Name = NamingHelper.GetName(name, access, codeGenerationContext.Model.CaseOfPrivateFields);
        }

        public FieldAdapter(Accessor accessor)
        //private CodeMemberField GetMemberFieldWithoutTypeAndName(Accessor accessor)
        {
            switch (accessor)
            {
                case Accessor.Public:
                    Attributes = MemberAttributes.Public;
                    break;
                case Accessor.Protected:
                    Attributes = MemberAttributes.Family;
                    break;
                case Accessor.Private:
                    Attributes = MemberAttributes.Private;
                    break;
            }
        }

        public bool Static
        {
            get { return (Attributes & MemberAttributes.Static) != 0; }
            set
            {
                if (value)
                {
                    Attributes |= MemberAttributes.Static;
                }
                else
                {
                    Attributes ^= MemberAttributes.Static;
                }
            }
        }

        #endregion
    }
}
