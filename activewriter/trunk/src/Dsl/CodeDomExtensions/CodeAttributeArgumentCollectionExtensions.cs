using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeAttributeArgumentCollectionExtensions
    {
        public static CodeAttributeArgumentCollection Clone(this CodeAttributeArgumentCollection collection)
        {
            if (collection == null) return null;
            CodeAttributeArgumentCollection c = new CodeAttributeArgumentCollection();
            foreach (CodeAttributeArgument argument in collection)
                c.Add(argument.Clone());
            return c;
        }

        public static void ReplaceType(this CodeAttributeArgumentCollection collection, string oldType, string newType)
        {
            if (collection == null) return;
            foreach (CodeAttributeArgument argument in collection)
                argument.ReplaceType(oldType, newType);
        }
    }
}
