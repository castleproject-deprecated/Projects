using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeMethodReturnStatementExtensions
    {
        public static CodeMethodReturnStatement Clone(this CodeMethodReturnStatement statement)
        {
            if (statement == null) return null;
            CodeMethodReturnStatement s = new CodeMethodReturnStatement();
            s.EndDirectives.AddRange(statement.EndDirectives);
            s.Expression = statement.Expression.Clone();
            s.LinePragma = statement.LinePragma;
            s.StartDirectives.AddRange(statement.StartDirectives);
            s.UserData.AddRange(statement.UserData);
            return s;
        }

        public static void ReplaceType(this CodeMethodReturnStatement statement, string oldType, string newType)
        {
            if (statement == null) return;
            statement.Expression.ReplaceType(oldType, newType);
        }
    }
}
