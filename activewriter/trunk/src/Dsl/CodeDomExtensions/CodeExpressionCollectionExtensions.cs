using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeExpressionCollectionExtensions
    {
        public static CodeExpressionCollection Clone(this CodeExpressionCollection collection)
        {
            if (collection == null) return null;
            CodeExpressionCollection c = new CodeExpressionCollection();
            foreach (CodeExpression expression in collection)
                c.Add(expression.Clone());
            return c;
        }

        public static void ReplaceType(this CodeExpressionCollection collection, string oldType, string newType)
        {
            if (collection == null) return;
            foreach (CodeExpression expression in collection)
                expression.ReplaceType(oldType, newType);
        }
    }
}
