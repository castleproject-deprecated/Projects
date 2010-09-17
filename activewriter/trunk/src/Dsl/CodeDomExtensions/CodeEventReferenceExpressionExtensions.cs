using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeEventReferenceExpressionExtensions
    {
        public static CodeEventReferenceExpression Clone(this CodeEventReferenceExpression expression)
        {
            if (expression == null) return null;
            CodeEventReferenceExpression e = new CodeEventReferenceExpression();
            e.EventName = expression.EventName;
            e.TargetObject = expression.TargetObject.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeEventReferenceExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.TargetObject.ReplaceType(oldType, newType);
        }
    }
}
