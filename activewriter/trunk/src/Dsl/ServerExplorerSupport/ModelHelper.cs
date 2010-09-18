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

namespace Castle.ActiveWriter.ServerExplorerSupport
{
	using System.Text;
	using System.Text.RegularExpressions;

	internal static class ModelHelper
    {
        private static bool usePascalCase = true;

        public static string GetSafeName(string name, string propertyNameFilterExpression)
        {
            string s;

            // optionally remove prefix from property name
            if (string.IsNullOrEmpty(propertyNameFilterExpression))
            {
                s = name;
            }
            else
            {
                s = Regex.Replace(name, propertyNameFilterExpression, string.Empty);
            }

            // Then format name
            if (usePascalCase)
            {
                s = PascalCase(s).Replace(" ", string.Empty);
            }
            else
            {
                s = s.Replace(" ", string.Empty);
            }


            return s;
        }

        public static string PascalCase(string original)
        {
            if (string.IsNullOrEmpty(original))
                return original;

            if (original.Contains("_"))
            {
                StringBuilder stringBuilder = new StringBuilder();
                string[] parts = original.Split('_');
                foreach (string part in parts)
                {
                    stringBuilder.Append(part.Substring(0, 1).ToUpperInvariant());
                    stringBuilder.Append(part.Substring(1).ToLowerInvariant());
                }
                return stringBuilder.ToString();
            }
            if (original.Length == 1)
            {
                return original.ToUpperInvariant();
            }
            // Pascal casing of simple strings (without "_"s) should make no assumptions
            // on the rest of the string other than the first character. 
            return original.Substring(0, 1).ToUpperInvariant() + original.Substring(1);
        }
    }
}
