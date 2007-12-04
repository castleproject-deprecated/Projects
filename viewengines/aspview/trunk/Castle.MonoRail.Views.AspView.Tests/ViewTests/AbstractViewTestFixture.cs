using System;
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
		protected string expected;

		[SetUp]
		public void SetUp()
		{
			Clear();

			CreateStubsAndMocks();

			CreateDefaultStubsAndMocks();
		}

		/// <summary>
		/// Creating test specific stubs and mocks
		/// </summary>
		protected virtual void CreateStubsAndMocks()
		{
		}

		protected void AddCompilation(string key, Type viewType)
		{
			string className = AspViewEngine.GetClassName(key);
			((IAspViewEngineTestAccess)engine).Compilations.Add(className, viewType);
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
			context = context ?? new MockRailsEngineContext(request, response, trace, url);
			flash = flash ?? context.Flash;
			controller = controller ?? new StubController(propertyBag, flash, request, response);
		}

		/// <summary>
		/// Creating default stub and mocks
		/// </summary>
		protected virtual void Clear()
		{
			expected = null;
			writer = null;
			engine = null;
			cookies = null;
			request = null;
			response = null;
			url = null;
			trace = null;
			propertyBag = null;
			flash = null;
			controller = null;
			context = null;
		}

		protected void InitializeView(Type viewType)
		{
			view = (IViewBaseInternal)Activator.CreateInstance(viewType);
			InitializeView();
		}

		protected void InitializeView(IViewBaseInternal viewInstance)
		{
			viewInstance.Initialize(engine, writer, context, controller);
		}
		
		protected void InitializeView()
		{
			InitializeView(view);
		}

		protected void SetLayout(Type layoutType)
		{
			IViewBaseInternal layout = (IViewBaseInternal)Activator.CreateInstance(layoutType);
			InitializeView(layout);
			layout.ContentView = view;
			view = layout;
		}

		protected void AssertViewOutputEqualsToExpected()
		{
			Assert.AreEqual(expected, ViewOutput, "View output differ. Output was:\r\n" + ViewOutput);
		}

		protected virtual string ViewOutput
		{
			get { return writer.GetStringBuilder().ToString(); }
		}

	}
}
