using MbUnit.Framework;
using Rhino.Mocks;
using Castle.MonoRail.Rest.Mime;

namespace Castle.MonoRail.Rest.Tests
{
    [TestFixture]
    public class ResponseHandlerFixture
    {
        private IControllerBridge bridge;
        private MockRepository mocks;
        private ResponseFormat format;
        private string handlerInvoked;
        private MimeType[] mimes;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            bridge = mocks.DynamicMock<IControllerBridge>();
            format = new ResponseFormat();
            handlerInvoked = "";

            ResponseFormatInternal iformat = format;

			iformat.AddRenderer("xml", delegate(Responder responder)
			{
				handlerInvoked = "xml";
			});
			iformat.AddRenderer("html", delegate(Responder responder)
			{
        	                           	handlerInvoked = "html";
        	                           });

            mimes = new MimeType[] {
                new MimeType("text/html", "html"),
                new MimeType("application/xml", "xml")
            };
        }

        [Test]
        public void IfFormatIsExplicitlyDefined_RespondserWithThatFormatIsInvoked()
        {
            ResponseHandler responder = new ResponseHandler(bridge, format);

            using (mocks.Record())
            {
                SetupResult.For(bridge.IsFormatDefined).Return(true);
                SetupResult.For(bridge.GetFormat()).Return("html");
            }

            responder.Respond();
            Assert.AreEqual("html", handlerInvoked);
        }

        [Test]
        public void IfFormatIsNotExplicitlyDefined_ResponderIsDeterminedFromAcceptHeaders()
        {
			ResponseHandler responder = new ResponseHandler(bridge, format, new MimeType("text/html", "html"));

            using (mocks.Record())
            {
                SetupResult.For(bridge.IsFormatDefined).Return(false);
            }

            responder.Respond();
            Assert.AreEqual("html", handlerInvoked);
        }

        [Test]
        public void IfFormat_IsDeterminedByMimeType_FirstAvailableMimeTypeIsRendered()
        {
        	ResponseHandler responder = new ResponseHandler(bridge,
        	                                                format,
											               	new MimeType("notfound","na"),
											               	new MimeType("text/html", "html"),
											               	new MimeType("application/xml", "xml")
															 );

            using (mocks.Record())
            {
                SetupResult.For(bridge.IsFormatDefined).Return(false);
            }

            responder.Respond();
            Assert.AreEqual("html", handlerInvoked);
        }

        
    }
}
