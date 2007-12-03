using System.Collections;
using System.IO;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Test;
using Castle.MonoRail.Views.AspView.Tests.Stubs;
using NUnit.Framework;

namespace Castle.MonoRail.Views.AspView.Tests.ViewTests
{
	public abstract class AbstractViewTestFixture
	{
		protected IViewBaseInternal view;
		protected IDictionary propertyBag;
		protected Flash flash;
		protected StringWriter writer;
		protected AspViewEngine engine;
		protected IDictionary cookies;
		protected IRequest request;
		protected IResponse response;
		protected UrlInfo url;
		protected ITrace trace;
		protected IController controller;
		protected IRailsEngineContext context;

		[SetUp]
		public void SetUp()
		{
			CreateView();

			CreateStubsAndMocks();

			CreateDefaultStubsAndMocks();

			ViewInitialize();
		}

		protected virtual void CreateView()
		{
			view = new StubView();
		}

		/// <summary>
		/// Creating test specific stubs and mocks
		/// </summary>
		protected virtual void CreateStubsAndMocks()
		{
		}

		/// <summary>
		/// Creating default stub and mocks
		/// </summary>
		protected virtual void CreateDefaultStubsAndMocks()
		{
			writer = writer ?? new StringWriter();
			engine = engine ?? new AspViewEngine();
			cookies = cookies ?? new Hashtable();
			request = request ?? new MockRequest(cookies);
			response = response ?? new MockResponse(cookies);
			url = url ?? new UrlInfo("", "Stub", "Stub");
			trace = trace ?? new MockTrace();
			propertyBag = propertyBag ?? new Hashtable();
			flash = flash ?? new Flash();
			controller = controller ?? new StubController(propertyBag, flash, request, response);
			context = context ?? new MockRailsEngineContext(request, response, trace, url);
		}

		protected virtual void ViewInitialize()
		{
			view.Initialize(engine, writer, context, controller);
		}

		protected void AssertViewOutputEqualsTo(string expected)
		{
			Assert.AreEqual(expected, ViewOutput, "View output differ. Output was:\r\n" + ViewOutput);
		}

		protected virtual string ViewOutput
		{
			get { return writer.GetStringBuilder().ToString(); }
		}

	}
}
