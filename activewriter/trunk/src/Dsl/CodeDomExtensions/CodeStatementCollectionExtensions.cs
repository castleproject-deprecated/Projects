using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeStatementCollectionExtensions
    {
        public static CodeStatementCollection Clone(this CodeStatementCollection collection)
        {
            if (collection == null) return null;
            CodeStatementCollection c = new CodeStatementCollection();
            foreach (CodeStatement statement in collection)
                c.Add(statement.Clone());
            return c;
        }

        public static void ReplaceType(this CodeStatementCollection statements, string oldType, string newType)
        {
            if (statements == null) return;
            foreach (CodeStatement statement in statements)
                statement.ReplaceType(oldType, newType);
        }
    }
}
