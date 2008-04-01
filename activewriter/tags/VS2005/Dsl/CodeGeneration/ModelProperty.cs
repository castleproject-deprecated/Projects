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

using System;
using System.Collections;
using Altinoren.ActiveWriter.ARValidators;

namespace Altinoren.ActiveWriter
{
    using System.CodeDom;
    using CodeGeneration;

    public partial class ModelProperty
    {
        #region Public Static Methods

        public static bool IsMetaDataGeneratable(CodeTypeMember member)
        {
            foreach (CodeAttributeDeclaration attribute in member.CustomAttributes)
            {
                if (attribute.Name == "PrimaryKey" || attribute.Name == "KeyProperty" || attribute.Name == "Field" || attribute.Name == "Property" || attribute.Name == "Version" || attribute.Name == "Timestamp")
                    return true;
            }

            return false;
        }

        #endregion


        #region Public Code Generation Methods

        public bool ImplementsINotifyPropertyChanged()
        {
            return (ModelClass != null
                        ? ModelClass.DoesImplementINotifyPropertyChanged()
                        : NestedClass.DoesImplementINotifyPropertyChanged());
        }

        public CodeAttributeDeclaration GetPrimaryKeyAttribute()
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("PrimaryKey");

            attribute.Arguments.Add(AttributeHelper.GetPrimitiveEnumAttributeArgument("PrimaryKeyType", Generator));
            if (!string.IsNullOrEmpty(Column))
                attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(Column));
            attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("ColumnType", ColumnType.ToString()));
            if (Access != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", Access));
            if (!string.IsNullOrEmpty(CustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", CustomAccess));
            if (Length > 0)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Length", Length));
            if (!string.IsNullOrEmpty(Params))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Params", Params));
            if (!string.IsNullOrEmpty(SequenceName))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("SequenceName", SequenceName));
            if (!string.IsNullOrEmpty(UnsavedValue))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("UnsavedValue", UnsavedValue));

            return attribute;
        }

        public CodeAttributeDeclaration GetFieldAttribute()
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("Field");
            PopulatePropertyOrFieldAttribute(attribute);
            return attribute;
        }

        public CodeAttributeDeclaration GetKeyPropertyAttribute()
        {
            // Why KeyPropertyAttribute doesn't have the same constructor signature as it's base class PropertyAttribute?
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("KeyProperty");
            PopulateKeyPropertyAttribute(attribute);
            return attribute;
        }

        public CodeAttributeDeclaration GetPropertyAttribute()
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("Property");
            PopulatePropertyOrFieldAttribute(attribute);
            return attribute;
        }

        public CodeAttributeDeclaration GetVersionAttribute()
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("Version");
            PopulateVersionAttribute(attribute);
            return attribute;
        }

        public CodeAttributeDeclaration GetTimestampAttribute()
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("Timestamp");
            PopulateTimestampAttribute(attribute);
            return attribute;
        }

        public CodeAttributeDeclaration GetDebuggerDisplayAttribute()
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("System.Diagnostics.DebuggerDisplay");
            attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(Name, Name));
            return attribute;
        }

        public CodeAttributeDeclaration GetDefaultMemberAttribute()
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("System.Reflection.DefaultMember");
            attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(Name));
            return attribute;
        }

        public CodeAttributeDeclaration[] GetValidationAttributes()
        {
            ArrayList list = GetValidatorsAsArrayList();
            if (list != null && list.Count > 0)
            {
                CodeAttributeDeclaration[] result = new CodeAttributeDeclaration[list.Count];

                for (int i = 0; i < list.Count; i++)
                    result[i] = ((AbstractValidation) list[i]).GetAttributeDeclaration();

                return result;
            }

            return null;
        }

        #endregion

        #region Private Methods

        private void PopulateKeyPropertyAttribute(CodeAttributeDeclaration attribute)
        {
            if (!string.IsNullOrEmpty(Column))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Column", Column));
            attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("ColumnType", ColumnType.ToString()));
            if (Access != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", Access));
            if (!string.IsNullOrEmpty(CustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", CustomAccess));
            if (!string.IsNullOrEmpty(Formula))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Formula", Formula));
            if (!Insert)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Insert", Insert));
            if (Length > 0)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Length", Length));
            if (NotNull)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("NotNull", NotNull));
            if (Unique)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Unique", Unique));
            if (!string.IsNullOrEmpty(UnsavedValue))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("UnsavedValue", UnsavedValue));
            if (!Update)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Update", Update));
        }

        private void PopulatePropertyOrFieldAttribute(CodeAttributeDeclaration attribute)
        {
            if (!string.IsNullOrEmpty(Column))
                attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(Column));
            if (ColumnType == NHibernateType.Custom)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("ColumnType", CustomColumnType));
            else
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("ColumnType", ColumnType.ToString()));
            if (Access != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", Access));
            if (!string.IsNullOrEmpty(CustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", CustomAccess));
            if (!string.IsNullOrEmpty(Formula))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Formula", Formula));
            if (!Insert)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Insert", Insert));
            if (Length > 0)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Length", Length));
            if (NotNull)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("NotNull", NotNull));
            if (Unique)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Unique", Unique));
            if (!Update)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Update", Update));
            if (!string.IsNullOrEmpty(UniqueKey))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("UniqueKey", UniqueKey));
            if (!string.IsNullOrEmpty(Index))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Index", Index));
            if (!string.IsNullOrEmpty(SqlType))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("SqlType", SqlType));
            if (!string.IsNullOrEmpty(Check))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Check", Check));
        }

        private void PopulateVersionAttribute(CodeAttributeDeclaration attribute)
        {
            if (!string.IsNullOrEmpty(Column))
                attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(Column));
            attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Type", ColumnType.ToString()));
            if (Access != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", Access));
            if (!string.IsNullOrEmpty(CustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", CustomAccess));
        }

        private void PopulateTimestampAttribute(CodeAttributeDeclaration attribute)
        {
            if (!string.IsNullOrEmpty(Column))
                attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(Column));
            if (Access != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", Access));
            if (!string.IsNullOrEmpty(CustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", CustomAccess));
        }

        #endregion
    }
}
