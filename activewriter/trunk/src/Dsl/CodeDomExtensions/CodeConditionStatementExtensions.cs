using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeConditionStatementExtensions
    {
        public static CodeConditionStatement Clone(this CodeConditionStatement statement)
        {
            if (statement == null) return null;
            CodeConditionStatement s = new CodeConditionStatement();
            s.Condition = statement.Condition.Clone();
            s.EndDirectives.AddRange(statement.EndDirectives);
            s.FalseStatements.AddRange(statement.FalseStatements.Clone());
            s.LinePragma = statement.LinePragma;
            s.StartDirectives.AddRange(statement.StartDirectives);
            s.TrueStatements.AddRange(statement.TrueStatements.Clone());
            s.UserData.AddRange(statement.UserData);
            return s;
        }

        public static void ReplaceType(this CodeConditionStatement statement, string oldType, string newType)
        {
            if (statement == null) return;
            statement.Condition.ReplaceType(oldType, newType);
            statement.FalseStatements.ReplaceType(oldType, newType);
            statement.TrueStatements.ReplaceType(oldType, newType);
        }
    }
}
