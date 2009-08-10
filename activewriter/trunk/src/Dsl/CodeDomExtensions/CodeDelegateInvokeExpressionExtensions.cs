using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeDelegateInvokeExpressionExtensions
    {
        public static CodeDelegateInvokeExpression Clone(this CodeDelegateInvokeExpression expression)
        {
            if (expression == null) return null;
            CodeDelegateInvokeExpression e = new CodeDelegateInvokeExpression();
            e.Parameters.AddRange(expression.Parameters.Clone());
            e.TargetObject = expression.TargetObject.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeDelegateInvokeExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.Parameters.ReplaceType(oldType, newType);
            expression.TargetObject.ReplaceType(oldType, newType);
        }
    }
}
