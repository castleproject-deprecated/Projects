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

	public static class CodeTypeReferenceExtensions
    {
        public static CodeTypeReference Clone(this CodeTypeReference reference)
        {
            if (reference == null) return null;
            CodeTypeReference r = new CodeTypeReference();
            r.ArrayElementType = reference.ArrayElementType.Clone();
            r.ArrayRank = reference.ArrayRank;
            r.BaseType = reference.BaseType;
            r.Options = reference.Options;
            r.TypeArguments.AddRange(reference.TypeArguments.Clone());
            r.UserData.AddRange(reference.UserData);
            return r;
        }

        public static void ReplaceType(this CodeTypeReference reference, string oldType, string newType)
        {
            if (reference == null) return;

            // Replace fundamental type names.
            if (reference.BaseType == oldType)
                reference.BaseType = newType;

            // Replace nested references.
            reference.ArrayElementType.ReplaceType(oldType, newType);
            reference.TypeArguments.ReplaceType(oldType, newType);
        }

        public static bool ContainsType(this CodeTypeReference reference, string type)
        {
            if (reference == null) return false;

            if (reference.BaseType == type)
                return true;

            if (reference.ArrayElementType.ContainsType(type))
                return true;

            if (reference.TypeArguments.ContainsType(type))
                return true;

            return false;
        }
    }
}
