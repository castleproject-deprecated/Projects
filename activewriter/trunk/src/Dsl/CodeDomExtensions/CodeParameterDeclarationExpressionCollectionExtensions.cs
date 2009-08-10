using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeParameterDeclarationExpressionCollectionExtensions
    {
        public static CodeParameterDeclarationExpressionCollection Clone(this CodeParameterDeclarationExpressionCollection collection)
        {
            if (collection == null) return null;
            CodeParameterDeclarationExpressionCollection c = new CodeParameterDeclarationExpressionCollection();
            foreach (CodeParameterDeclarationExpression expression in collection)
                c.Add(expression.Clone());
            return c;
        }

        public static void ReplaceType(this CodeParameterDeclarationExpressionCollection collection, string oldType, string newType)
        {
            if (collection == null) return;
            foreach (CodeParameterDeclarationExpression parameter in collection)
                parameter.ReplaceType(oldType, newType);
        }

        public static bool ContainsType(this CodeParameterDeclarationExpressionCollection collection, string type)
        {
            foreach (CodeParameterDeclarationExpression param in collection)
                if (param.ContainsType(type))
                    return true;
            return false;
        }
    }
}
