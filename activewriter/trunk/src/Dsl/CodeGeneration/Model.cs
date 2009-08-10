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

namespace Altinoren.ActiveWriter
{
    using System.CodeDom;
    using System.Collections.Generic;

    public partial class Model
    {
        private const string GenericListInterface = "IList";
        private const string GenericListClass = "List";

        public string ModelFileFullName { get; set; } // Set by DocData when the model is loaded.

        #region Public Code Generation Methods

        public List<CodeNamespaceImport> NamespaceImports
        {
            get
            {
                List<CodeNamespaceImport> imports = new List<CodeNamespaceImport>();
                imports.Add(new CodeNamespaceImport(Common.SystemNamespace));
                imports.Add(new CodeNamespaceImport(Common.GenericCollectionsNamespace));
                imports.Add(new CodeNamespaceImport(Common.CollectionsNamespace));
                imports.Add(new CodeNamespaceImport(Common.ActiveRecordNamespace));
                if (HasClassWithValidators())
                    imports.Add(new CodeNamespaceImport(Common.ValidatorNamespace));
                if (UseNullables == NullableUsage.WithHelperLibrary)
                    imports.Add(new CodeNamespaceImport(Common.NullablesNamespace));
                if (HasClassImplementingINotifyPropertyChangedOrChanging())
                    imports.Add(new CodeNamespaceImport(Common.ComponentmodelNamespace));
                if (AdditionalImports != null && AdditionalImports.Count > 0)
                {
                    foreach (Import item in AdditionalImports)
                    {
                        if (!string.IsNullOrEmpty(item.Name))
                        {
                            if (!string.IsNullOrEmpty(item.Replaces))
                            {
                                CodeNamespaceImport import = imports.Find(i => i.Namespace == item.Replaces);
                                if (import != null)
                                    imports.Remove(import);
                            }
                            imports.Add(new CodeNamespaceImport(item.Name));
                        }
                    }
                }

                return imports;
            }
        }

        public string EffectiveListInterface
        {
            get
            {
                return string.IsNullOrEmpty(CollectionInterface)
                    ? GenericListInterface
                    : CollectionInterface;
            }
        }

        public string EffectiveListClass
        {
            get
            {
                return string.IsNullOrEmpty(CollectionImplementation)
                    ? GenericListClass
                    : CollectionImplementation;
            }
        }

        public PropertyAccess FieldPropertyAccess
        {
            get
            {
                switch (CaseOfPrivateFields)
                {
                    case FieldCase.Camelcase:
                        return PropertyAccess.FieldCamelcase;
                    case FieldCase.CamelcaseUnderscore:
                        return PropertyAccess.FieldCamelcaseUnderscore;
                    case FieldCase.PascalcaseMUnderscore:
                        return PropertyAccess.FieldPascalcaseMUnderscore;
                    default:
                        throw new NotImplementedException("Cannot convert field case to property access: " + CaseOfPrivateFields);
                }
            }
        }

        public bool PropertyChangedDefinedInBaseClass
        {
            get
            {
                return !string.IsNullOrEmpty(BaseClassPropertyChangedMethod);
            }
        }

        public string PropertyChangedMethodName
        {
            get
            {
                if (!string.IsNullOrEmpty(BaseClassPropertyChangedMethod))
                    return BaseClassPropertyChangedMethod;

                return Common.PropertyChangedInternalMethod;
            }
        }

        public bool PropertyChangingDefinedInBaseClass
        {
            get
            {
                return !string.IsNullOrEmpty(BaseClassPropertyChangingMethod);
            }
        }

        public string PropertyChangingMethodName
        {
            get
            {
                if (!string.IsNullOrEmpty(BaseClassPropertyChangingMethod))
                    return BaseClassPropertyChangingMethod;

                return Common.PropertyChangingInternalMethod;
            }
        }

        #endregion

        #region Private Methods

        private bool HasClassWithValidators()
        {
            bool hasClass = this.Classes.Find(cls => cls.HasPropertyWithValidators()) != null;
            bool hasNestedClass = this.NestedClasses.Find(cls => cls.HasPropertyWithValidators()) != null;

            return hasClass || hasNestedClass;
        }

        private bool HasClassImplementingINotifyPropertyChangedOrChanging()
        {
            bool hasModelClass = Classes.Find(c => c.DoesImplementINotifyPropertyChanged() || c.DoesImplementINotifyPropertyChanging()) != null;
            bool hasNestedClass = NestedClasses.Find(n => n.DoesImplementINotifyPropertyChanged() || n.DoesImplementINotifyPropertyChanging()) != null;

            return hasModelClass || hasNestedClass;
        }

        #endregion   
    }
}
