using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
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
