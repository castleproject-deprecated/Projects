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

	public static class CodeTypeDeclarationExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeDeclaration"></param>
        /// <returns>Null if no constructor is found.</returns>
        public static CodeConstructor FindEmptyConstructor(this CodeTypeDeclaration typeDeclaration)
        {
            foreach (CodeTypeMember typeMember in typeDeclaration.Members)
                if (typeMember is CodeConstructor && ((CodeConstructor)typeMember).Parameters.Count == 0)
                    return (CodeConstructor)typeMember;
            return null;
        }

        public static void CreateEmptyPublicConstructor(this CodeTypeDeclaration typeDeclaration)
        {
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            typeDeclaration.Members.Add(constructor);
        }
    }
}
