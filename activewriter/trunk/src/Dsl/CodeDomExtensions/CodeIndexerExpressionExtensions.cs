using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeIndexerExpressionExtensions
    {
        public static CodeIndexerExpression Clone(this CodeIndexerExpression expression)
        {
            if (expression == null) return null;
            CodeIndexerExpression e = new CodeIndexerExpression();
            e.Indices.AddRange(expression.Indices.Clone());
            e.TargetObject = expression.TargetObject.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeIndexerExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.Indices.ReplaceType(oldType, newType);
            expression.TargetObject.ReplaceType(oldType, newType);
        }
    }
}
