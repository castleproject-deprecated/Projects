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

namespace Castle.ActiveWriter.CodeDomExtensions
{
	using System.CodeDom;

	public static class CodeParameterDeclarationExpressionCollectionExtensions
    {
        public static CodeParameterDeclarationExpressionCollection Clone(this CodeParameterDeclarationExpressionCollection collection)
        {
            if (collection == null) return null;
            CodeParameterDeclarationExpressionCollection c = new CodeParameterDeclarationExpressionCollection();
            foreach (CodeParameterDeclarationExpression expression in collection)
                c.Add(expression.Clone());
            return c;
        }

        public static void ReplaceType(this CodeParameterDeclarationExpressionCollection collection, string oldType, string newType)
        {
            if (collection == null) return;
            foreach (CodeParameterDeclarationExpression parameter in collection)
                parameter.ReplaceType(oldType, newType);
        }

        public static bool ContainsType(this CodeParameterDeclarationExpressionCollection collection, string type)
        {
            foreach (CodeParameterDeclarationExpression param in collection)
                if (param.ContainsType(type))
                    return true;
            return false;
        }
    }
}
