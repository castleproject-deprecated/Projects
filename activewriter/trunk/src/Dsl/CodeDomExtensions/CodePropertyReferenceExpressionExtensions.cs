using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodePropertyReferenceExpressionExtensions
    {
        public static CodePropertyReferenceExpression Clone(this CodePropertyReferenceExpression expression)
        {
            if (expression == null) return null;
            CodePropertyReferenceExpression e = new CodePropertyReferenceExpression();
            e.PropertyName = expression.PropertyName;
            e.TargetObject = expression.TargetObject.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodePropertyReferenceExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.TargetObject.ReplaceType(oldType, newType);
        }
    }
}
