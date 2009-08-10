using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeBinaryOperatorExpressionExtensions
    {
        public static CodeBinaryOperatorExpression Clone(this CodeBinaryOperatorExpression expression)
        {
            if (expression == null) return null;
            CodeBinaryOperatorExpression e = new CodeBinaryOperatorExpression();
            e.Left = expression.Left.Clone();
            e.Operator = expression.Operator;
            e.Right = expression.Right.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeBinaryOperatorExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.Left.ReplaceType(oldType, newType);
            expression.Right.ReplaceType(oldType, newType);
        }
    }
}
