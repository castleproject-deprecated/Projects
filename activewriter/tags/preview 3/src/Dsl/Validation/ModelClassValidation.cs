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

// Big TODO: Combine with CodeGenerationHelper validations in a seperate structure

namespace Altinoren.ActiveWriter
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.VisualStudio.Modeling.Validation;
    using Altinoren.ActiveWriter.Validation;

    [ValidationState(ValidationState.Enabled)]
    public partial class ModelClass
    {
        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidatePropertyNameAndDuplication(ValidationContext context)
        {
            if (Properties.Count > 0)
            {
                List<string> properties = new List<string>();

                foreach (ModelProperty property in Properties)
                {
                    if (string.IsNullOrEmpty(property.Name))
                    {
                        // Since we need the name for uniqueness validation, we check the existance of the name here.
                        context.LogError("Property must have a name", "AW001ValidatePropertyNameError", this);
                    }
                    else
                    {
                        if (properties.Contains(property.Name))
                            context.LogError("Property " + property.Name + " must have a unique name",
                                             "AW001ValidateDuplicatePropertyNameError", this);
                        else
                            properties.Add(property.Name);
                    }
                }
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateSpaceInName(ValidationContext context)
        {
            if (!string.IsNullOrEmpty(Name) && Name.IndexOf(" ") > 0)
            {
                context.LogError("Class names cannot contain spaces", "AW001ValidateSpaceInClassNameError", this);
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateStartsWithNumberInName(ValidationContext context)
        {
            int i;
            if (!string.IsNullOrEmpty(Name) && int.TryParse(Name.Substring(0, 1), out i))
            {
                context.LogError("Class names cannot start with a number",
                                 "AW001ValidateStartsWithNumberInClassNameError", this);
            }
        }
        
        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateManyToManyValidity(ValidationContext context)
        {
            ReadOnlyCollection<ManyToManyRelation> manyToManySources = ManyToManyRelation.GetLinksToManyToManySources(this);
            foreach (ManyToManyRelation relationship in manyToManySources)
            {
                if (String.IsNullOrEmpty(relationship.Table))
                    context.LogError(
                        String.Format("Class {0} does not have a table name on it's many to many relation to class {1}",
                                      relationship.Source.Name, relationship.Target.Name),
                        "AW001ValidateManyToManyValidity1Error", relationship);
                if (String.IsNullOrEmpty(relationship.SourceColumn))
                    context.LogError(
                        String.Format("Class {0} does not have a source column name on it's many to many relation to class {1}",
                                      relationship.Source.Name, relationship.Target.Name),
                        "AW001ValidateManyToManyValidity2Error", relationship);
                if (String.IsNullOrEmpty(relationship.TargetColumn))
                    context.LogError(
                        String.Format("Class {0} does not have a target column name on it's many to many relation to class {1}",
                                      relationship.Source.Name, relationship.Target.Name),
                        "AW001ValidateManyToManyValidity3Error", relationship);
            }
        }
        
        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateManyToOneValidity(ValidationContext context)
        {
            ReadOnlyCollection<ManyToOneRelation> manyToOneSources = ManyToOneRelation.GetLinksToSources(this);
            foreach (ManyToOneRelation relationship in manyToOneSources)
            {
                // WARNING: The comparison below does not take case sensitive column naming into account
                if (!String.IsNullOrEmpty(relationship.TargetColumnKey) && !String.IsNullOrEmpty(relationship.SourceColumn) &&
                    !relationship.SourceColumn.ToUpperInvariant().Equals(relationship.TargetColumnKey.ToUpperInvariant())
                    )
                    context.LogError(
                        String.Format(
                            "Class {0} column name does not match with column key {1} on it's many to one relation to class {2}",
                            relationship.Source.Name, relationship.TargetColumnKey, relationship.Target.Name),
                        "AW001ValidateManyToOneValidity2Error", relationship);
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateOneToOneValidity(ValidationContext context)
        {
            OneToOneRelation oneToOneAsSource = OneToOneRelation.GetLinkToOneToOneTarget(this);
            if (oneToOneAsSource != null)
            {
                if (oneToOneAsSource.Source == oneToOneAsSource.Target)
                {
                    context.LogError(
                        String.Format(
                            "Class {0} cannot have one to one relation with itself",
                            oneToOneAsSource.Source.Name),
                        "AW001ValidateOneToOneWithSelf1Error", oneToOneAsSource.Source);
                }
                else
                {
                    OneToOneRelation oneToOneAsTarget = OneToOneRelation.GetLinkToOneToOneTarget(oneToOneAsSource.Target);
                    if (oneToOneAsTarget != null && oneToOneAsTarget.Target == this)
                        context.LogError(
                            String.Format(
                                "Class {0} and {1} have a circular one to one relationship",
                                oneToOneAsTarget.Source.Name, oneToOneAsTarget.Target.Name),
                            "AW001ValidateOneToOneWithSelf2Error", oneToOneAsTarget.Source, oneToOneAsTarget.Target);
                }
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateMultiplePrimaryKeys(ValidationContext context)
        {
            if (Properties.Count > 0 && ValidationHelper.GetPrimaryKeyCount(Properties) > 1)
                context.LogError(
                    string.Format(
                        "Class {0} has multiple primary keys. Try using a composite key with multiple properties instead.",
                        Name), "AW001ValidateMultiplePrimaryKeysError", this);
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateCompositeKeyWithSingleProperty(ValidationContext context)
        {
            if (Properties.Count > 0 && ValidationHelper.GetCompositeKeyCount(Properties) == 1)
                context.LogError(
                    string.Format(
                        "Class {0} has a composite key defined ona a single property. Use multiple properties for the composite key or try using a primary key instead.",
                        Name), "AW001ValidateCompositeKeyWithSinglePropertyError", this);
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateBothPrimaryAndCompositeKey(ValidationContext context)
        {
            if (Properties.Count > 0 && ValidationHelper.GetCompositeKeyCount(Properties) > 0 && ValidationHelper.GetPrimaryKeyCount(Properties) > 0)
                context.LogError(
                    string.Format(
                        "Class {0} has both primary and composite key(s) defined. Try using either of them.",
                        Name), "AW001ValidateBothPrimaryAndCompositeKeyError", this);
        }
        
        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateAllCompositeKeysHaveSameAccess(ValidationContext context)
        {
            List<ModelProperty> composits = ValidationHelper.GetCompositeKeys(Properties);
            
            if (composits.Count > 1)
            {
                PropertyAccess access = composits[0].Access;

                if (composits.FindAll(
                    delegate(ModelProperty property)
                    {
						return (property.Access != access);
                    }
                    ).Count > 0)
                    context.LogError("All composite keys must have the same Access value.",
                                     "AW001ValidateAllCompositeKeysHaveSameAccessError", this);
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateMultipleDebuggerDisplays(ValidationContext context)
        {
            if (Properties.Count > 0 && ValidationHelper.GetDebuggerDisplayCount(Properties) > 1)
                context.LogError(
                    string.Format(
                        "Class {0} has multiple debugger display attributes set. Only one is allowed per class.",
                        Name), "AW001ValidateMultipleDebuggerDisplaysError", this);
        }
    }
}
