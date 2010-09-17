using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeTypeOfExpressionExtensions
    {
        public static CodeTypeOfExpression Clone(this CodeTypeOfExpression expression)
        {
            if (expression == null) return null;
            CodeTypeOfExpression e = new CodeTypeOfExpression();
            e.Type = expression.Type.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeTypeOfExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.Type.ReplaceType(oldType, newType);
        }
    }
}
