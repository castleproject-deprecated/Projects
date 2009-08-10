using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeMethodReferenceExpressionExtensions
    {
        public static CodeMethodReferenceExpression Clone(this CodeMethodReferenceExpression expression)
        {
            if (expression == null) return null;
            CodeMethodReferenceExpression e = new CodeMethodReferenceExpression();
            e.MethodName = expression.MethodName;
            e.TargetObject = expression.TargetObject.Clone();
            e.TypeArguments.AddRange(expression.TypeArguments.Clone());
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeMethodReferenceExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.TargetObject.ReplaceType(oldType, newType);
            expression.TypeArguments.ReplaceType(oldType, newType);
        }
    }
}
