using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
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
