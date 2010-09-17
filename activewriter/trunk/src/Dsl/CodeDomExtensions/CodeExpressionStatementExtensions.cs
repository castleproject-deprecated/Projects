using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeExpressionStatementExtensions
    {
        public static CodeExpressionStatement Clone(this CodeExpressionStatement statement)
        {
            if (statement == null) return null;
            CodeExpressionStatement s = new CodeExpressionStatement();
            s.EndDirectives.AddRange(statement.EndDirectives);
            s.Expression = statement.Expression.Clone();
            s.LinePragma = statement.LinePragma;
            s.StartDirectives.AddRange(statement.StartDirectives);
            s.UserData.AddRange(statement.UserData);
            return s;
        }

        public static void ReplaceType(this CodeExpressionStatement statement, string oldType, string newType)
        {
            if (statement == null) return;
            statement.Expression.ReplaceType(oldType, newType);
        }
    }
}
