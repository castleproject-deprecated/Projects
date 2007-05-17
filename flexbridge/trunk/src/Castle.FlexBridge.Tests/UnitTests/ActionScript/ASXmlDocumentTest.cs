// Copyright 2007 Castle Project - http://www.castleproject.org/
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

using System;
using System.Xml;
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Tests.UnitTests;
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests.UnitTests.ActionScript
{
    [TestFixture]
    [TestsOn(typeof(ASXmlDocument))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class ASXmlDocumentTest : BaseUnitTest
    {
        [Test]
        public void DefaultConstructor()
        {
            ASXmlDocument xml = new ASXmlDocument();

            Assert.AreEqual("", xml.XmlString);
            Assert.AreEqual("", xml.XmlDocument.OuterXml);
        }

        [Test]
        public void ConstructorWithString()
        {
            ASXmlDocument xml = new ASXmlDocument("<root />");

            Assert.AreEqual("<root />", xml.XmlString);
            Assert.AreEqual("<root />", xml.XmlDocument.OuterXml);
        }

        [Test]
        public void ConstructorWithXmlDocument()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root />");

            ASXmlDocument xml = new ASXmlDocument(doc);

            Assert.AreEqual(doc.OuterXml, xml.XmlString);
            Assert.AreEqual(doc, xml.XmlDocument);
        }

        [Test]
        public void GetterAndSetterInteractions()
        {
            ASXmlDocument xml = new ASXmlDocument("<root />");

            Assert.AreEqual("<root />", xml.XmlString);
            Assert.AreEqual("<root />", xml.XmlDocument.OuterXml); // black box impl. note: this clears the internal XmlString
            Assert.AreEqual("<root />", xml.XmlString);

            // set using XmlString
            xml.XmlString = "<trunk />";
            Assert.AreEqual("<trunk />", xml.XmlString);
            Assert.AreEqual("<trunk />", xml.XmlDocument.OuterXml);

            // modify under covers and note that value is reflected immediately because XmlString not
            // cached when an XmlDocument is present
            xml.XmlDocument.DocumentElement.SetAttribute("attr", "value");
            Assert.AreEqual("<trunk attr=\"value\" />", xml.XmlString);

            // set using XmlDocument
            XmlDocument doc = new XmlDocument();
            xml.XmlDocument = doc;
            Assert.AreEqual("", xml.XmlString);
            Assert.AreSame(doc, xml.XmlDocument);
        }
    }
}