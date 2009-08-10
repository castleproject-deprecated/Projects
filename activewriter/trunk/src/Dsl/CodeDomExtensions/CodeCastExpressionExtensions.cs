using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeCastExpressionExtensions
    {
        public static CodeCastExpression Clone(this CodeCastExpression expression)
        {
            if (expression == null) return null;
            CodeCastExpression e = new CodeCastExpression();
            e.Expression = expression.Expression.Clone();
            e.TargetType = expression.TargetType.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeCastExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.TargetType.ReplaceType(oldType, newType);
            expression.Expression.ReplaceType(oldType, newType);
        }
    }
}
