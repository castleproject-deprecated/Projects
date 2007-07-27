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
    public class NVelocityTokenTestCase : ScannerTestBase
    {
        [Test]
        public void DollarAndIdentifierInDirectiveParams()
        {
            scanner.SetSource(
                "#if($varName)");

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "varName");
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertEOF();
        }

        [Test]
        public void TwoReferencesInDirectiveParams()
        {
            scanner.SetSource(
                "#if($varName1 $varName2)");

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "varName1");
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "varName2");
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertEOF();
        }

        [Test]
        public void OpeningAndClosingBrackets()
        {
            scanner.SetSource(
                "#if( [ ] )");

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);

            AssertMatchToken(TokenType.NVLBrack);
            AssertMatchToken(TokenType.NVRBrack);

            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertEOF();
        }

        [Test]
        public void ReferenceInsideXmlTag()
        {
            scanner.SetSource(
                "<div $var.Call() />");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "div");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "var");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "Call");
            AssertMatchToken(TokenType.NVLParen);
            AssertMatchToken(TokenType.NVRParen);

            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void ReferenceInsideXmlAttributeValue()
        {
            scanner.SetSource(
                "<div class=\"$var.Call()\" />");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "div");
            AssertMatchToken(TokenType.XmlAttributeName, "class");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "var");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "Call");
            AssertMatchToken(TokenType.NVLParen);
            AssertMatchToken(TokenType.NVRParen);

            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void ReferenceWithSelectorFollowedByText()
        {
            scanner.SetSource(
                "$var.Method()text");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "var");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "Method");
            AssertMatchToken(TokenType.NVLParen);
            AssertMatchToken(TokenType.NVRParen);
            
            AssertMatchToken(TokenType.XmlText, "text");

            AssertEOF();
        }

        [Test]
        public void NVStringLiteralWithDollarAmountShouldBeASingleToken()
        {
            scanner.SetSource(
                "$a.B(\"$100\")");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "a");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "B");
            AssertMatchToken(TokenType.NVLParen);
            AssertMatchToken(TokenType.NVDoubleQuote);
            AssertMatchToken(TokenType.NVStringLiteral, "$100");
            AssertMatchToken(TokenType.NVDoubleQuote);
            AssertMatchToken(TokenType.NVRParen);
        }

        [Test]
        public void NVStringLiteralWithDirectiveShouldBeScanned()
        {
            scanner.SetSource(
                "$a.B(\"#if(true)$a#end\")");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "a");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "B");
            AssertMatchToken(TokenType.NVLParen);
            AssertMatchToken(TokenType.NVDoubleQuote);
            
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVTrue);
            AssertMatchToken(TokenType.NVDirectiveRParen);
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "a");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "end");

            AssertMatchToken(TokenType.NVDoubleQuote);
            AssertMatchToken(TokenType.NVRParen);
        }
    }
}