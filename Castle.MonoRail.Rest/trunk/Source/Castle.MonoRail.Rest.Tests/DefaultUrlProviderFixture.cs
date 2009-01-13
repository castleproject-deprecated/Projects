using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Castle.MonoRail.Framework.Services;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Rest.Tests.Stubs;
using Rhino.Mocks;
namespace Castle.MonoRail.Rest.Tests
{
    [TestFixture]
    public class DefaultUrlProviderFixture
    {
        [Test]
        public void Should_AddTwoUrls_WhenControllerAddedToTree()
        {
            MockRepository mocks = new MockRepository();

            IUrlTokenizer tokenizer = mocks.DynamicMock<IUrlTokenizer>();
            IControllerTree controllerTree = new DefaultControllerTree();

            StubServiceProvider serviceProvider = new StubServiceProvider(tokenizer, controllerTree);

            DefaultUrlProvider provider = new DefaultUrlProvider();
            provider.Service(serviceProvider);

            using (mocks.Record())
            {
                tokenizer.AddDefaultRule("area/controller.rails", "area", "controller", "collection");
                tokenizer.AddDefaultRule("/area/controller.rails", "area","controller", "collection");
            }

            using (mocks.Playback())
            {
                controllerTree.AddController("area", "controller", typeof(SampleRestController));
            }
        }
    }

    public class StubServiceProvider : IServiceProvider
    {
        private IUrlTokenizer tokenizer;
        private IControllerTree controllerTree;

        public StubServiceProvider(IUrlTokenizer tokenizer, IControllerTree controllerTree)
        {
            this.tokenizer = tokenizer;
            this.controllerTree = controllerTree;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IUrlTokenizer))
            {
                return tokenizer;
            }
            else if (serviceType == typeof(IControllerTree))
            {
                return controllerTree;
            }
            return null;
        }
    }
}
