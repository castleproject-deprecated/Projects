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

namespace Castle.NVelocity.Tests.ScannerTests
{
    using NUnit.Framework;

    [TestFixture]
    public class NVelocityDirectiveTestCase : ScannerTestBase
    {
        [Test]
        public void IfDirectiveInsideXmlText()
        {
            scanner.SetSource(
                "text #if text");

            AssertMatchToken(TokenType.XmlText, "text ");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.XmlText, " text");

            AssertEOF();
        }

        [Test]
        public void IfDirectiveWithBracesInsideXmlText()
        {
            scanner.SetSource(
                "text #{if} text");

            AssertMatchToken(TokenType.XmlText, "text ");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.XmlText, " text");

            AssertEOF();
        }

        [Test]
        public void IfDirectiveWithBracesWithXmlTextTouchingDirective()
        {
            scanner.SetSource(
                "text#{if}text");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.XmlText, "text");

            AssertEOF();
        }

        [Test]
        public void IfDirectiveWithOnlyLCurly()
        {
            scanner.SetSource(
                "text#{if");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.NVDirectiveHash);

            try
            {
                scanner.GetToken();
                Assert.Fail();
            }
            catch (ScannerError) {}
            
        }

        [Test]
        public void IfDirectiveFollowedByParens()
        {
            scanner.SetSource(
                "text#if()");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertEOF();
        }

        [Test]
        public void IfDirectiveWithBracesFollowedByParens()
        {
            scanner.SetSource(
                "text#{if}()");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertEOF();
        }

        [Test]
        public void IfDirectiveFollowedBySpaceAndParens()
        {
            scanner.SetSource(
                "text#if ()");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertEOF();
        }

        [Test]
        public void EndDirectiveFollowedByXmlText()
        {
            scanner.SetSource(
                "text#end text");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "end");
            AssertMatchToken(TokenType.XmlText, " text");

            AssertEOF();
        }

        [Test]
        public void IfDirectiveInsideXmlTag()
        {
            scanner.SetSource(
                "<div #if() />");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "div");

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void IfDirectiveInsideXmlAttributeValue()
        {
            scanner.SetSource(
                "<div class=\"#if() text #end\" />");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "div");
            AssertMatchToken(TokenType.XmlAttributeName, "class");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertMatchToken(TokenType.XmlAttributeText, " text ");

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "end");

            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }
    }
}