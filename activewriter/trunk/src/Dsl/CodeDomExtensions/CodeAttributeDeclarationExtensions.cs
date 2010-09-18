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

	public static class CodeAttributeDeclarationExtensions
    {
        public static CodeAttributeDeclaration Clone(this CodeAttributeDeclaration attribute)
        {
            if (attribute == null) return null;
            CodeAttributeDeclaration a = new CodeAttributeDeclaration(attribute.AttributeType.Clone());
            a.Arguments.AddRange(attribute.Arguments.Clone());
            a.Name = attribute.Name;
            return a;
        }

        public static void ReplaceType(this CodeAttributeDeclaration attribute, string oldType, string newType)
        {
            if (attribute == null) return;
            attribute.Arguments.ReplaceType(oldType, newType);
            attribute.AttributeType.ReplaceType(oldType, newType);
        }
    }
}
