using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeVariableDeclarationStatementExtensions
    {
        public static CodeVariableDeclarationStatement Clone(this CodeVariableDeclarationStatement statement)
        {
            if (statement == null) return null;
            CodeVariableDeclarationStatement s = new CodeVariableDeclarationStatement();
            s.EndDirectives.AddRange(statement.EndDirectives);
            s.InitExpression = statement.InitExpression.Clone();
            s.LinePragma = statement.LinePragma;
            s.Name = statement.Name;
            s.StartDirectives.AddRange(statement.StartDirectives);
            s.Type = statement.Type.Clone();
            s.UserData.AddRange(statement.UserData);
            return s;
        }

        public static void ReplaceType(this CodeVariableDeclarationStatement statement, string oldType, string newType)
        {
            if (statement == null) return;
            statement.InitExpression.ReplaceType(oldType, newType);
            statement.Type.ReplaceType(oldType, newType);
        }
    }
}
