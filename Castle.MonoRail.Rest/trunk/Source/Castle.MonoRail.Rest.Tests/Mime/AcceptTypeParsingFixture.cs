namespace Castle.MonoRail.Rest.Tests.Mime
{
	using System.Globalization;
	using System.Threading;
	using MbUnit.Framework;
	using Castle.MonoRail.Rest.Mime;

	[TestFixture]
    public class AcceptTypeParsingFixture
    {
        private MimeTypes TestMimes;

		[SetUp]
        public void Setup()
        {
            TestMimes = new MimeTypes();
            TestMimes.Register("text/plain", "text");
            TestMimes.Register("text/html", "html", new[] {"application/xhtml+xml"});
            TestMimes.Register("application/xml", "xml",new[] {"text/xml"});
        	Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        private MimeType[] ParseHeader(string header)
        {
            return AcceptType.Parse(header, TestMimes);
        }

        [Test]
        public void TextHtml_ShouldBeSingleElementArrayOfHtmlMimeType()
        {
            string header = "text/html";
            var types = ParseHeader(header);
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Length);
            Assert.AreEqual("html", types[0].Symbol);
            
        }

        [Test]
        public void ApplicationXml_ShouldBeSingleElementArrayOfXmlMimeType()
        {
            string header = "application/xml";
            var types = ParseHeader(header);
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Length);
            Assert.AreEqual("xml", types[0].Symbol);
        }

        [Test]
        public void HtmlAndXmlMimeTypesTogether_ShouldBeTwoElementArray()
        {
            string header = "application/xml,text/html";
            var types = ParseHeader(header);
            Assert.AreEqual(2, types.Length);
        }

        [Test]
        public void OrderingOfAcceptHeader_ShouldBePreserved()
        {
            string header = "application/xml,text/html";
            string header2 = "text/html,application/xml";

            var types = ParseHeader(header);
            Assert.AreEqual("xml", types[0].Symbol);

            var types2 = ParseHeader(header2);
            Assert.AreEqual("html", types2[0].Symbol);
        }

        [Test]
        public void CanHandleAcceptType_WithQValue()
        {
            var header = "text/html;q=0.5";
            var types = ParseHeader(header);
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Length);
            Assert.AreEqual("html", types[0].Symbol);
        }

        [Test]
        public void AcceptTypes_WithQValues_ShouldBeOrderedAccordingToThatValue()
        {
            var header = "application/xml,text/plain;q=0.5,text/html";
            var types = ParseHeader(header);

            Assert.AreEqual("xml", types[0].Symbol);
            Assert.AreEqual("html", types[1].Symbol);
            Assert.AreEqual("text", types[2].Symbol);            
        }

        [Test]
        public void IfAcceptTypes_ContainMimestringAndSynonym_OnlyOneMimeTypeShouldBeReturned()
        {
            var header = "application/xml,text/xml";
            var types = ParseHeader(header);
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Length);
            Assert.AreEqual("xml", types[0].Symbol);
        }

        [Test]
        public void Synonyms_AreMappedTo_Symbol()
        {
            var header = "application/xhtml+xml";
            var types = ParseHeader(header);
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Length);
            Assert.AreEqual("html", types[0].Symbol);
        }

        [Test]
        public void MoreSpecificXmlTypes_AreSortedAheadOf_ApplicationXml()
        {
            var header = "application/xml,application/xhtml+xml";
            var types = ParseHeader(header);
            Assert.IsNotNull(types);
            Assert.AreEqual(2, types.Length);
            Assert.AreEqual("html", types[0].Symbol);
            Assert.AreEqual("xml", types[1].Symbol);
        }
    }
}
