using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeTypeReferenceCollectionExtensions
    {
        public static CodeTypeReferenceCollection Clone(this CodeTypeReferenceCollection collection)
        {
            if (collection == null) return null;
            CodeTypeReferenceCollection c = new CodeTypeReferenceCollection();
            foreach (CodeTypeReference reference in collection)
                c.Add(reference.Clone());
            return c;
        }

        public static void ReplaceType(this CodeTypeReferenceCollection collection, string oldType, string newType)
        {
            if (collection == null) return;
            foreach (CodeTypeReference reference in collection)
                reference.ReplaceType(oldType, newType);
        }

        public static bool ContainsType(this CodeTypeReferenceCollection collection, string type)
        {
            foreach (CodeTypeReference reference in collection)
                if (reference.ContainsType(type))
                    return true;
            return false;
        }
    }
}
