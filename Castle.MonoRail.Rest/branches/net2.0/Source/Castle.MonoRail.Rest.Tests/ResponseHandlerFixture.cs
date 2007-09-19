using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            ResponseFormatInternal iformat = (ResponseFormatInternal)format;
            iformat.AddRenderer("xml", x => handlerInvoked = "xml");
            iformat.AddRenderer("html", x => handlerInvoked = "html");

            mimes = new MimeType[] {
                new MimeType() { Symbol="html",MimeString="text/html"},
                new MimeType() { Symbol="xml",MimeString="application/xml"}
            };
        }

        [Test]
        public void IfFormatIsExplicitlyDefined_RespondserWithThatFormatIsInvoked()
        {
            ResponseHandler responder = new ResponseHandler() {
                ControllerBridge = bridge,
                Format = format
            };

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
            ResponseHandler responder = new ResponseHandler()
            {
                ControllerBridge = bridge,
                Format = format,
                AcceptedMimes = new MimeType[] {
                                    new MimeType() { Symbol="html",MimeString="text/html"}
                }
            };

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
            ResponseHandler responder = new ResponseHandler()
            {
                ControllerBridge = bridge,
                Format = format,
                AcceptedMimes = new MimeType[] {
                                    new MimeType() { Symbol="na",MimeString="notfound"},
                                    new MimeType() { Symbol="html",MimeString="text/html"},
                                    new MimeType() {Symbol="xml",MimeString="application/xml"}
                }
            };

            using (mocks.Record())
            {
                SetupResult.For(bridge.IsFormatDefined).Return(false);
            }

            responder.Respond();
            Assert.AreEqual("html", handlerInvoked);
        }

        
    }
}
