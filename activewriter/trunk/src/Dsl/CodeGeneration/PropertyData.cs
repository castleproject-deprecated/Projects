using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using Altinoren.ActiveWriter.ARValidators;

namespace Altinoren.ActiveWriter.CodeGeneration
{

    using System;

    /// <summary>
    /// PropertyData represents either a ModelProperty or something like the
    /// common primary key.  This is somewhat ugly, but the model classes
    /// don't seem to like being modified directly.
    /// </summary>
    public class PropertyData
    {
        #region ModelProperty Mirror Properties

        public string Name { get; private set; }

        public InheritablePropertyAccess Access { get; private set; }
        public Accessor Accessor { get; private set; }
        private string Check { get; set; }
        public string Column { get; set; }
        public NHibernateType ColumnType { get; set; }
        // Composite Key Name Missing.
        private string CustomAccess { get; set; }
        private string CustomColumnType { get; set; }
        public string CustomMemberType { get; private set; }
        public bool DebuggerDisplay { get; private set; }
        public bool DefaultMember { get; private set; }
        private string ColumnDefault { get; set; }
        public string Description { get; private set; }
        private string Formula { get; set; }
        public PrimaryKeyType Generator { get; set; }
        private string Index { get; set; }
        private bool Insert { get; set; }
        public KeyType KeyType { get; set; }
        private int Length { get; set; }
        public bool NotNull { get; set; }
        private string Params { get; set; }
        public PropertyType PropertyType { get; private set; }
        private string SequenceName { get; set; }
        private string SqlType { get; set; }
        private bool Unique { get; set; }
        private string UniqueKey { get; set; }
        private string UnsavedValue { get; set; }
        private bool Update { get; set; }
        private string ValidatorPropertyStorage { get; set; }

        public ModelClass ModelClass { get; private set; }
        private NestedClass NestedClass { get; set; }

        #endregion

        private PropertyData(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Property name cannot be null.");
            Name = name;

            Access = InheritablePropertyAccess.Inherit;
            Accessor = Accessor.Public;
            Check = null;
            Column = null;
            ColumnType = NHibernateType.String;
            CustomAccess = null;
            CustomColumnType = null;
            CustomMemberType = null;
            DebuggerDisplay = false;
            DefaultMember = false;
            ColumnDefault = null;
            Description = null;
            Formula = null;
            Generator = PrimaryKeyType.Native;
            Index = null;
            Insert = true;
            KeyType = KeyType.None;
            Length = 0;
            NotNull = false;
            Params = null;
            PropertyType = PropertyType.Property;
            SequenceName = null;
            SqlType = null;
            Unique = false;
            UniqueKey = null;
            UnsavedValue = null;
            Update = true;
            ValidatorPropertyStorage = null;
        }

        public PropertyData(string name, ModelClass modelClass)
            : this(name)
        {
            ModelClass = modelClass;
        }

        public PropertyData(string name, NestedClass nestedClass)
            : this(name)
        {
            NestedClass = nestedClass;
        }

        public PropertyData(ModelProperty p)
        {
            Name = p.Name;

            Access = p.Access;
            Accessor = p.Accessor;
            Check = p.Check;
            Column = p.Column;
            ColumnType = p.ColumnType;
            CustomAccess = p.CustomAccess;
            CustomColumnType = p.CustomColumnType;
            CustomMemberType = p.CustomMemberType;
            DebuggerDisplay = p.DebuggerDisplay;
            DefaultMember = p.DefaultMember;
            ColumnDefault = p.ColumnDefault;
            Description = p.Description;
            Formula = p.Formula;
            Generator = p.Generator;
            Index = p.Index;
            Insert = p.Insert;
            KeyType = p.KeyType;
            Length = p.Length;
            NotNull = p.NotNull;
            Params = p.Params;
            PropertyType = p.PropertyType;
            SequenceName = p.SequenceName;
            SqlType = p.SqlType;
            Unique = p.Unique;
            UniqueKey = p.UniqueKey;
            UnsavedValue = p.UnsavedValue;
            Update = p.Update;
            ValidatorPropertyStorage = p.GetValidatorValue();
            
            ModelClass = p.ModelClass;
            NestedClass = p.NestedClass;
        }

        public bool IsJoinedKey
        {
            get
            {
                return ModelClass != null && ModelClass.IsJoinedSubclass && KeyType == KeyType.PrimaryKey;
            }
        }

        public PropertyAccess EffectiveAccess
        {
            get
            {
                if (Access == InheritablePropertyAccess.Inherit)
                {
                    if (ModelClass != null)
                        return ModelClass.EffectiveAccess;
                    if (NestedClass != null)
                        return NestedClass.EffectiveAccess;
                    throw new InvalidOperationException("We must have either a NestedClass or ModelClass parent.");
                }

                return Access.GetMatchingPropertyAccess();
            }
        }

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

        public bool ImplementsINotifyPropertyChanging()
        {
            return (ModelClass != null
                        ? ModelClass.DoesImplementINotifyPropertyChanging()
                        : NestedClass.DoesImplementINotifyPropertyChanging());
        }

        public CodeAttributeDeclaration GetPrimaryKeyAttribute()
        {
            if (ModelClass.IsJoinedSubclass)
            {
                CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("JoinedKey");

                if (!string.IsNullOrEmpty(Column))
                    attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(Column));

                return attribute;
            }
            else
            {
                CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("PrimaryKey");

                attribute.Arguments.Add(AttributeHelper.GetPrimitiveEnumAttributeArgument("PrimaryKeyType", Generator));
                if (!string.IsNullOrEmpty(Column))
                    attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(Column));
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("ColumnType", ColumnType.ToString()));

                if (!string.IsNullOrEmpty(CustomAccess))
                    attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", CustomAccess));
                else if (EffectiveAccess != PropertyAccess.Property)
                    attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", EffectiveAccess));

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
                    result[i] = ((AbstractValidation)list[i]).GetAttributeDeclaration();

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

            if (!string.IsNullOrEmpty(CustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", CustomAccess));
            else if (EffectiveAccess != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", EffectiveAccess));

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

            if (!string.IsNullOrEmpty(CustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", CustomAccess));
            else if (EffectiveAccess != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", EffectiveAccess));

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
            if (!string.IsNullOrEmpty(ColumnDefault))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Default", ColumnDefault));
        }

        private void PopulateVersionAttribute(CodeAttributeDeclaration attribute)
        {
            if (!string.IsNullOrEmpty(Column))
                attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(Column));
            attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Type", ColumnType.ToString()));

            if (!string.IsNullOrEmpty(CustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", CustomAccess));
            else if (EffectiveAccess != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", EffectiveAccess));
        }

        private void PopulateTimestampAttribute(CodeAttributeDeclaration attribute)
        {
            if (!string.IsNullOrEmpty(Column))
                attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(Column));

            if (!string.IsNullOrEmpty(CustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", CustomAccess));
            else if (EffectiveAccess != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", EffectiveAccess));
        }

        #endregion

        public ArrayList GetValidatorsAsArrayList()
        {
            if (IsValidatorSet)
            {
                return ModelProperty.DeserializeValidatorList(ValidatorPropertyStorage);
            }

            return null;
        }

        public bool IsValidatorSet
        {
            get { return !string.IsNullOrEmpty(ValidatorPropertyStorage); }
        }
    }
}