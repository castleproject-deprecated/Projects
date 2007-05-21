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
    public class TokenPositionTestCase : ScannerTestBase
    {
        [Test]
        public void SingleLineTokenPositions()
        {
            scanner.SetSource(
                "<name/>");

            AssertMatchToken(TokenType.XmlTagStart, new Position(1, 1, 1, 2));
            AssertMatchToken(TokenType.XmlTagName, "name", new Position(1, 2, 1, 6));
            AssertMatchToken(TokenType.XmlForwardSlash, new Position(1, 6, 1, 7));
            AssertMatchToken(TokenType.XmlTagEnd, new Position(1, 7, 1, 8));

            AssertEOF();
        }

        [Test]
        public void MultilineTokenPositions()
        {
            scanner.SetSource(
                "<name\n" +
                ">");

            AssertMatchToken(TokenType.XmlTagStart, new Position(1, 1, 1, 2));
            AssertMatchToken(TokenType.XmlTagName, "name", new Position(1, 2, 1, 6));
            AssertMatchToken(TokenType.XmlTagEnd, new Position(2, 1, 2, 2));

            AssertEOF();
        }
    }
}