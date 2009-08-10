using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeArrayCreateExpressionExtensions
    {
        public static CodeArrayCreateExpression Clone(this CodeArrayCreateExpression expression)
        {
            if (expression == null) return null;
            CodeArrayCreateExpression e = new CodeArrayCreateExpression();
            e.CreateType = expression.CreateType.Clone();
            e.Initializers.AddRange(expression.Initializers.Clone());
            e.Size = expression.Size;
            e.SizeExpression = expression.SizeExpression.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeArrayCreateExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.CreateType.ReplaceType(oldType, newType);
            expression.Initializers.ReplaceType(oldType, newType);
            expression.SizeExpression.ReplaceType(oldType, newType);
        }
    }
}
