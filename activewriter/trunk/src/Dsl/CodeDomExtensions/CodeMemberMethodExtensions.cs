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

	public static class CodeMemberMethodExtensions
    {
        public static CodeMemberMethod Clone(this CodeMemberMethod method)
        {
            if (method == null) return null;

            CodeMemberMethod m = new CodeMemberMethod();
            m.Attributes = method.Attributes;
            m.Comments.AddRange(method.Comments);
            m.CustomAttributes = method.CustomAttributes.Clone();
            m.EndDirectives.AddRange(method.EndDirectives);
            m.ImplementationTypes.AddRange(method.ImplementationTypes.Clone());
            m.LinePragma = method.LinePragma;
            m.Name = method.Name;
            m.Parameters.AddRange(method.Parameters.Clone());
            m.PrivateImplementationType = method.PrivateImplementationType.Clone();
            m.ReturnType = method.ReturnType.Clone();
            m.ReturnTypeCustomAttributes.AddRange(method.ReturnTypeCustomAttributes.Clone());
            m.StartDirectives.AddRange(method.StartDirectives);
            m.Statements.AddRange(method.Statements.Clone());
            // TypeParameters needn't be cloned since we don't modify them.
            m.TypeParameters.AddRange(method.TypeParameters);
            m.UserData.AddRange(method.UserData);
            return m;
        }

        public static void ReplaceType(this CodeMemberMethod method, string oldType, string newType)
        {
            if (method == null) return;

            // Attributes apply before the method type parameters get processed.
            method.CustomAttributes.ReplaceType(oldType, newType);
            method.ReturnTypeCustomAttributes.ReplaceType(oldType, newType);

            // If oldType is in the TypeParameters, there is nothing to do since the type parameters override the outside generic parameters.
            foreach (CodeTypeParameter parameter in method.TypeParameters)
                if (parameter.Name == oldType)
                    return;

            // In my tests it seems that ImplementationTypes cannot refer to type parameters.
            //method.ImplementationTypes.ReplaceType(oldType, newType);
            //method.PrivateImplementationType.ReplaceType(oldType, newType);

            method.Parameters.ReplaceType(oldType, newType);
            method.ReturnType.ReplaceType(oldType, newType);
            method.Statements.ReplaceType(oldType, newType);
        }
    }
}
