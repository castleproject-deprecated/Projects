using MbUnit.Framework;
using Castle.MonoRail.Rest.Mime;

namespace Castle.MonoRail.Rest.Tests.Mime
{
    [TestFixture]
    public class AcceptTypeParsingFixture
    {
        private MimeTypes TestMimes;
        [SetUp]
        public void Setup()
        {
            TestMimes = new MimeTypes();
            TestMimes.Register("text/plain", "text");
            TestMimes.Register("text/html", "html", new string[] {"application/xhtml+xml"});
			TestMimes.Register("application/xml", "xml", new string[] { "text/xml" });
            
        }

        private MimeType[] ParseHeader(string header)
        {
            return AcceptType.Parse(header, TestMimes);
        }

        [Test]
        public void TextHtml_ShouldBeSingleElementArrayOfHtmlMimeType()
        {
            string header = "text/html";
            MimeType[] types = ParseHeader(header);
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Length);
            Assert.AreEqual("html", types[0].Symbol);
            
        }

        [Test]
        public void ApplicationXml_ShouldBeSingleElementArrayOfXmlMimeType()
        {
            string header = "application/xml";
			MimeType[] types = ParseHeader(header);
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Length);
            Assert.AreEqual("xml", types[0].Symbol);
        }

        [Test]
        public void HtmlAndXmlMimeTypesTogether_ShouldBeTwoElementArray()
        {
            string header = "application/xml,text/html";
			MimeType[] types = ParseHeader(header);
            Assert.AreEqual(2, types.Length);
        }

        [Test]
        public void OrderingOfAcceptHeader_ShouldBePreserved()
        {
            string header = "application/xml,text/html";
            string header2 = "text/html,application/xml";

			MimeType[] types = ParseHeader(header);
            Assert.AreEqual("xml", types[0].Symbol);

			MimeType[] types2 = ParseHeader(header2);
            Assert.AreEqual("html", types2[0].Symbol);
        }

        [Test]
        public void CanHandleAcceptType_WithQValue()
        {
            string header = "text/html;q=0.5";
			MimeType[] types = ParseHeader(header);
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Length);
            Assert.AreEqual("html", types[0].Symbol);
        }

        [Test]
        public void AcceptTypes_WithQValues_ShouldBeOrderedAccordingToThatValue()
        {
            string header = "application/xml,text/plain;q=0.5,text/html";
			MimeType[] types = ParseHeader(header);

            Assert.AreEqual("xml", types[0].Symbol);
            Assert.AreEqual("html", types[1].Symbol);
            Assert.AreEqual("text", types[2].Symbol);            
        }

        [Test]
        public void IfAcceptTypes_ContainMimestringAndSynonym_OnlyOneMimeTypeShouldBeReturned()
        {
            string header = "application/xml,text/xml";
			MimeType[] types = ParseHeader(header);
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Length);
            Assert.AreEqual("xml", types[0].Symbol);
        }

        [Test]
        public void Synonyms_AreMappedTo_Symbol()
        {
            string header = "application/xhtml+xml";
			MimeType[] types = ParseHeader(header);
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Length);
            Assert.AreEqual("html", types[0].Symbol);
        }

        [Test]
        public void MoreSpecificXmlTypes_AreSortedAheadOf_ApplicationXml()
        {
            string header = "application/xml,application/xhtml+xml";
			MimeType[] types = ParseHeader(header);
            Assert.IsNotNull(types);
            Assert.AreEqual(2, types.Length);
            Assert.AreEqual("html", types[0].Symbol);
            Assert.AreEqual("xml", types[1].Symbol);
        }
    }
}
