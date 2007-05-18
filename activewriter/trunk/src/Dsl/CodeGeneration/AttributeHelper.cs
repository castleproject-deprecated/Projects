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

namespace Altinoren.ActiveWriter.CodeGeneration
{
    using System;
    using System.CodeDom;

    public static class AttributeHelper
    {
        #region Public Methods

        public static CodeAttributeArgument GetStringArrayAttributeArgument(string[] values)
        {
            CodeExpression[] initializers = new CodeExpression[values.Length];
            for (int i = 0; i < values.Length; i++)
                initializers[i] = new CodePrimitiveExpression(values[i]);

            return new CodeAttributeArgument(new CodeArrayCreateExpression(typeof(string), initializers));
        }

        public static CodeAttributeArgument GetPrimitiveAttributeArgument(string name, string value)
        {
            // TODO: Does this support VB.Net?
            return new CodeAttributeArgument(new CodeSnippetExpression(String.Format("\"{0} = {{1}}\"", name, value)));
        }

        public static CodeAttributeArgument GetPrimitiveAttributeArgument(object o)
        {
            return new CodeAttributeArgument(new CodePrimitiveExpression(o));
        }

        public static CodeAttributeArgument GetPrimitiveTypeAttributeArgument(string type)
        {
            return new CodeAttributeArgument(new CodeTypeOfExpression(type));
        }

        public static CodeAttributeArgument GetNamedAttributeArgument(string name, object value)
        {
            return new CodeAttributeArgument(name, new CodePrimitiveExpression(value));
        }

        public static CodeAttributeArgument GetNamedEnumAttributeArgument(string name, string type, Enum value)
        {
            return
                new CodeAttributeArgument(name,
                                          new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(type),
                                                                           value.ToString()));
        }

        public static CodeAttributeArgument GetPrimitiveEnumAttributeArgument(string type, Enum value)
        {
            return
                new CodeAttributeArgument(
                    new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(type), value.ToString()));
        }

        public static CodeAttributeArgument GetNamedTypeAttributeArgument(string name, string type)
        {
            return new CodeAttributeArgument(name, new CodeTypeOfExpression(type));
        }

        #endregion

    }
}
