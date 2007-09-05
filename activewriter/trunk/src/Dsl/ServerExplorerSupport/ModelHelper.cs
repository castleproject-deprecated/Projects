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

using System.Text;
namespace Altinoren.ActiveWriter.ServerExplorerSupport
{
    internal static class ModelHelper
    {

        private static bool useCameCase = true;

        public static string GetSafeName(string name)
        {
            if (useCameCase)
            {
                return CamelCase(name).Replace(" ", string.Empty);
            }
            else
            {
                return name.Replace(" ", string.Empty);
            }
        }

        public static string CamelCase(string original)
        {
            if (original == "" || original == null) return original;
            StringBuilder sb = new StringBuilder();
            string[] parts = original.Split('_');
            foreach (string p in parts)
            {
                sb.Append(p.Substring(0, 1).ToUpper());
                sb.Append(p.Substring(1).ToLower());
            }
            return sb.ToString();
        }
    }
}
