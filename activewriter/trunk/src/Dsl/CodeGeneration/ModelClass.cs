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
using System.CodeDom;
using Altinoren.ActiveWriter.CodeGeneration;

namespace Altinoren.ActiveWriter
{
    public partial class ModelClass
    {
        #region Public Code Generation Methods
        
        public bool IsGeneric()
        {
            return
                (Model.UseGenerics && UseGenerics == InheritableBoolean.Inherit) ||
                UseGenerics == InheritableBoolean.True;
        }

        public bool AreRelationsGeneric()
        {
            return
                (Model.UseGenericRelations && UseGenericRelations == InheritableBoolean.Inherit) ||
                UseGenericRelations == InheritableBoolean.True;
        }

        public bool DoesImplementINotifyPropertyChanged()
        {
            return (Model.ImplementINotifyPropertyChanged && ImplementINotifyPropertyChanged == InheritableBoolean.Inherit) ||
                ImplementINotifyPropertyChanged == InheritableBoolean.True;
        }

        public bool DoesImplementINotifyPropertyChanging()
        {
            return (Model.ImplementINotifyPropertyChanging && ImplementINotifyPropertyChanging == InheritableBoolean.Inherit) ||
                ImplementINotifyPropertyChanging == InheritableBoolean.True;
        }

        public bool HasPropertyWithValidators()
        {
            return this.Properties.Find(
                delegate (ModelProperty property)
                    {
                        return property.IsValidatorSet();
                    }
                ) != null;
        }

        public CodeAttributeDeclaration GetActiveRecordAttribute()
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ActiveRecord");

            if (!string.IsNullOrEmpty(Table))
                attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(Table));
            if (Cache != CacheEnum.Undefined)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Cache", "CacheEnum", Cache));
            if (!string.IsNullOrEmpty(CustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", CustomAccess));
            if (!string.IsNullOrEmpty(DiscriminatorColumn))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("DiscriminatorColumn", DiscriminatorColumn));
            if (!string.IsNullOrEmpty(DiscriminatorType))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("DiscriminatorType", DiscriminatorType));
            if (!string.IsNullOrEmpty(DiscriminatorValue))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("DiscriminatorValue", DiscriminatorValue));
            if (Lazy)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Lazy", Lazy));
            if (!string.IsNullOrEmpty(Proxy))
                attribute.Arguments.Add(AttributeHelper.GetNamedTypeAttributeArgument("Proxy", Proxy));
            if (!string.IsNullOrEmpty(Schema))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Schema", Schema));
            if (!string.IsNullOrEmpty(Where))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Where", Where));
            if (DynamicInsert)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("DynamicInsert", DynamicInsert));
            if (DynamicUpdate)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("DynamicUpdate", DynamicUpdate));
            if (!string.IsNullOrEmpty(Persister))
                attribute.Arguments.Add(AttributeHelper.GetNamedTypeAttributeArgument("Persister", Persister));
            if (SelectBeforeUpdate)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("SelectBeforeUpdate", SelectBeforeUpdate));
            if (Polymorphism != Polymorphism.Implicit)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Polymorphism", "Polymorphism", Polymorphism));
            if (!Mutable)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Mutable", Mutable));
            if (BatchSize != 1)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("BatchSize", BatchSize));
            if (Locking != OptimisticLocking.Version)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Locking", "OptimisticLocking", Locking));
            if (!UseAutoImport)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("UseAutoImport", UseAutoImport));

            return attribute;
        }

        #endregion

    }
}
