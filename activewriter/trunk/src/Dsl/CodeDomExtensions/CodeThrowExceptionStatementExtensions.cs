using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeThrowExceptionStatementExtensions
    {
        public static CodeThrowExceptionStatement Clone(this CodeThrowExceptionStatement statement)
        {
            if (statement == null) return null;
            CodeThrowExceptionStatement s = new CodeThrowExceptionStatement();
            s.EndDirectives.AddRange(statement.EndDirectives);
            s.LinePragma = statement.LinePragma;
            s.StartDirectives.AddRange(statement.StartDirectives);
            s.ToThrow = statement.ToThrow.Clone();
            s.UserData.AddRange(statement.UserData);
            return s;
        }

        public static void ReplaceType(this CodeThrowExceptionStatement statement, string oldType, string newType)
        {
            if (statement == null) return;
            statement.ToThrow.ReplaceType(oldType, newType);
        }
    }
}
