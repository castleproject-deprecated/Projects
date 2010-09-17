using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter.CodeDomExtensions
{
    public static class CodeParameterDeclarationExpressionExtensions
    {
        public static CodeParameterDeclarationExpression Clone(this CodeParameterDeclarationExpression expression)
        {
            if (expression == null) return null;
            CodeParameterDeclarationExpression e = new CodeParameterDeclarationExpression();
            e.CustomAttributes = expression.CustomAttributes.Clone();
            e.Direction = expression.Direction;
            e.Name = expression.Name;
            e.Type = expression.Type.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeParameterDeclarationExpression parameter, string oldType, string newType)
        {
            if (parameter == null) return;
            parameter.CustomAttributes.ReplaceType(oldType, newType);
            parameter.Type.ReplaceType(oldType, newType);
        }

        public static bool ContainsType(this CodeParameterDeclarationExpression parameter, string type)
        {
            return parameter.Type.ContainsType(type);
        }
    }
}
