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

namespace Altinoren.ActiveWriter
{
    using System.CodeDom;
    using CodeGeneration;
    using GeneratedCodeExtensions;

    public partial class ManyToOneRelation
    {
        public RelationType EffectiveRelationType
        {
            get
            {
                return (TargetRelationType == InheritedRelationType.Inherited
                            ? Source.Model.ManyToOneRelationType
                            : TargetRelationType.GetMatchingRelationType());
            }
        }

        public string EffectiveSourcePropertyName
        {
            get
            {
                return string.IsNullOrEmpty(SourcePropertyName)
                    ? Target.Name
                    : SourcePropertyName;
            }
        }

        public string EffectiveTargetPropertyName
        {
            get
            {
                // Don't pluralize custom SourcePropertyNames to allow for override of automatic pluralization.
                // If the user types it, they should know what they want.
                return string.IsNullOrEmpty(TargetPropertyName)
                           ? NamingHelper.GetPlural(Source.Name)
                           : TargetPropertyName;
            }
        }

        public string EffectiveSourceColumn
        {
            get
            {
                if (string.IsNullOrEmpty(SourceColumn) && !string.IsNullOrEmpty(Source.Model.ForeignKeyFormat))
                    return string.Format(Source.Model.ForeignKeyFormat, EffectiveSourcePropertyName);
                else
                    return SourceColumn;
            }
        }

        public CodeAttributeDeclaration GetHasManyAttribute(CodeGenerationContext context)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("HasMany");

            attribute.Arguments.Add(AttributeHelper.GetPrimitiveTypeAttributeArgument(Source.Name));
            if (TargetAccess != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", TargetAccess));
            if (TargetCache != CacheEnum.Undefined)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Cache", "CacheEnum", TargetCache));
            if (TargetCascade != ManyRelationCascadeEnum.None)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Cascade", "ManyRelationCascadeEnum", TargetCascade));
            if (!string.IsNullOrEmpty(TargetColumnKey))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("ColumnKey", TargetColumnKey));

            if (!string.IsNullOrEmpty(TargetCustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", TargetCustomAccess));
            else if (Target.Model.AutomaticAssociations)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", context.Namespace + "." + context.InternalPropertyAccessorName + ", " + context.AssemblyName));

            if (EffectiveRelationType == RelationType.Map)
            {
                // TODO: Index & IndexType ?
            }
            if (TargetInverse)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Inverse", TargetInverse));
            if (TargetLazy)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Lazy", TargetLazy));
            if (TargetFetch != FetchEnum.Unspecified)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Fetch", "FetchEnum", TargetFetch));
            if (!string.IsNullOrEmpty(TargetMapType))
                attribute.Arguments.Add(AttributeHelper.GetNamedTypeAttributeArgument("MapType", TargetMapType));
            if (!string.IsNullOrEmpty(TargetOrderBy))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("OrderBy", TargetOrderBy));
            if (EffectiveRelationType != RelationType.Guess)
                attribute.Arguments.Add(
                    AttributeHelper.GetNamedEnumAttributeArgument("RelationType", "RelationType", EffectiveRelationType));
            if (!string.IsNullOrEmpty(TargetSchema))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Schema", TargetSchema));
            if (EffectiveRelationType == RelationType.Set && !string.IsNullOrEmpty(TargetSort))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Sort", TargetSort));
            if (!string.IsNullOrEmpty(TargetTable))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Table", TargetTable));
            if (!string.IsNullOrEmpty(TargetWhere))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Where", TargetWhere));
            if (TargetNotFoundBehaviour != NotFoundBehaviour.Default)
                attribute.Arguments.Add(
                    AttributeHelper.GetNamedEnumAttributeArgument("NotFoundBehaviour", "NotFoundBehaviour",
                                                  TargetNotFoundBehaviour));
            if (!string.IsNullOrEmpty(TargetIndexType))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("IndexType", TargetIndexType));
            if (!string.IsNullOrEmpty(TargetIndex))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Index", TargetIndex));
            if (!string.IsNullOrEmpty(TargetElement))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Element", TargetElement));

            AddOptionalCollectionType(attribute,
                string.IsNullOrEmpty(TargetIUserCollectionType) ? Target.Model.ManyToOneIUserCollectionType : TargetIUserCollectionType,
                Target.AreRelationsGeneric() ? Source.Name : null);

            return attribute;
        }

        public CodeAttributeDeclaration GetBelongsToAttribute()
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("BelongsTo");
            if (!string.IsNullOrEmpty(EffectiveSourceColumn) && EffectiveSourceColumn != EffectiveSourcePropertyName)
                attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(EffectiveSourceColumn));
            if (SourceCascade != CascadeEnum.None)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Cascade", "CascadeEnum", SourceCascade));

            if (!string.IsNullOrEmpty(SourceCustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", SourceCustomAccess));
            else if (Source.Model.AutomaticAssociations)
#warning FIXME - This should be returning the correct property accessor for the field case the user has selected.
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", PropertyAccess.FieldCamelcaseUnderscore));

            if (!SourceInsert)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Insert", SourceInsert));
            if (SourceNotNull)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("NotNull", SourceNotNull));
            if (SourceOuterJoin != OuterJoinEnum.Auto)
                attribute.Arguments.Add(
                    AttributeHelper.GetNamedEnumAttributeArgument("OuterJoin", "OuterJoinEnum", SourceOuterJoin));
            if (!string.IsNullOrEmpty(SourceType))
                attribute.Arguments.Add(AttributeHelper.GetNamedTypeAttributeArgument("Type", SourceType));
            if (SourceUnique)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Unique", SourceUnique));
            if (!SourceUpdate)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Update", SourceUpdate));
            if (SourceNotFoundBehaviour != NotFoundBehaviour.Default)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("NotFoundBehaviour", "NotFoundBehaviour", SourceNotFoundBehaviour));

            return attribute;
        }

        /// <summary>
        /// Constructs a CollectionType named attribute.
        /// </summary>
        /// <param name="attribute">The attribute to add the CollectionType argument to.</param>
        /// <param name="collectionType">If null, nothing is added to the attribute.</param>
        /// <param name="genericTypeParameter">If non-null, this is used as the generic parameter to the collection type.</param>
        public static void AddOptionalCollectionType(CodeAttributeDeclaration attribute, string collectionType, string genericTypeParameter)
        {
            if (!string.IsNullOrEmpty(collectionType))
            {
                if (!string.IsNullOrEmpty(genericTypeParameter))
                    attribute.Arguments.Add(AttributeHelper.GetNamedGenericTypeAttributeArgument("CollectionType", collectionType, genericTypeParameter));
                else
                    attribute.Arguments.Add(AttributeHelper.GetNamedTypeAttributeArgument("CollectionType", collectionType));
            }
        }
    }
}
