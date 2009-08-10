using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeObjectCreateExpressionExtensions
    {
        public static CodeObjectCreateExpression Clone(this CodeObjectCreateExpression expression)
        {
            if (expression == null) return null;
            CodeObjectCreateExpression e = new CodeObjectCreateExpression();
            e.CreateType = expression.CreateType.Clone();
            e.Parameters.AddRange(expression.Parameters.Clone());
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeObjectCreateExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.CreateType.ReplaceType(oldType, newType);
            expression.Parameters.ReplaceType(oldType, newType);
        }
    }
}
