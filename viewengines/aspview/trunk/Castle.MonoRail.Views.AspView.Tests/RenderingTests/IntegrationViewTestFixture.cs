using Castle.Core.Logging;
using Castle.MonoRail.Views.AspView.Compiler;

namespace Castle.MonoRail.Views.AspView.Tests.RenderingTests
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using Framework.Descriptors;
	using Framework.Helpers;
	using Framework.Resources;
	using Framework.Services;
	using Framework.Test;
	using Framework;
	using Xunit;

	public class IntegrationViewTestFixture
	{
		private readonly string siteRoot;
		protected ControllerContext ControllerContext;
		protected HelperDictionary Helpers;
		private string lastOutput;
		protected string Layout;
		protected StubEngineContext StubEngineContext;
		protected Hashtable PropertyBag;
		protected string Area = null;
		protected string ControllerName = "test_controller";
		protected string Action = "test_action";
		protected DefaultViewComponentFactory ViewComponentFactory;
		protected AspViewEngine viewEngine;

		public IntegrationViewTestFixture()
			: this("..\\..\\RenderingTests")
		{
		}

		public IntegrationViewTestFixture(string siteRoot)
		{
			this.siteRoot = siteRoot;
			SetUp();
		}


		public string SiteRoot
		{
			get { return siteRoot; }
		}

		public void SetUp()
		{
			string viewPath = Path.Combine(SiteRoot, "Views");
			Layout = null;
			PropertyBag = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			Helpers = new HelperDictionary();
			StubMonoRailServices services = new StubMonoRailServices();
			services.UrlBuilder = new DefaultUrlBuilder(new StubServerUtility(), new StubRoutingEngine());
			services.UrlTokenizer = new DefaultUrlTokenizer();
			UrlInfo urlInfo = new UrlInfo(
				"example.org", "test", "/TestBrail", "http", 80,
				"http://test.example.org/test_area/test_controller/test_action.tdd",
				Area, ControllerName, Action, "tdd", "no.idea");
			StubEngineContext = new StubEngineContext(new StubRequest(), new StubResponse(), services,
													  urlInfo);
			StubEngineContext.AddService<IUrlBuilder>(services.UrlBuilder);
			StubEngineContext.AddService<IUrlTokenizer>(services.UrlTokenizer);
			StubEngineContext.AddService<IViewComponentFactory>(ViewComponentFactory);
			StubEngineContext.AddService<ILoggerFactory>(new ConsoleFactory());
			StubEngineContext.AddService<IViewSourceLoader>(new FileAssemblyViewSourceLoader(viewPath));
			

			ViewComponentFactory = new DefaultViewComponentFactory();
			ViewComponentFactory.Service(StubEngineContext);
			ViewComponentFactory.Initialize();

			ControllerContext = new ControllerContext();
			ControllerContext.Helpers = Helpers;
			ControllerContext.PropertyBag = PropertyBag;
			StubEngineContext.CurrentControllerContext = ControllerContext;


			Helpers["urlhelper"] = Helpers["url"] = new UrlHelper(StubEngineContext);
			Helpers["htmlhelper"] = Helpers["html"] = new HtmlHelper(StubEngineContext);
			Helpers["dicthelper"] = Helpers["dict"] = new DictHelper(StubEngineContext);
			Helpers["DateFormatHelper"] = Helpers["DateFormat"] = new DateFormatHelper(StubEngineContext);


			//FileAssemblyViewSourceLoader loader = new FileAssemblyViewSourceLoader(viewPath);
//			loader.AddAssemblySource(
//				new AssemblySourceInfo(Assembly.GetExecutingAssembly().FullName,
//									   "Castle.MonoRail.Views.Brail.Tests.ResourcedViews"));

			viewEngine = new AspViewEngine();
			viewEngine.Service(StubEngineContext);
			AspViewEngineOptions options = new AspViewEngineOptions();
			options.CompilerOptions.AutoRecompilation = true;
			options.CompilerOptions.KeepTemporarySourceFiles = false;
			string root = GetSiteRoot();
			ICompilationContext context = 
				new CompilationContext(
					new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory),
					new DirectoryInfo(root),
					new DirectoryInfo(root + @"\RenderingTests\Views"),
					new DirectoryInfo(root));
			viewEngine.Initialize(context, options);
			System.Console.WriteLine("init");

			BeforEachTest();
		}

		protected virtual void BeforEachTest()
		{
			System.Console.WriteLine("BeforEachTest");
		}

		public string ProcessView(string templatePath)
		{
			StringWriter sw = new StringWriter();
			if (string.IsNullOrEmpty(Layout) == false)
				ControllerContext.LayoutNames = new string[] { Layout };
			StubEngineContext.CurrentControllerContext = ControllerContext;
			viewEngine.Process(templatePath, sw, StubEngineContext, null, ControllerContext);
			lastOutput = sw.ToString();
			return lastOutput;
		}

		protected virtual string GetSiteRoot()
		{
			DirectoryInfo current = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
			while (current != null && current.Name != "Castle.MonoRail.Views.AspView.Tests")
			{
				current = current.Parent;
			}
			if (current == null) throw new Exception("Cannot resolve site root");
			return current.FullName;
		}

		protected void AddResource(string name, string resourceName, Assembly asm)
		{
			IResourceFactory resourceFactory = new DefaultResourceFactory();
			ResourceDescriptor descriptor = new ResourceDescriptor(
				null,
				name,
				resourceName,
				null,
				null);
			IResource resource = resourceFactory.Create(
				descriptor,
				asm);
			ControllerContext.Resources.Add(name, resource);
		}

		/*
		protected string RenderStaticWithLayout(string staticText)
		{
			if (string.IsNullOrEmpty(Layout) == false)
				ControllerContext.LayoutNames = new string[] { Layout, };
			StubEngineContext.CurrentControllerContext = ControllerContext;

			BooViewEngine.RenderStaticWithinLayout(staticText, StubEngineContext, null, ControllerContext);
			lastOutput = ((StringWriter)StubEngineContext.Response.Output)
				.GetStringBuilder().ToString();
			return lastOutput;
		}
		*/
		public void AssertReplyEqualTo(string expected)
		{
			Assert.Equal(expected, lastOutput);
		}

		public void AssertReplyContains(string contained)
		{
			Assert.Contains(contained, lastOutput);
		}
	}
}
