using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeArrayIndexerExpressionExtensions
    {
        public static CodeArrayIndexerExpression Clone(this CodeArrayIndexerExpression expression)
        {
            if (expression == null) return null;
            CodeArrayIndexerExpression e = new CodeArrayIndexerExpression();
            e.Indices.AddRange(expression.Indices.Clone());
            e.TargetObject = expression.TargetObject.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeArrayIndexerExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.Indices.ReplaceType(oldType, newType);
            expression.TargetObject.ReplaceType(oldType, newType);
        }
    }
}
