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
    using System.Collections.Generic;

    public partial class Model
    {
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
                if (this.UseNullables == NullableUsage.WithHelperLibrary)
                    imports.Add(new CodeNamespaceImport(Common.NullablesNamespace));
                if (HasClassImplementsINotifyPropertyChanged())
                    imports.Add(new CodeNamespaceImport(Common.ComponentmodelNamespace));
                if (this.AdditionalImports != null && this.AdditionalImports.Count > 0)
                {
                    foreach (Import item in this.AdditionalImports)
                    {
                        if (!string.IsNullOrEmpty(item.Name))
                            imports.Add(new CodeNamespaceImport(item.Name));
                    }
                }

                return imports;
            }
        }

        #endregion

        #region Private Methods

        private bool HasClassImplementsINotifyPropertyChanged()
        {
            bool hasModelClass = this.Classes.Find(
                                     delegate(ModelClass cls) { return cls.DoesImplementINotifyPropertyChanged(); }
                                     ) != null;
            bool hasNestedClass = this.NestedClasses.Find(
                                      delegate(NestedClass cls) { return cls.DoesImplementINotifyPropertyChanged(); }
                                      ) != null;

            return hasModelClass || hasNestedClass;
        }

        #endregion   
    }
}
