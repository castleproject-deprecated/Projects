// Copyright 2007 Jonathon Rossi - http://www.jonorossi.com/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.NVelocity.Tests.ParserTests
{
    using Castle.NVelocity.Ast;
    using NUnit.Framework;

    [TestFixture]
    public class NVelocityExpressionParserTestCase : ParserTestBase
    {
        [Test]
        public void ParseOrExpression()
        {
            NVBinaryExpression expr = (NVBinaryExpression)GetExpressionFromTemplate(
                "true || false");

            // Check that LHS and RHS are BooleanExpressions and the operator is Or
            Assert.AreEqual(Operator.Or, expr.Op);
            Assert.AreEqual(true, ((NVBoolExpression)expr.Lhs).Value);
            Assert.AreEqual(false, ((NVBoolExpression)expr.Rhs).Value);
        }

        [Test]
        public void ParseAndExpression()
        {
            NVBinaryExpression expr = (NVBinaryExpression)GetExpressionFromTemplate(
                "true && true and true");

            // The AST should look like this:
            //    and
            //    / \
            //  and  true
            //  / \
            //true true

            // Check that there is a BinaryExpression with operator 'And'
            Assert.AreEqual(Operator.And, expr.Op);

            // Check the LHS of the expr
            Assert.AreEqual(Operator.And, ((NVBinaryExpression)expr.Lhs).Op);
            Assert.AreEqual(true, ((NVBoolExpression)((NVBinaryExpression)expr.Lhs).Lhs).Value);
            Assert.AreEqual(true, ((NVBoolExpression)((NVBinaryExpression)expr.Lhs).Rhs).Value);

            // Check the RHS of the expr
            Assert.AreEqual(true, ((NVBoolExpression)expr.Rhs).Value);
        }

        [Test]
        public void ParseBinaryExpressionWithRelOp()
        {
            NVBinaryExpression expr = (NVBinaryExpression)GetExpressionFromTemplate(
                "1 <= 2 && true ne false");

            // The AST should look like this:
            //     and
            //    /   \
            //  <=     !=
            //  / \    / \
            // 1   2   T  F

            // Check that there is a BinaryExpression with operator 'and'
            Assert.AreEqual(Operator.And, expr.Op);

            // Check that expr.LHS operator is Lte
            Assert.AreEqual(Operator.Lte, ((NVBinaryExpression) expr.Lhs).Op);
            Assert.AreEqual(1, ((NVNumExpression)((NVBinaryExpression)expr.Lhs).Lhs).Value);
            Assert.AreEqual(2, ((NVNumExpression)((NVBinaryExpression)expr.Lhs).Rhs).Value);
            
            // Check that expr.RHS operator is Neq
            Assert.AreEqual(Operator.Neq, ((NVBinaryExpression)expr.Rhs).Op);
            Assert.AreEqual(true, ((NVBoolExpression)((NVBinaryExpression)expr.Rhs).Lhs).Value);
            Assert.AreEqual(false, ((NVBoolExpression)((NVBinaryExpression)expr.Rhs).Rhs).Value);
        }

        [Test]
        public void ParseBinaryExpressionWithAddOp()
        {
            NVBinaryExpression expr = (NVBinaryExpression)GetExpressionFromTemplate(
                "1 + 2 - 3");

            // The AST should look like this:
            //     -
            //    / \
            //   +   3
            //  / \
            // 1   2

            // Check that there is a BinaryExpression with operator 'Minus'
            Assert.AreEqual(Operator.Minus, expr.Op);

            // Check the LHS of the expr
            Assert.AreEqual(Operator.Plus, ((NVBinaryExpression)expr.Lhs).Op);
            Assert.AreEqual(1, ((NVNumExpression)((NVBinaryExpression)expr.Lhs).Lhs).Value);
            Assert.AreEqual(2, ((NVNumExpression)((NVBinaryExpression)expr.Lhs).Rhs).Value);

            // Check the RHS of the expr
            Assert.AreEqual(3, ((NVNumExpression)expr.Rhs).Value);
        }

        [Test]
        public void ParseBinaryExpressionWithMulOp()
        {
            NVBinaryExpression expr = (NVBinaryExpression)GetExpressionFromTemplate(
                "1 * 2 / 3");

            // Check that there is a BinaryExpression with operator 'Div'
            Assert.AreEqual(Operator.Div, expr.Op);

            // Check the LHS
            Assert.AreEqual(Operator.Mul, ((NVBinaryExpression)expr.Lhs).Op);
            Assert.AreEqual(1, ((NVNumExpression)((NVBinaryExpression)expr.Lhs).Lhs).Value);
            Assert.AreEqual(2, ((NVNumExpression)((NVBinaryExpression)expr.Lhs).Rhs).Value);

            // Chec the RHS
            Assert.AreEqual(3, ((NVNumExpression)expr.Rhs).Value);
        }

        [Test]
        public void ParseUnaryExpression()
        {
            GetExpressionFromTemplate("+1 * -1");
            //TODO: Check UnaryExpression objects

            GetExpressionFromTemplate("!false and !!true");
            //TODO: Check UnaryExpression objects
        }

        [Test]
        public void ParseNVReference()
        {
            GetExpressionFromTemplate("$obj.Field");
            //TODO: Check DesignatorExpression

            GetExpressionFromTemplate("$obj.Method($obj2)");
            //TODO: Check DesignatorExpression
        }

        [Test]
        public void ParseExpressionWithParentheses()
        {
            NVBinaryExpression expr = (NVBinaryExpression)GetExpressionFromTemplate(
                "1 * (2 + 3)");

            // Check expr
            Assert.AreEqual(Operator.Mul, expr.Op);

            // Check LHS
            Assert.AreEqual(1, ((NVNumExpression)expr.Lhs).Value);
            
            // Check RHS
            NVBinaryExpression rhs = (NVBinaryExpression)expr.Rhs;
            Assert.AreEqual(Operator.Plus, rhs.Op);
            Assert.AreEqual(2, ((NVNumExpression)rhs.Lhs).Value);
            Assert.AreEqual(3, ((NVNumExpression)rhs.Rhs).Value);
        }

        [Test]
        public void ParseBooleanExpression()
        {
            NVBoolExpression exprTrue = (NVBoolExpression)GetExpressionFromTemplate("true");
            Assert.AreEqual(true, exprTrue.Value);

            NVBoolExpression exprFalse = (NVBoolExpression)GetExpressionFromTemplate("false");
            Assert.AreEqual(false, exprFalse.Value);
        }

        [Test]
        public void ParseNumExpression()
        {
            NVNumExpression expr = (NVNumExpression)GetExpressionFromTemplate("100");

            // Check that the value of the expression is '100'
            Assert.AreEqual(100, expr.Value);
        }

        [Test]
        public void ParseStringLiteralSingleQuotes()
        {
            NVStringExpression expr = (NVStringExpression)GetExpressionFromTemplate("'string'");

            // Check that the value of the expression is 'string'
            Assert.AreEqual("string", expr.Value);
        }

        [Test]
        public void ParseStringLiteralDoubleQuotes()
        {
            NVStringExpression expr = (NVStringExpression)GetExpressionFromTemplate("\"string\"");

            // Check that the value of the expression is 'string'
            Assert.AreEqual("string", expr.Value);
        }

        [Test]
        public void ParseDictionaryEmpty()
        {
            GetExpressionFromTemplate("\"%{}\"");
            //TODO: Check NVDictionary
        }

        [Test]
        public void ParseDictionaryWithOnePair()
        {
            GetExpressionFromTemplate("\"%{ key='value' }\"");
            //TODO: Check NVDictionary
        }

        [Test]
        public void ParseDictionaryWithTwoPairs()
        {
            GetExpressionFromTemplate("\"%{ key='value', anotherKey='anotherValue' }\"");
            //TODO: Check NVDictionary
        }

        [Test]
        public void ParseDictionaryWithReferenceAsValue()
        {
            GetExpressionFromTemplate("\"%{ key=$var.Field }\"");
            //TODO: Check NVDictionary
        }

        [Test]
        public void ParseRangeWithTwoConstants()
        {
            GetExpressionFromTemplate("[1..10]");
            //TODO
        }

        [Test]
        public void ParseRangeWithConstantAndReference()
        {
            GetExpressionFromTemplate("[1..$n]");
            //TODO
        }

        [Test]
        public void ParseRangeWithTwoReferences()
        {
            GetExpressionFromTemplate("[$i..$n]");
            //TODO
        }

        [Test]
        public void ParseArrayEmpty()
        {
            GetExpressionFromTemplate("[]");
            //TODO
        }

        [Test]
        public void ParseArrayWithSingleExpression()
        {
            GetExpressionFromTemplate("[10]");
            //TODO
        }

        [Test]
        public void ParseArrayWithTwoExpressions()
        {
            GetExpressionFromTemplate("[10, 20]");
        }

        [Test]
        public void ParseArrayWithVariousDifferentExpressions()
        {
            GetExpressionFromTemplate("[\"MonoRail\", $is, \"cool\"]");
            //TODO
        }
    }
}