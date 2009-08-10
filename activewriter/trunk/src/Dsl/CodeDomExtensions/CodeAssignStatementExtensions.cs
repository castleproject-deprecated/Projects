using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeAssignStatementExtensions
    {
        public static CodeAssignStatement Clone(this CodeAssignStatement statement)
        {
            if (statement == null) return null;
            CodeAssignStatement s = new CodeAssignStatement();
            s.EndDirectives.AddRange(statement.EndDirectives);
            s.Left = statement.Left.Clone();
            s.LinePragma = statement.LinePragma;
            s.Right = statement.Right.Clone();
            s.StartDirectives.AddRange(statement.StartDirectives);
            s.UserData.AddRange(statement.UserData);
            return s;
        }

        public static void ReplaceType(this CodeAssignStatement statement, string oldType, string newType)
        {
            if (statement == null) return;
            statement.Left.ReplaceType(oldType, newType);
            statement.Right.ReplaceType(oldType, newType);
        }
    }
}
