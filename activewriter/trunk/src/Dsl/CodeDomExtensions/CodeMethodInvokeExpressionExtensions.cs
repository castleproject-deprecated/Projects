using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeMethodInvokeExpressionExtensions
    {
        public static CodeMethodInvokeExpression Clone(this CodeMethodInvokeExpression expression)
        {
            if (expression == null) return null;
            CodeMethodInvokeExpression e = new CodeMethodInvokeExpression();
            e.Method = expression.Method.Clone();
            e.Parameters.AddRange(expression.Parameters.Clone());
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeMethodInvokeExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.Method.ReplaceType(oldType, newType);
            expression.Parameters.ReplaceType(oldType, newType);
        }
    }
}
