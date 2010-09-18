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

	public static class CodeAttributeDeclarationCollectionExtensions
    {
        public static CodeAttributeDeclarationCollection Clone(this CodeAttributeDeclarationCollection collection)
        {
            if (collection == null) return null;
            CodeAttributeDeclarationCollection c = new CodeAttributeDeclarationCollection();
            foreach (CodeAttributeDeclaration attribute in collection)
                c.Add(attribute.Clone());
            return c;
        }

        public static void ReplaceType(this CodeAttributeDeclarationCollection collection, string oldType, string newType)
        {
            if (collection == null) return;
            foreach (CodeAttributeDeclaration attribute in collection)
                attribute.ReplaceType(oldType, newType);
        }
    }
}
