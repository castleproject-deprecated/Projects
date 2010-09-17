using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
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
