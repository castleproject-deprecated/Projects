using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeCatchClauseCollectionExtensions
    {
        public static CodeCatchClauseCollection Clone(this CodeCatchClauseCollection collection)
        {
            if (collection == null) return null;
            CodeCatchClauseCollection c = new CodeCatchClauseCollection();
            foreach (CodeCatchClause clause in collection)
                c.Add(clause.Clone());
            return c;
        }

        public static void ReplaceType(this CodeCatchClauseCollection collection, string oldType, string newType)
        {
            if (collection == null) return;
            foreach (CodeCatchClause clause in collection)
                clause.ReplaceType(oldType, newType);
        }
    }
}
