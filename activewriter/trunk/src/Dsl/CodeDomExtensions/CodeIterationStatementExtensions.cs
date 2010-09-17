using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeIterationStatementExtensions
    {
        public static CodeIterationStatement Clone(this CodeIterationStatement statement)
        {
            if (statement == null) return null;
            CodeIterationStatement s = new CodeIterationStatement();
            s.EndDirectives.AddRange(statement.EndDirectives);
            s.IncrementStatement = statement.IncrementStatement.Clone();
            s.InitStatement = statement.InitStatement.Clone();
            s.LinePragma = statement.LinePragma;
            s.StartDirectives.AddRange(statement.StartDirectives);
            s.Statements.AddRange(statement.Statements.Clone());
            s.TestExpression = statement.TestExpression.Clone();
            s.UserData.AddRange(statement.UserData);
            return s;
        }

        public static void ReplaceType(this CodeIterationStatement statement, string oldType, string newType)
        {
            if (statement == null) return;
            statement.IncrementStatement.ReplaceType(oldType, newType);
            statement.InitStatement.ReplaceType(oldType, newType);
            statement.Statements.ReplaceType(oldType, newType);
            statement.TestExpression.ReplaceType(oldType, newType);
        }
    }
}
