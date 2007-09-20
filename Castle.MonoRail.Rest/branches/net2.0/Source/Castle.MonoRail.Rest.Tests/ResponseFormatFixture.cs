using MbUnit.Framework;
using Rhino.Mocks;
namespace Castle.MonoRail.Rest.Tests
{
    [TestFixture]
    public class ResponseFormatFixture
    {
        private MockRepository mocks;
        private IControllerBridge bridge;
        private ResponseFormatInternal format;
        private string handlerInvoked;

        [Test]
        public void IfRespondWithMimeAll_ThenFirstResponder_ShouldBeInvoked()
        {
            mocks = new MockRepository();
            bridge = mocks.DynamicMock<IControllerBridge>();

            using (mocks.Record())
            {
                SetupResult.For(bridge.ControllerAction).Return("Show");
            }
            format = new ResponseFormat();
        	format.AddRenderer("html", delegate(Responder responder) {
        	                           	handlerInvoked = "html";
        	                           });
        	format.AddRenderer("xml", delegate(Responder responder) {
        	                           	handlerInvoked = "xml";
        	                           });

            format.RespondWith("all", bridge);
            Assert.AreEqual("html", handlerInvoked);
        }

        
    }
}
