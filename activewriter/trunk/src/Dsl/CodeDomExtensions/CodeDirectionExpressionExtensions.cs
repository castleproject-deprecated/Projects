using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeDirectionExpressionExtensions
    {
        public static CodeDirectionExpression Clone(this CodeDirectionExpression expression)
        {
            if (expression == null) return null;
            CodeDirectionExpression e = new CodeDirectionExpression();
            e.Direction = expression.Direction;
            e.Expression = expression.Expression.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeDirectionExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.Expression.ReplaceType(oldType, newType);
        }
    }
}
