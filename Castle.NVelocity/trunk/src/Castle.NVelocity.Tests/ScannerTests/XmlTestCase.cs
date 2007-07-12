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
    public class XmlTestCase : ScannerTestBase
    {
        [Test]
        public void EmptySource()
        {
            scanner.SetSource(
                "");

            AssertEOF();
        }

        [Test]
        public void SingleWellFormedTag()
        {
            scanner.SetSource(
                "<name>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void SingleWellFormedSelfClosingTag()
        {
            scanner.SetSource(
                "<name/>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void WellFormedStartAndEndTags()
        {
            scanner.SetSource(
                "<name></name>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void WellFormedStartAndEndTagsWithSimpleContent()
        {
            scanner.SetSource(
                "<name>text with spaces</name>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertMatchToken(TokenType.XmlText, "text with spaces");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void WellFormedTagContainingNewLine()
        {
            scanner.SetSource(
                "<name\n" +
                ">");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void XmlDeclaration()
        {
            scanner.SetSource(
                "<?xml version=\"1.0\" ?>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlQuestionMark);
            AssertMatchToken(TokenType.XmlTagName, "xml");
            AssertMatchToken(TokenType.XmlAttributeName, "version");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "1.0");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlQuestionMark);
            AssertMatchToken(TokenType.XmlTagEnd);
            AssertEOF();
        }

        [Test]
        public void DocTypeDeclaration()
        {
            scanner.SetSource(
                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" " +
                "\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlExclaimationMark);
            AssertMatchToken(TokenType.XmlTagName, "DOCTYPE");
            AssertMatchToken(TokenType.XmlAttributeName, "html");
            AssertMatchToken(TokenType.XmlAttributeName, "PUBLIC");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "-//W3C//DTD XHTML 1.0 Transitional//EN");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void ProcessingInstruction()
        {
            scanner.SetSource(
                "<?xml-stylesheet href=\"style.css\" type=\"text/css\"?>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlQuestionMark);
            AssertMatchToken(TokenType.XmlTagName, "xml-stylesheet");
            AssertMatchToken(TokenType.XmlAttributeName, "href");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "style.css");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeName, "type");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "text/css");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlQuestionMark);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void WellFormedOpenAndCloseComment()
        {
            scanner.SetSource(
                "before<!-- inside -->after");

            AssertMatchToken(TokenType.XmlText, "before");
            AssertMatchToken(TokenType.XmlComment, "<!-- inside -->");
            AssertMatchToken(TokenType.XmlText, "after");

            AssertEOF();
        }

        [Test]
        public void FirstThreeCharactersOfCommentIsAnElement()
        {
            scanner.SetSource(
                "<!- notAComment");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlExclaimationMark);
            
            // Throws a ScannerError because a '-' is not allowed in a tag
            try
            {
                scanner.GetToken();
                Assert.Fail(); // Should not get to this point
            }
            catch (ScannerError) { }
        }

        [Test]
        public void TagTokenCharsScannedAsXmlTextInDefaultState()
        {
            scanner.SetSource(
                "> / \" ? !");

            AssertMatchToken(TokenType.XmlText, "> / \" ? !");

            AssertEOF();
        }

        [Test]
        public void CDataSection()
        {
            scanner.SetSource(
                "<![CDATA[ text ]]>");

            AssertMatchToken(TokenType.XmlCDataStart, "<![CDATA[");
            AssertMatchToken(TokenType.XmlText, " text ");
            AssertMatchToken(TokenType.XmlCDataEnd, "]]>");
        }

        [Test]
        public void CDataSectionContainingTag()
        {
            scanner.SetSource(
                "<![CDATA[ <tag> ]]>");

            AssertMatchToken(TokenType.XmlCDataStart, "<![CDATA[");
            AssertMatchToken(TokenType.XmlText, " <tag> ");
            AssertMatchToken(TokenType.XmlCDataEnd, "]]>");
        }

        [Test]
        public void IgnoreElementsInScriptElement()
        {
            scanner.SetSource(
                "<script>text<strong>text</script>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "script");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertMatchToken(TokenType.XmlText, "text<strong>text");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagName, "script");
            AssertMatchToken(TokenType.XmlTagEnd);
        }

        [Test]
        public void IgnoreElementsInScriptElementInAnotherElement()
        {
            scanner.SetSource(
                "<div><script>text<strong>text</script></div>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "div");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "script");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertMatchToken(TokenType.XmlText, "text<strong>text");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagName, "script");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagName, "div");
            AssertMatchToken(TokenType.XmlTagEnd);
        }

        [Test]
        public void HashNotFollowedByTextIsXmlText()
        {
            scanner.SetSource(
                "<td>#</td>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "td");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertMatchToken(TokenType.XmlText, "#");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagName, "td");
            AssertMatchToken(TokenType.XmlTagEnd);
        }

        [Test]
        public void HashPrecededByTextNotFollowedByTextIsXmlText()
        {
            scanner.SetSource(
                "<td>text#</td>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "td");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertMatchToken(TokenType.XmlText, "text#");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagName, "td");
            AssertMatchToken(TokenType.XmlTagEnd);
        }

        [Test]
        public void HashAtBeginningOfXmlTagAttributeValue()
        {
            scanner.SetSource(
                "<a title=\"#\"/>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "a");
            AssertMatchToken(TokenType.XmlAttributeName, "title");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "#");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);
        }

        [Test]
        public void HashWithinXmlTagAttributeValue()
        {
            scanner.SetSource(
                "<a title=\"You are #1.\"/>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "a");
            AssertMatchToken(TokenType.XmlAttributeName, "title");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "You are #1.");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);
        }

        [Test]
        public void DollarSignThatIsNotNVReference()
        {
            scanner.SetSource(
                "$");

            AssertMatchToken(TokenType.XmlText, "$");
        }

        [Test]
        public void DollarSignWithinXmlTextThatIsNotNVReference()
        {
            scanner.SetSource(
                "I have $100.");

            AssertMatchToken(TokenType.XmlText, "I have $100.");
        }

        [Test]
        public void DollarSignWithinXmlTagAttributeValueThatIsNotNVReference()
        {
            scanner.SetSource(
                "<a title=\"I have $100.\"/>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "a");
            AssertMatchToken(TokenType.XmlAttributeName, "title");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "I have $100.");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);
        }
    }
}