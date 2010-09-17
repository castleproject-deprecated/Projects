using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeCatchClauseExtensions
    {
        public static CodeCatchClause Clone(this CodeCatchClause clause)
        {
            if (clause == null) return null;
            CodeCatchClause c = new CodeCatchClause();
            c.CatchExceptionType = clause.CatchExceptionType.Clone();
            c.LocalName = clause.LocalName;
            c.Statements.AddRange(clause.Statements.Clone());
            return c;
        }

        public static void ReplaceType(this CodeCatchClause clause, string oldType, string newType)
        {
            if (clause == null) return;
            clause.CatchExceptionType.ReplaceType(oldType, newType);
            clause.Statements.ReplaceType(oldType, newType);
        }
    }
}
