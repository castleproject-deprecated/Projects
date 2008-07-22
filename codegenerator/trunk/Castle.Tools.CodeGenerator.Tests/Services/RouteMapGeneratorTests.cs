using System;
using Castle.Tools.CodeGenerator.Model.TreeNodes;
using Castle.Tools.CodeGenerator.Services.Generators;
using NUnit.Framework;
using Rhino.Mocks;

namespace Castle.Tools.CodeGenerator.Services
{
	public class RouteMapGeneratorTests
	{
		#region Member Data

		protected MockRepository mocks;
		protected ILogger logging;
		protected INamingService naming;
		protected ISourceGenerator source;
		protected RouteMapGenerator generator;

		#endregion

		#region Test Setup and Teardown Methods

		[SetUp]
		public virtual void Setup()
		{
			mocks = new MockRepository();
			naming = new DefaultNamingService();
			source = new DefaultSourceGenerator();
			logging = new NullLogger();
			generator = new RouteMapGenerator(logging, source, naming, "TargetNamespace", typeof (IServiceProvider).FullName);
		}

		#endregion
	}

	[TestFixture]
	public class RouteMapGeneratorControllerTests : RouteMapGeneratorTests
	{
		#region Test Methods

		[Test]
		public void VisitControllerNode_Always_CreatesType()
		{
			ControllerTreeNode node = new ControllerTreeNode("HomeController", "ControllerNamespace");

			mocks.ReplayAll();
			generator.Visit(node);
			mocks.VerifyAll();

			CodeDomAssert.AssertHasField(source.Ccu.Namespaces[0].Types[0], "_services");
		}

		#endregion
	}

	[TestFixture]
	public class RouteMapGeneratorActionTests : RouteMapGeneratorTests
	{
		#region Member Data

		private AreaTreeNode root;
		private ControllerTreeNode controller;

		#endregion

		#region Test Setup and Teardown Methods

		public override void Setup()
		{
			base.Setup();
			root = new AreaTreeNode("Root");
			controller = new ControllerTreeNode("HomeController", "ControllerNamespace");
			root.AddChild(controller);
		}

		#endregion

		#region Test Methods

		[Test]
		public void VisitRouteNode_NoParameters_CreatesMethod()
		{
			RouteTreeNode node = new StaticRouteTreeNode("Index", "index");
			ActionTreeNode actionTreeNode = new ActionTreeNode("action");
			actionTreeNode.AddChild(node);
			controller.AddChild(actionTreeNode);

			mocks.ReplayAll();
			generator.Visit(controller);
			mocks.VerifyAll();

			CodeDomAssert.AssertHasField(source.Ccu.Namespaces[0].Types[0], "_services");
			CodeDomAssert.AssertHasMethod(source.Ccu.Namespaces[0].Types[2], "Index");
		}

		[Test]
		public void VisitRouteNode_OneParameters_CreatesMethod()
		{
			RouteTreeNode node = new StaticRouteTreeNode("AuthenticateLogIn", "login/authenticate/<userName:string>/<password:string>");
			ActionTreeNode actionTreeNode = new ActionTreeNode("action");
			actionTreeNode.AddChild(node);
			controller.AddChild(actionTreeNode);
			node.AddChild(new ParameterTreeNode("userName", typeof(string)));

			using (mocks.Unordered())
			{
			}

			mocks.ReplayAll();
			generator.Visit(controller);
			mocks.VerifyAll();

			CodeDomAssert.AssertHasField(source.Ccu.Namespaces[0].Types[0], "_services");
			CodeDomAssert.AssertHasMethod(source.Ccu.Namespaces[0].Types[2], "AuthenticateLogIn");
		}

		#endregion
	}
}
