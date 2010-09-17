using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeAttachEventStatementExtensions
    {
        public static CodeAttachEventStatement Clone(this CodeAttachEventStatement statement)
        {
            if (statement == null) return null;
            CodeAttachEventStatement s = new CodeAttachEventStatement();
            s.EndDirectives.AddRange(statement.EndDirectives);
            s.Event = statement.Event.Clone();
            s.LinePragma = statement.LinePragma;
            s.Listener = statement.Listener.Clone();
            s.StartDirectives.AddRange(statement.StartDirectives);
            s.UserData.AddRange(statement.UserData);
            return s;
        }

        public static void ReplaceType(this CodeAttachEventStatement statement, string oldType, string newType)
        {
            if (statement == null) return;
            statement.Event.ReplaceType(oldType, newType);
            statement.Listener.ReplaceType(oldType, newType);
        }
    }
}
