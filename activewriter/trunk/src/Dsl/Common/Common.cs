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
    public static class Common
    {
        // TODO: Tidy this class.
        public static string[] ARAttributes = new[]
                                                  {
                                                      "ActiveRecord",
                                                      "Property",
                                                      "Field",
                                                      "PrimaryKey",
                                                      "CompositeKey",
                                                      "HasMany",
                                                      "BelongsTo",
                                                      "HasAndBelongsToMany",
                                                      "OneToOne",
                                                      "KeyProperty",
                                                      "Nested"
                                                  };
        
        public static string CompositeClassNameSuffix = "CompositeKey";
        public static string ActiveRecordNamespace = "Castle.ActiveRecord";
        public static string ValidatorNamespace = "Castle.Components.Validator";
        public static string CollectionsNamespace = "System.Collections";
        public static string GenericCollectionsNamespace = "System.Collections.Generic";
        public static string SystemNamespace = "System";
        public static string NullablesNamespace = "Nullables";
        public static string ComponentmodelNamespace = "System.ComponentModel";
        public static string INotifyPropertyChangedType = "INotifyPropertyChanged";
        public static string INotifyPropertyChangingType = "INotifyPropertyChanging";
        public static string DefaultBaseClass = "ActiveRecordBase";
        public static string DefaultValidationBaseClass = "ActiveRecordValidationBase";
        public static string XorHelperMethod = "XorHelper";
        public static string PropertyChangedInternalMethod = "NotifyPropertyChanged";
        public static string PropertyChangingInternalMethod = "NotifyPropertyChanging";
        public static string BlankProjectTemplateName = "EmptyProject.zip";
        public static string DTEProjectTemplateLanguageCSharp = "CSharp";
        public static string DTEProjectTemplateLanguageVB = "VisualBasic";
        public static string ModelsFolderName = "Models";
        public static string ViewsFolderName = "Views";
        public static string ControllersFolderName = "Controllers";
        public static string InMemoryCompiledAssemblyName = "AW";
        public static string ActiveRecordVersion = "1.0.0.0";
        public static string vsProgIdBase = "!VisualStudio.DTE.9.0:";
    }
}
