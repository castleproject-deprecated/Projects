using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
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
