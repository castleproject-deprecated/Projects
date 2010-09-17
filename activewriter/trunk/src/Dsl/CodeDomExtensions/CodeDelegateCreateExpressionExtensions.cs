using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeDelegateCreateExpressionExtensions
    {
        public static CodeDelegateCreateExpression Clone(this CodeDelegateCreateExpression expression)
        {
            if (expression == null) return null;
            CodeDelegateCreateExpression e = new CodeDelegateCreateExpression();
            e.DelegateType = expression.DelegateType.Clone();
            e.MethodName = expression.MethodName;
            e.TargetObject = expression.TargetObject.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeDelegateCreateExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.DelegateType.ReplaceType(oldType, newType);
            expression.TargetObject.ReplaceType(oldType, newType);
        }
    }
}
