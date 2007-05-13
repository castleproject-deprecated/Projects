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
    using System.Text.RegularExpressions;

    public static class Common
    {
        // TODO: Tidy this class.
        private static string[] umToA = new string[] { "agendum", "datum", "extremum", "bacterium", "erratum" };
        
        public static string[] ARAttributes = new string[] {"ActiveRecord", "Property", "Field", "PrimaryKey", "CompositeKey", "HasMany", "BelongsTo", "HasAndBelongsToMany"};
        public static string CompositeClassNameSuffix = "CompositeKey";
        public static string ActiveRecordNamespace = "Castle.ActiveRecord";
        public static string CollectionsNamespace = "System.Collections";
        public static string GenericCollectionsNamespace = "System.Collections.Generic";
        public static string SystemNamespace = "System";
        public static string NullablesNamespace = "Nullables";
        public static string ComponentmodelNamespace = "System.ComponentModel";
        public static string INotifyPropertyChangedType = "INotifyPropertyChanged";
        public static string DefaultBaseClass = "ActiveRecordBase";
        public static string DefaultValidationBaseClass = "ActiveRecordValidationBase";
        public static string XorHelperMethod = "XorHelper";
        public static string PropertyChangedInternalMethod = "NotifyPropertyChanged";
        public static string BlankProjectTemplateName = "EmptyProject.zip";
        public static string DTEProjectTemplateLanguageCSharp = "CSharp";
        public static string DTEProjectTemplateLanguageVB = "VisualBasic";
        public static string ModelsFolderName = "Models";
        public static string ViewsFolderName = "Views";
        public static string ControllersFolderName = "Controllers";
        public static string InMemoryCompiledAssemblyName = "AW";
        public static int EmbeddedResourceBuildActionIndex = 3;

        public static string MakeCamel(string value)
        {
            if (value.Length > 1)
                return value.Substring(0, 1).ToLowerInvariant() + value.Substring(1, value.Length - 1);
            else
                return value.ToLowerInvariant();
        }

        public static string MakePascal(string value)
        {
            if (value.Length > 1)
                return value.Substring(0, 1).ToUpperInvariant() + value.Substring(1, value.Length - 1);
            else
                return value.ToUpperInvariant();
        }

        public static string GetPlural(string name)
        {
            // Algorithm partially taken from Damian Conway's paper: An Algorithmic Approach to English Pluralization
            // http://www.csse.monash.edu.au/~damian/papers/HTML/Plurals.html,
            // We assume all are nouns. Order below is important in some cases. See the paper.

            string lowercaseName = name.ToLowerInvariant();

            if (lowercaseName.Equals("data"))
                return name;
            if (lowercaseName.Equals("child"))
                return name + "ren";
            if (lowercaseName.Equals("criterion"))
                return name.Substring(0, name.Length - 2) + "a";

            if (IsMatch(name, @"^\w*man$")) return ReplaceAtTheEnd(name, 3, "men");
            if (IsMatch(name, @"^\w*[lm]ouse$")) return ReplaceAtTheEnd(name, 4, "ice");
            if (IsMatch(name, @"^\w*tooth$")) return ReplaceAtTheEnd(name, 5, "teeth");
            if (IsMatch(name, @"^\w*goose$")) return ReplaceAtTheEnd(name, 5, "geese");
            if (IsMatch(name, @"^\w*foot$")) return ReplaceAtTheEnd(name, 4, "foot");
            if (IsMatch(name, @"^\w*[csx]is$")) return ReplaceAtTheEnd(name, 2, "es");

            foreach (string key in umToA)
            {
                if (lowercaseName == key)
                    return name.Substring(0, name.Length - 2) + "a";
            }

            if (IsMatch(name, @"^\w*trix$")) return ReplaceAtTheEnd(name, 4, "trices");
            if (IsMatch(name, @"^\w*eau$")) return ReplaceAtTheEnd(name, 3, "eaux");
            if (IsMatch(name, @"^\w*ieu$")) return ReplaceAtTheEnd(name, 3, "ieux");
            if (IsMatch(name, @"^\w*[iay]nx$")) return ReplaceAtTheEnd(name, 2, "nges");
            if (IsMatch(name, @"^\w*[cs]h$")) return ReplaceAtTheEnd(name, 1, "hes");
            if (IsMatch(name, @"^\w*ss$")) return ReplaceAtTheEnd(name, 2, "sses");
            if (IsMatch(name, @"^\w*[aeo]lf|[^d]eaf|arf$")) return ReplaceAtTheEnd(name, 1, "ves");
            if (IsMatch(name, @"^\w*[nlw]ife$")) return ReplaceAtTheEnd(name, 1, "ves");
            if (IsMatch(name, @"^\w*[aeiou]y$")) return ReplaceAtTheEnd(name, 1, "ys");
            if (IsMatch(name, @"^\w*y$")) return ReplaceAtTheEnd(name, 1, "ies");
            if (IsMatch(name, @"^\w*[aeiou]o$")) return ReplaceAtTheEnd(name, 1, "os");
            if (IsMatch(name, @"^\w*o$")) return ReplaceAtTheEnd(name, 1, "oes");
            if (IsMatch(name, @"^\w*[aeiou]x$")) return ReplaceAtTheEnd(name, 1, "xes");

            if (lowercaseName.EndsWith("s"))
                return name;

            return name + "s";
        }

        public static bool IsMatch(string name, string exp)
        {
            Regex expression = new Regex(exp, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            return expression.Matches(name).Count > 0;
        }

        public static string ReplaceAtTheEnd(string name, int numberOfCharacter, string replacement)
        {
            return name.Substring(0, name.Length - numberOfCharacter) + replacement;
        }
    }
}