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
    public class LineScannerTestCase : ScannerTestBase
    {
        [Test]
        public void XmlTagThatIsOnMultipleLines()
        {
            scanner.IsLineScanner = true;
            
            scanner.SetSource(
                "<div");
            
            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "div");

            AssertEOF();

            scanner.RestoreState(scanner.RetrieveState());

            scanner.SetSource(
                "class=\"someClass\"/>");

            AssertMatchToken(TokenType.XmlAttributeName, "class");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "someClass");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }
        
        [Test]
        public void XmlAttributeValueThatIsOnMultipleLines()
        {
            scanner.IsLineScanner = true;

            scanner.SetSource(
                "<div class=\"start");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "div");
            AssertMatchToken(TokenType.XmlAttributeName, "class");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "start");

            AssertEOF();

            scanner.RestoreState(scanner.RetrieveState());

            scanner.SetSource(
                "end\"/>");

            AssertMatchToken(TokenType.XmlAttributeText, "end");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void NVMultilineCommentThatIsOnMultipleLines()
        {
            scanner.IsLineScanner = true;

            scanner.SetSource(
                "text#*comment");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.NVMultilineCommentStart);
            AssertMatchToken(TokenType.NVMultilineComment, "comment");
            
            AssertEOF();

            scanner.RestoreState(scanner.RetrieveState());

            scanner.SetSource(
                "morecomment*#moretext");

            AssertMatchToken(TokenType.NVMultilineComment, "morecomment");
            AssertMatchToken(TokenType.NVMultilineCommentEnd);
            AssertMatchToken(TokenType.XmlText, "moretext");

            AssertEOF();
        }

        [Test]
        public void XmlCommentThatIsOnMultipleLines()
        {
            scanner.IsLineScanner = true;

            scanner.SetSource(
                "text<!--comment");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.XmlCommentStart);
            AssertMatchToken(TokenType.XmlComment, "comment");

            AssertEOF();

            scanner.RestoreState(scanner.RetrieveState());

            scanner.SetSource(
                "morecomment-->moretext");

            AssertMatchToken(TokenType.XmlComment, "morecomment");
            AssertMatchToken(TokenType.XmlCommentEnd);
            AssertMatchToken(TokenType.XmlText, "moretext");

            AssertEOF();
        }

        [Test]
        public void XmlCommentThatIsOnMultipleLinesWithNoPrecedingXmlText()
        {
            scanner.IsLineScanner = true;

            scanner.SetSource(
                "<!--comment");

            AssertMatchToken(TokenType.XmlCommentStart, new Position(1, 1, 1, 5));
            AssertMatchToken(TokenType.XmlComment, "comment", new Position(1, 5, 1, 12));

            AssertEOF();

            scanner.RestoreState(scanner.RetrieveState());

            scanner.SetSource(
                "morecomment-->");

            AssertMatchToken(TokenType.XmlComment, "morecomment", new Position(1, 1, 1, 12));
            AssertMatchToken(TokenType.XmlCommentEnd, new Position(1, 12, 1, 15));

            AssertEOF();
        }
    }
}