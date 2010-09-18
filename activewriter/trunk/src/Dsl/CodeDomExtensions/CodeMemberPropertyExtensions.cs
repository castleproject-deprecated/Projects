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

	public static class CodeMemberPropertyExtensions
    {
        public static CodeMemberProperty Clone(this CodeMemberProperty property)
        {
            if (property == null) return null;

            CodeMemberProperty p = new CodeMemberProperty();
            p.Attributes = property.Attributes;
            p.Comments.AddRange(property.Comments);
            p.CustomAttributes = property.CustomAttributes.Clone();
            p.EndDirectives.AddRange(property.EndDirectives);
            p.GetStatements.AddRange(property.GetStatements.Clone());
            p.HasGet = property.HasGet;
            p.HasSet = property.HasSet;
            p.ImplementationTypes.AddRange(property.ImplementationTypes.Clone());
            p.LinePragma = property.LinePragma;
            p.Name = property.Name;
            p.Parameters.AddRange(property.Parameters.Clone());
            p.PrivateImplementationType = property.PrivateImplementationType.Clone();
            p.SetStatements.AddRange(property.SetStatements.Clone());
            p.StartDirectives.AddRange(property.StartDirectives);
            p.Type = property.Type.Clone();
            p.UserData.AddRange(property.UserData);
            return p;
        }

        public static void ReplaceType(this CodeMemberProperty property, string oldType, string newType)
        {
            if (property == null) return;

            // ImplementationTypes aren't replaced.  See comment in CodeMemberMethodExtensions.

            property.CustomAttributes.ReplaceType(oldType, newType);
            property.GetStatements.ReplaceType(oldType, newType);
            property.Parameters.ReplaceType(oldType, newType);
            property.SetStatements.ReplaceType(oldType, newType);
            property.Type.ReplaceType(oldType, newType);
        }
    }
}
