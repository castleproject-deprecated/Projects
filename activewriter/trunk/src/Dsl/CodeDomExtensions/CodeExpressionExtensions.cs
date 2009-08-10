using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeExpressionExtensions
    {
        public static CodeExpression Clone(this CodeExpression expression)
        {
            if (expression == null) return null;

            if (expression is CodeArrayCreateExpression)
                return (expression as CodeArrayCreateExpression).Clone();
            if (expression is CodeArrayIndexerExpression)
                return (expression as CodeArrayIndexerExpression).Clone();
            if (expression is CodeBinaryOperatorExpression)
                return (expression as CodeBinaryOperatorExpression).Clone();
            if (expression is CodeCastExpression)
                return (expression as CodeCastExpression).Clone();
            if (expression is CodeDelegateCreateExpression)
                return (expression as CodeDelegateCreateExpression).Clone();
            if (expression is CodeDelegateInvokeExpression)
                return (expression as CodeDelegateInvokeExpression).Clone();
            if (expression is CodeDirectionExpression)
                return (expression as CodeDirectionExpression).Clone();
            if (expression is CodeEventReferenceExpression)
                return (expression as CodeEventReferenceExpression).Clone();
            if (expression is CodeFieldReferenceExpression)
                return (expression as CodeFieldReferenceExpression).Clone();
            if (expression is CodeIndexerExpression)
                return (expression as CodeIndexerExpression).Clone();
            if (expression is CodeMethodInvokeExpression)
                return (expression as CodeMethodInvokeExpression).Clone();
            if (expression is CodeMethodReferenceExpression)
                return (expression as CodeMethodReferenceExpression).Clone();
            if (expression is CodeObjectCreateExpression)
                return (expression as CodeObjectCreateExpression).Clone();
            if (expression is CodeParameterDeclarationExpression)
                return (expression as CodeParameterDeclarationExpression).Clone();
            if (expression is CodePropertyReferenceExpression)
                return (expression as CodePropertyReferenceExpression).Clone();
            if (expression is CodeTypeOfExpression)
                return (expression as CodeTypeOfExpression).Clone();
            if (expression is CodeTypeReferenceExpression)
                return (expression as CodeTypeReferenceExpression).Clone();

            // The following types don't clone anything since there are no types to replace internally.
            // If this method is used later for more sophisticated purposes, the methods might need to
            // be implemented in more detail.  The other methods above might require alteration as well.
            if (expression is CodeArgumentReferenceExpression)
                return expression;
            if (expression is CodeBaseReferenceExpression)
                return expression;
            if (expression is CodePrimitiveExpression)
                return expression;
            if (expression is CodePropertySetValueReferenceExpression)
                return expression;
            if (expression is CodeSnippetExpression)
                return expression;
            if (expression is CodeThisReferenceExpression)
                return expression;
            if (expression is CodeVariableReferenceExpression)
                return expression;

            throw new NotImplementedException("Clone has not been implemented for expression of type: " + expression.GetType().FullName);
        }

        public static void ReplaceType(this CodeExpression expression, string oldType, string newType)
        {
            if (expression == null) return;

            if (expression is CodeArrayCreateExpression)
                (expression as CodeArrayCreateExpression).ReplaceType(oldType, newType);
            else if (expression is CodeArrayIndexerExpression)
                (expression as CodeArrayIndexerExpression).ReplaceType(oldType, newType);
            else if (expression is CodeBinaryOperatorExpression)
                (expression as CodeBinaryOperatorExpression).ReplaceType(oldType, newType);
            else if (expression is CodeCastExpression)
                (expression as CodeCastExpression).ReplaceType(oldType, newType);
            else if (expression is CodeDelegateCreateExpression)
                (expression as CodeDelegateCreateExpression).ReplaceType(oldType, newType);
            else if (expression is CodeDelegateInvokeExpression)
                (expression as CodeDelegateInvokeExpression).ReplaceType(oldType, newType);
            else if (expression is CodeDirectionExpression)
                (expression as CodeDirectionExpression).ReplaceType(oldType, newType);
            else if (expression is CodeEventReferenceExpression)
                (expression as CodeEventReferenceExpression).ReplaceType(oldType, newType);
            else if (expression is CodeFieldReferenceExpression)
                (expression as CodeFieldReferenceExpression).ReplaceType(oldType, newType);
            else if (expression is CodeIndexerExpression)
                (expression as CodeIndexerExpression).ReplaceType(oldType, newType);
            else if (expression is CodeMethodInvokeExpression)
                (expression as CodeMethodInvokeExpression).ReplaceType(oldType, newType);
            else if (expression is CodeMethodReferenceExpression)
                (expression as CodeMethodReferenceExpression).ReplaceType(oldType, newType);
            else if (expression is CodeObjectCreateExpression)
                (expression as CodeObjectCreateExpression).ReplaceType(oldType, newType);
            else if (expression is CodeParameterDeclarationExpression)
                (expression as CodeParameterDeclarationExpression).ReplaceType(oldType, newType);
            else if (expression is CodePropertyReferenceExpression)
                (expression as CodePropertyReferenceExpression).ReplaceType(oldType, newType);
            else if (expression is CodeTypeOfExpression)
                (expression as CodeTypeOfExpression).ReplaceType(oldType, newType);
            else if (expression is CodeTypeReferenceExpression)
                (expression as CodeTypeReferenceExpression).ReplaceType(oldType, newType);

            // The following types don't contain any types to replace.
            else if (expression is CodeArgumentReferenceExpression)
                return;
            else if (expression is CodeBaseReferenceExpression)
                return;
            else if (expression is CodePrimitiveExpression)
                return;
            else if (expression is CodePropertySetValueReferenceExpression)
                return;
            else if (expression is CodeSnippetExpression)
                return;
            else if (expression is CodeThisReferenceExpression)
                return;
            else if (expression is CodeVariableReferenceExpression)
                return;

            // For not-implemented types.
            else
                throw new NotImplementedException("ReplaceType has not been implemented for expression of type: " + expression.GetType().FullName);
        }
    }
}
