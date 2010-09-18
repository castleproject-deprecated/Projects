#region License
//  Copyright 2004-2010 Castle Project - http:www.castleproject.org/
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http:www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// 
#endregion
namespace Castle.ActiveWriter
{
	using Microsoft.VisualStudio.Modeling.Validation;

	[ValidationState(ValidationState.Enabled)]
    public partial class ModelProperty
    {
        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateSpaceInPropertyName(ValidationContext context)
        {
            if (!string.IsNullOrEmpty(Name) && Name.IndexOf(" ") > 0)
            {
                context.LogError("Property names cannot contain spaces", "AW001ValidateSpaceInPropertyNameError", this);
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateStartsWithNumberInPropertyName(ValidationContext context)
        {
            int i;
            if (!string.IsNullOrEmpty(Name) && int.TryParse(Name.Substring(0, 1), out i))
            {
                context.LogError("Property names cannot start with a number",
                                 "AW001ValidateStartsWithNumberInPropertyNameError", this);
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateSequenceKeyHasSequenceName(ValidationContext context)
        {
            if (KeyType != KeyType.None && Generator == PrimaryKeyType.Sequence && string.IsNullOrEmpty(SequenceName))
            {
                context.LogError("Sequence generator requires a sequence name",
                                 "AW001ValidateSequenceKeyHasSequenceNameError", this);
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidatePrimaryKeyInVersionOrTimestamp(ValidationContext context)
        {
            if ((PropertyType == PropertyType.Timestamp || PropertyType == PropertyType.Version) &&
                KeyType != KeyType.None)
            {
                context.LogError("Versions and timestamps cannot be marked as keys.",
                                 "AW001ValidatePrimaryKeyInVersionOrTimestampError", this);
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateDebuggerDisplayInCompositeKey(ValidationContext context)
        {
            if (KeyType == KeyType.CompositeKey && DebuggerDisplay == true)
            {
                context.LogError("Composite key properties cannot be used as DebuggerDisplay Attributes.",
                                 "AW001ValidateDebuggerDisplayInCompositeKeyError", this);
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateValidatorInPrimaryKey(ValidationContext context)
        {
            if (KeyType == KeyType.PrimaryKey && IsValidatorSet())
            {
                context.LogWarning("Validators will be ignored in primary key.",
                                 "AW001ValidateValidatorInPrimaryKeyError", this);
            }
        }
        
        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateValidatorInCompositeKey(ValidationContext context)
        {
            if (KeyType == KeyType.CompositeKey && IsValidatorSet())
            {
                context.LogWarning("Validators will be ignored in composite keys.",
                                 "AW001ValidateValidatorInCompositeKeyError", this);
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateValidatorInVersion(ValidationContext context)
        {
            if (PropertyType == PropertyType.Version && IsValidatorSet())
            {
                context.LogWarning("Validators will be ignored on version property.",
                                 "AW001ValidateValidatorInVersionError", this);
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateValidatorInTimestamp(ValidationContext context)
        {
            if (PropertyType == PropertyType.Timestamp && IsValidatorSet())
            {
                context.LogWarning("Validators will be ignored on timestamp property.",
                                 "AW001ValidateValidatorInTimestampError", this);
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateValidatorInField(ValidationContext context)
        {
            if (PropertyType == PropertyType.Field && IsValidatorSet())
            {
                context.LogWarning("Validators will be ignored on field property.",
                                 "AW001ValidateValidatorInFieldError", this);
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateDataTypeOfVersionOrTimestamp(ValidationContext context)
        {
            if ((PropertyType == PropertyType.Timestamp || PropertyType == PropertyType.Version) &&
                ColumnType != NHibernateType.Int64 && ColumnType != NHibernateType.Int32 && ColumnType != NHibernateType.Int16 && ColumnType != NHibernateType.Ticks && ColumnType != NHibernateType.Timestamp && ColumnType != NHibernateType.TimeSpan)
            {
                context.LogError("Versions and timestamps can be of the following column types: Int64, Int32, Int16, Ticks, Timestamp, or TimeSpan.",
                                 "AW001ValidateDataTypeOfVersionOrTimestampError", this);
            }
        }

		[ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
		private void ValidateCustomTypePropertiesIfColumnTypeIsNotCustom(ValidationContext context)
		{
			if (ColumnType != NHibernateType.Custom && 
				(!string.IsNullOrEmpty(CustomColumnType) || !string.IsNullOrEmpty(CustomMemberType)))
			{
				context.LogWarning("CustomColumnType and CustomMemberType will be ignored if ColumnType is not Custom.",
								 "AW001CustomTypePropertiesIfColumnTypeIsNotCustom", this);
			}
		}

		[ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
		private void ValidateCustomTypePropertiesIfColumnTypeIsCustom(ValidationContext context)
		{
			if (ColumnType == NHibernateType.Custom &&
				(string.IsNullOrEmpty(CustomColumnType) || string.IsNullOrEmpty(CustomMemberType)))
			{
				context.LogError("CustomColumnType and CustomMemberType must be set if ColumnType is Custom.",
								 "AW001ValidateCustomTypePropertiesIfColumnTypeIsCustom", this);
			}
		}

		[ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
		private void ValidatePrimaryKeyInCustomType(ValidationContext context)
		{
			if (ColumnType == NHibernateType.Custom && KeyType != KeyType.None)
			{
				context.LogError("Custom types cannot be marked as keys.",
								 "AW001ValidatePrimaryKeyInCustomType", this);
			}
		}

	}
}
