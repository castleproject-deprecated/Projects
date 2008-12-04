using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;

namespace Altinoren.ActiveWriter.CodeGeneration.Adapters
{
    public class MethodAdapter
    {
        #region Method

        public static CodeTypeMember[] GetCompositeClassGetHashCodeMethods(List<CodeMemberField> fields, bool outputVisualBasic)
        //public static CodeTypeMember[] GetCompositeClassGetHashCodeMethods(List<CodeMemberField> fields)
        {
            //public override int GetHashCode()
            //{
            //  return _keyA.GetHashCode() ^ _keyB.GetHashCode();
            //}

            CodeTypeMember[] methods = new CodeTypeMember[2];

            CodeMemberMethod getHashCode = new CodeMemberMethod();
            getHashCode.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            getHashCode.ReturnType = new CodeTypeReference(typeof(Int32));
            getHashCode.Name = "GetHashCode";

            List<CodeExpression> expressions = new List<CodeExpression>();
            foreach (CodeMemberField field in fields)
            {
                expressions.Add(
                    new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(null, field.Name), "GetHashCode"));
            }

            // Now, there's no CodeBinaryOperatorType.XOr or something. We write a helper method instead.
            CodeMemberMethod xor = new CodeMemberMethod();
            xor.Attributes = MemberAttributes.Private;
            xor.ReturnType = new CodeTypeReference(typeof(Int32));
            xor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Int32), "left"));
            xor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Int32), "right"));
            xor.Name = Common.XorHelperMethod;
            if (outputVisualBasic)
                xor.Statements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("left XOR right")));
            else
                xor.Statements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("left ^ right")));

            CodeExpression expression;
            if (expressions.Count > 2)
                expression =
                    new CodeMethodInvokeExpression(null, Common.XorHelperMethod, expressions[0], GetXor(expressions, 1));
            else
                expression =
                    new CodeMethodInvokeExpression(null, Common.XorHelperMethod, expressions[0], expressions[1]);

            getHashCode.Statements.Add(new CodeMethodReturnStatement(expression));

            methods[0] = getHashCode;
            methods[1] = xor;

            return methods;
        }

        public static CodeTypeMember GetCompositeClassEqualsMethod(string className, List<CodeMemberField> fields)
        //public static CodeTypeMember GetCompositeClassEqualsMethod(string className, List<CodeMemberField> fields)
        {
            //public override bool Equals( object obj )
            //{
            //    if( obj == this ) return true;
            //    if( obj == null || obj.GetType() != this.GetType() ) return false;
            //    MyCompositeKey test = ( MyCompositeKey ) obj;
            //    return ( _keyA == test.KeyA || (_keyA != null && _keyA.Equals( test.KeyA ) ) ) &&
            //      ( _keyB == test.KeyB || ( _keyB != null && _keyB.Equals( test.KeyB ) ) );
            //}
            CodeMemberMethod equals = new CodeMemberMethod
            {
                Attributes = (MemberAttributes.Public | MemberAttributes.Override),
                ReturnType = new CodeTypeReference(typeof(Boolean)),
                Name = "Equals"
            };

            CodeParameterDeclarationExpression param = new CodeParameterDeclarationExpression(typeof(Object), "obj");
            equals.Parameters.Add(param);

            equals.Statements.Add(new CodeConditionStatement(
                                      new CodeBinaryOperatorExpression(
                                          new CodeFieldReferenceExpression(null, "obj"),
                                          CodeBinaryOperatorType.ValueEquality, new CodeThisReferenceExpression()
                                          ), new CodeMethodReturnStatement(new CodePrimitiveExpression(true))
                                      )
                );

            equals.Statements.Add(new CodeConditionStatement
                                      (
                                      new CodeBinaryOperatorExpression
                                          (
                                          new CodeBinaryOperatorExpression(
                                              new CodeFieldReferenceExpression(null, "obj"),
                                              CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(null)),
                                          CodeBinaryOperatorType.BooleanOr,
                                          new CodeBinaryOperatorExpression(
                                              new CodeMethodInvokeExpression(
                                                  new CodeFieldReferenceExpression(null, "obj"), "GetType"),
                                              CodeBinaryOperatorType.IdentityInequality,
                                              new CodeMethodInvokeExpression(new CodeThisReferenceExpression(),
                                                                             "GetType"))
                                          )
                                      , new CodeMethodReturnStatement(new CodePrimitiveExpression(false))
                                      )
                );

            equals.Statements.Add(
                new CodeVariableDeclarationStatement(new CodeTypeReference(className), "test",
                                                     new CodeCastExpression(new CodeTypeReference(className),
                                                                            new CodeFieldReferenceExpression(null, "obj"))));

            List<CodeExpression> expressions = new List<CodeExpression>();
            foreach (CodeMemberField field in fields)
            {
                expressions.Add(
                    new CodeBinaryOperatorExpression(
                    //_keyA == test.KeyA
                        new CodeBinaryOperatorExpression(
                            new CodeFieldReferenceExpression(null, field.Name),
                            CodeBinaryOperatorType.ValueEquality,
                            new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(null, "test"), field.Name)),
                        CodeBinaryOperatorType.BooleanOr, // ||
                        new CodeBinaryOperatorExpression(
                    //_keyA != null
                            new CodeBinaryOperatorExpression(
                                new CodeFieldReferenceExpression(null, field.Name),
                                CodeBinaryOperatorType.IdentityInequality,
                                new CodePrimitiveExpression(null)
                                ),
                            CodeBinaryOperatorType.BooleanAnd, // &&
                    // _keyA.Equals( test.KeyA )   
                            new CodeMethodInvokeExpression(
                                new CodeFieldReferenceExpression(null, field.Name), "Equals",
                                new CodeFieldReferenceExpression(
                                    new CodeFieldReferenceExpression(null, "test"), field.Name))
                            )
                        )
                    );
            }

            CodeExpression expression;
            if (expressions.Count > 2)
                expression =
                    new CodeBinaryOperatorExpression(expressions[0], CodeBinaryOperatorType.BooleanAnd,
                                                     GetBooleanAnd(expressions, 1));
            else
                expression =
                    new CodeBinaryOperatorExpression(expressions[0], CodeBinaryOperatorType.BooleanAnd, expressions[1]);


            equals.Statements.Add(new CodeMethodReturnStatement(expression));

            return equals;
        }

        public static CodeMemberMethod GetCompositeClassToStringMethod(List<CodeMemberField> fields)
        //public static CodeMemberMethod GetCompositeClassToStringMethod(List<CodeMemberField> fields)
        {
            //public override string ToString()
            //{
            //  return string.Join( ":", new string[] { _keyA, _keyB } );
            //}
            CodeMemberMethod toString = new CodeMemberMethod
            {
                Attributes = (MemberAttributes.Public | MemberAttributes.Override),
                ReturnType = new CodeTypeReference(typeof(String)),
                Name = "ToString"
            };

            CodeExpression[] expressions = new CodeExpression[fields.Count];
            for (int i = 0; i < fields.Count; i++)
            {
                expressions[i] =
                    new CodeMethodInvokeExpression(
                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fields[i].Name), "ToString");
            }

            toString.Statements.Add(new CodeMethodReturnStatement(
                                        new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Join",
                                                                       new CodeSnippetExpression("\":\""),
                                                                       new CodeArrayCreateExpression(typeof(String),
                                                                                                     expressions)))
                );
            return toString;
        }

        public static CodeExpression GetXor(IList<CodeExpression> expressions, int i)
        {
            if (i == expressions.Count - 2)
                return new CodeMethodInvokeExpression(null, Common.XorHelperMethod, expressions[i], expressions[i + 1]);

            return
                new CodeMethodInvokeExpression(null, Common.XorHelperMethod, expressions[i],
                                               GetXor(expressions, i + 1));
        }

        public static CodeExpression GetBooleanAnd(List<CodeExpression> expressions, int i)
        {
            if (i == expressions.Count - 2)
                return
                    new CodeBinaryOperatorExpression(expressions[i], CodeBinaryOperatorType.BooleanAnd,
                                                     expressions[i + 1]);

            return
                new CodeBinaryOperatorExpression(expressions[i], CodeBinaryOperatorType.BooleanAnd,
                                                 GetBooleanAnd(expressions, i + 1));
        }

        #endregion
    }
}
