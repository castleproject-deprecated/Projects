using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeTypeReferenceExpressionExtensions
    {
        public static CodeTypeReferenceExpression Clone(this CodeTypeReferenceExpression expression)
        {
            if (expression == null) return null;
            CodeTypeReferenceExpression e = new CodeTypeReferenceExpression();
            e.Type = expression.Type.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeTypeReferenceExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.Type.ReplaceType(oldType, newType);
        }
    }
}
