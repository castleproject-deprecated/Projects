using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeStatementExtensions
    {
        public static CodeStatement Clone(this CodeStatement statement)
        {
            if (statement == null) return null;

            if (statement is CodeAssignStatement)
                return (statement as CodeAssignStatement).Clone();
            if (statement is CodeAttachEventStatement)
                return (statement as CodeAttachEventStatement).Clone();
            if (statement is CodeConditionStatement)
                return (statement as CodeConditionStatement).Clone();
            if (statement is CodeExpressionStatement)
                return (statement as CodeExpressionStatement).Clone();
            if (statement is CodeIterationStatement)
                return (statement as CodeIterationStatement).Clone();
            if (statement is CodeLabeledStatement)
                return (statement as CodeLabeledStatement).Clone();
            if (statement is CodeMethodReturnStatement)
                return (statement as CodeMethodReturnStatement).Clone();
            if (statement is CodeRemoveEventStatement)
                return (statement as CodeRemoveEventStatement).Clone();
            if (statement is CodeThrowExceptionStatement)
                return (statement as CodeThrowExceptionStatement).Clone();
            if (statement is CodeTryCatchFinallyStatement)
                return (statement as CodeTryCatchFinallyStatement).Clone();
            if (statement is CodeVariableDeclarationStatement)
                return (statement as CodeVariableDeclarationStatement).Clone();

            // The following statements don't have any types to replace, so we don't need to
            // change anything about them right now.  If we were doing something more
            // advanced in the future, we might need to do more work here.
            if (statement is CodeCommentStatement)
                return statement;
            if (statement is CodeGotoStatement)
                return statement;
            if (statement is CodeSnippetStatement)
                return statement;

            throw new NotImplementedException("Clone has not been implemented for statement of type: " + statement.GetType().FullName);
        }

        public static void ReplaceType(this CodeStatement statement, string oldType, string newType)
        {
            if (statement == null) return;

            if (statement is CodeAssignStatement)
                (statement as CodeAssignStatement).ReplaceType(oldType, newType);
            else if (statement is CodeAttachEventStatement)
                (statement as CodeAttachEventStatement).ReplaceType(oldType, newType);
            else if (statement is CodeConditionStatement)
                (statement as CodeConditionStatement).ReplaceType(oldType, newType);
            else if (statement is CodeExpressionStatement)
                (statement as CodeExpressionStatement).ReplaceType(oldType, newType);
            else if (statement is CodeIterationStatement)
                (statement as CodeIterationStatement).ReplaceType(oldType, newType);
            else if (statement is CodeLabeledStatement)
                (statement as CodeLabeledStatement).ReplaceType(oldType, newType);
            else if (statement is CodeMethodReturnStatement)
                (statement as CodeMethodReturnStatement).ReplaceType(oldType, newType);
            else if (statement is CodeRemoveEventStatement)
                (statement as CodeRemoveEventStatement).ReplaceType(oldType, newType);
            else if (statement is CodeThrowExceptionStatement)
                (statement as CodeThrowExceptionStatement).ReplaceType(oldType, newType);
            else if (statement is CodeTryCatchFinallyStatement)
                (statement as CodeTryCatchFinallyStatement).ReplaceType(oldType, newType);
            else if (statement is CodeVariableDeclarationStatement)
                (statement as CodeVariableDeclarationStatement).ReplaceType(oldType, newType);

            // The following statements don't have any types to replace.
            else if (statement is CodeCommentStatement)
                return;
            else if (statement is CodeGotoStatement)
                return;
            else if (statement is CodeSnippetStatement)
                return;

            // Unknown statement types.
            else
                throw new NotImplementedException("ReplaceType has not been implemented for statement of type: " + statement.GetType().FullName);
        }
    }
}
