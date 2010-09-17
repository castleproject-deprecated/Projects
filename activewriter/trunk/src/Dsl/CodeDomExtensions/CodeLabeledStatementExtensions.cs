using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeLabeledStatementExtensions
    {
        public static CodeLabeledStatement Clone(this CodeLabeledStatement statement)
        {
            if (statement == null) return null;
            CodeLabeledStatement s = new CodeLabeledStatement();
            s.EndDirectives.AddRange(statement.EndDirectives);
            s.Label = statement.Label;
            s.LinePragma = statement.LinePragma;
            s.StartDirectives.AddRange(statement.StartDirectives);
            s.Statement = statement.Statement.Clone();
            s.UserData.AddRange(statement.UserData);
            return s;
        }

        public static void ReplaceType(this CodeLabeledStatement statement, string oldType, string newType)
        {
            if (statement == null) return;
            statement.Statement.ReplaceType(oldType, newType);
        }
    }
}
