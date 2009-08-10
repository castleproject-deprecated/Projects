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

    public partial class ManyToManyRelation
    {
        public RelationType EffectiveSourceRelationType
        {
            get
            {
                return (SourceRelationType == InheritedRelationType.Inherited
                            ? Source.Model.ManyToManyRelationType
                            : SourceRelationType.GetMatchingRelationType());
            }
        }

        public RelationType EffectiveTargetRelationType
        {
            get
            {
                return (TargetRelationType == InheritedRelationType.Inherited
                            ? Source.Model.ManyToManyRelationType
                            : TargetRelationType.GetMatchingRelationType());
            }
        }

        public string EffectiveSourcePropertyName
        {
            get
            {
                // Don't pluralize custom SourcePropertyNames to allow for override of automatic pluralization.
                // If the user types it, they should know what they want.
                return string.IsNullOrEmpty(SourcePropertyName)
                           ? NamingHelper.GetPlural(Target.Name)
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

        public string EffectiveCollectionIDColumn
        {
            get
            {
                if (!string.IsNullOrEmpty(CollectionIDColumn))
                    return CollectionIDColumn;

                if (!string.IsNullOrEmpty(Source.Model.ManyToManyCollectionIDColumnFormat))
                    return string.Format(Source.Model.ManyToManyCollectionIDColumnFormat, EffectiveTable);

                if (!string.IsNullOrEmpty(Source.Model.CommonPrimaryKeyColumnFormat))
                    return string.Format(Source.Model.CommonPrimaryKeyColumnFormat, EffectiveTable);

                // We don't use a class name for the format since there is no class for many to many relationships.
                if (!string.IsNullOrEmpty(Source.Model.CommonPrimaryKeyPropertyFormat))
                    return string.Format(Source.Model.CommonPrimaryKeyPropertyFormat, EffectiveTable);

                return null;
            }
        }

        public NHibernateType EffectiveCollectionIDColumnType
        {
            get
            {
                if (!string.IsNullOrEmpty(CollectionIDColumn))
                    return CollectionIDColumnType;

                if (!string.IsNullOrEmpty(Source.Model.ManyToManyCollectionIDColumnFormat))
                    return Source.Model.ManyToManyCollectionIDColumnType;

                if (!string.IsNullOrEmpty(Source.Model.CommonPrimaryKeyColumnFormat))
                    return Source.Model.CommonPrimaryKeyColumnType;

                if (!string.IsNullOrEmpty(Source.Model.CommonPrimaryKeyPropertyFormat))
                    return Source.Model.CommonPrimaryKeyColumnType;

                return CollectionIDColumnType;
            }
        }

        public PrimaryKeyType EffectiveCollectionIDGenerator
        {
            get
            {
                if (!string.IsNullOrEmpty(CollectionIDColumn))
                    return CollectionIDGenerator;

                if (!string.IsNullOrEmpty(Source.Model.ManyToManyCollectionIDColumnFormat))
                    return Source.Model.ManyToManyCollectionIDGenerator;

                if (!string.IsNullOrEmpty(Source.Model.CommonPrimaryKeyColumnFormat))
                    return Source.Model.CommonPrimaryKeyGenerator;

                if (!string.IsNullOrEmpty(Source.Model.CommonPrimaryKeyPropertyFormat))
                    return Source.Model.CommonPrimaryKeyGenerator;

                return CollectionIDGenerator;
            }
        }

        public string EffectiveTargetColumn
        {
            get
            {
                if (string.IsNullOrEmpty(TargetColumn) && !string.IsNullOrEmpty(Target.Model.ForeignKeyFormat))
                    return string.Format(Target.Model.ForeignKeyFormat, Target.Name);
                return TargetColumn;
            }
        }

        public string EffectiveSourceColumn
        {
            get
            {
                if (string.IsNullOrEmpty(SourceColumn) && !string.IsNullOrEmpty(Source.Model.ForeignKeyFormat))
                    return string.Format(Source.Model.ForeignKeyFormat, Source.Name);
                else
                    return SourceColumn;
            }
        }

        public string EffectiveTable
        {
            get
            {
                if (string.IsNullOrEmpty(Table) && !string.IsNullOrEmpty(Source.Model.ManyToManyTableFormat))
                    return string.Format(Source.Model.ManyToManyTableFormat, Source.Name, Target.Name);
                else
                    return Table;
            }
        }

        public bool EffectiveAutomaticAssociations
        {
            get
            {
                return Source.Model.AutomaticAssociations
                       && SourcePropertyGenerated
                       && TargetPropertyGenerated;
            }
        }

        public CodeAttributeDeclaration GetHasAndBelongsToAttributeFromSource(CodeGenerationContext context)
        {
            var attribute = new CodeAttributeDeclaration("HasAndBelongsToMany");

            attribute.Arguments.Add(AttributeHelper.GetPrimitiveTypeAttributeArgument(Target.Name));
            if (SourceAccess != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess",
                                                                                      SourceAccess));
            if (SourceCache != CacheEnum.Undefined)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Cache", "CacheEnum", SourceCache));
            if (SourceCascade != ManyRelationCascadeEnum.None)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Cascade",
                                                                                      "ManyRelationCascadeEnum",
                                                                                      SourceCascade));

            if (!string.IsNullOrEmpty(SourceCustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", SourceCustomAccess));
            else if (EffectiveAutomaticAssociations)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", context.Namespace + "." + context.InternalPropertyAccessorName + ", " + context.AssemblyName));

            attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("ColumnRef", EffectiveTargetColumn));
            attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("ColumnKey", EffectiveSourceColumn));
            if (EffectiveSourceRelationType == RelationType.Map)
            {
                // TODO: Index & IndexType
            }
            if (SourceInverse)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Inverse", SourceInverse));
            if (SourceLazy)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Lazy", SourceLazy));
            if (!string.IsNullOrEmpty(SourceMapType))
                attribute.Arguments.Add(AttributeHelper.GetNamedTypeAttributeArgument("MapType", SourceMapType));
            if (!string.IsNullOrEmpty(SourceOrderBy))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("OrderBy", SourceOrderBy));
            if (EffectiveSourceRelationType != RelationType.Guess)
                attribute.Arguments.Add(
                    AttributeHelper.GetNamedEnumAttributeArgument("RelationType", "RelationType", EffectiveSourceRelationType));
            if (EffectiveSourceRelationType == RelationType.Set && !string.IsNullOrEmpty(SourceSort))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Sort", SourceSort));
            if (!string.IsNullOrEmpty(SourceWhere))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Where", SourceWhere));
            if (SourceNotFoundBehaviour != NotFoundBehaviour.Default)
                attribute.Arguments.Add(
                    AttributeHelper.GetNamedEnumAttributeArgument("NotFoundBehaviour", "NotFoundBehaviour",
                                                                  SourceNotFoundBehaviour));

            ManyToOneRelation.AddOptionalCollectionType(attribute,
                string.IsNullOrEmpty(SourceIUserCollectionType) ? Source.Model.ManyToManyIUserCollectionType : SourceIUserCollectionType,
                Source.AreRelationsGeneric() ? Target.Name : null);

            PopulateCommonFields(attribute);

            return attribute;
        }

        public CodeAttributeDeclaration GetHasAndBelongsToAttributeFromTarget(CodeGenerationContext context)
        {
            var attribute = new CodeAttributeDeclaration("HasAndBelongsToMany");

            attribute.Arguments.Add(AttributeHelper.GetPrimitiveTypeAttributeArgument(Source.Name));
            if (TargetAccess != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess",
                                                                                      TargetAccess));
            if (TargetCache != CacheEnum.Undefined)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Cache", "CacheEnum", TargetCache));
            if (TargetCascade != ManyRelationCascadeEnum.None)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Cascade",
                                                                                      "ManyRelationCascadeEnum",
                                                                                      TargetCascade));

            if (!string.IsNullOrEmpty(TargetCustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", TargetCustomAccess));
            else if (EffectiveAutomaticAssociations)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", context.Namespace + "." + context.InternalPropertyAccessorName + ", " + context.AssemblyName));

            attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("ColumnRef", EffectiveSourceColumn));
            attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("ColumnKey", EffectiveTargetColumn));
            if (EffectiveTargetRelationType == RelationType.Map)
            {
                // TODO: Index & IndexType
            }
            if (TargetInverse)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Inverse", TargetInverse));
            if (TargetLazy)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Lazy", TargetLazy));
            if (!string.IsNullOrEmpty(TargetMapType))
                attribute.Arguments.Add(AttributeHelper.GetNamedTypeAttributeArgument("MapType", TargetMapType));
            if (!string.IsNullOrEmpty(TargetOrderBy))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("OrderBy", TargetOrderBy));
            if (EffectiveTargetRelationType != RelationType.Guess)
                attribute.Arguments.Add(
                    AttributeHelper.GetNamedEnumAttributeArgument("RelationType", "RelationType", EffectiveTargetRelationType));
            if (EffectiveTargetRelationType == RelationType.Set && !string.IsNullOrEmpty(TargetSort))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Sort", TargetSort));
            if (!string.IsNullOrEmpty(TargetWhere))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Where", TargetWhere));
            if (TargetNotFoundBehaviour != NotFoundBehaviour.Default)
                attribute.Arguments.Add(
                    AttributeHelper.GetNamedEnumAttributeArgument("NotFoundBehaviour", "NotFoundBehaviour",
                                                                  TargetNotFoundBehaviour));

            ManyToOneRelation.AddOptionalCollectionType(attribute,
                string.IsNullOrEmpty(TargetIUserCollectionType) ? Target.Model.ManyToManyIUserCollectionType : TargetIUserCollectionType,
                Target.AreRelationsGeneric() ? Source.Name : null);

            PopulateCommonFields(attribute);

            return attribute;
        }

        public CodeAttributeDeclaration GetCollectionIdAttribute(RelationType relationType)
        {
            if (string.IsNullOrEmpty(EffectiveCollectionIDColumn) || relationType != RelationType.IdBag)
                return null;

            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("CollectionID");

            attribute.Arguments.Add(AttributeHelper.GetPrimitiveEnumAttributeArgument("CollectionIDType", EffectiveCollectionIDGenerator));
            attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(EffectiveCollectionIDColumn));
            attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(EffectiveCollectionIDColumnType.ToString()));

            return attribute;
        }

        private void PopulateCommonFields(CodeAttributeDeclaration attribute)
        {
            if (!string.IsNullOrEmpty(Schema))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Schema", Schema));
            if (!string.IsNullOrEmpty(EffectiveTable))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Table", EffectiveTable));
        }
    }
}
