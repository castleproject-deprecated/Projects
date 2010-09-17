using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeTryCatchFinallyStatementExtensions
    {
        public static CodeTryCatchFinallyStatement Clone(this CodeTryCatchFinallyStatement statement)
        {
            if (statement == null) return null;
            CodeTryCatchFinallyStatement s = new CodeTryCatchFinallyStatement();
            s.CatchClauses.AddRange(statement.CatchClauses.Clone());
            s.EndDirectives.AddRange(statement.EndDirectives);
            s.FinallyStatements.AddRange(statement.FinallyStatements.Clone());
            s.LinePragma = statement.LinePragma;
            s.StartDirectives.AddRange(statement.StartDirectives);
            s.TryStatements.AddRange(statement.TryStatements.Clone());
            s.UserData.AddRange(statement.UserData);
            return s;
        }

        public static void ReplaceType(this CodeTryCatchFinallyStatement statement, string oldType, string newType)
        {
            if (statement == null) return;
            statement.CatchClauses.ReplaceType(oldType, newType);
            statement.FinallyStatements.ReplaceType(oldType, newType);
            statement.TryStatements.ReplaceType(oldType, newType);
        }
    }
}
