using System;
using Castle.Tools.CodeGenerator.Model.TreeNodes;
using Castle.Tools.CodeGenerator.Services.Generators;
using NUnit.Framework;
using Rhino.Mocks;

namespace Castle.Tools.CodeGenerator.Services
{
	[TestFixture]
	public class WizardStepMapGeneratorTests
	{
		#region Member Data
		protected MockRepository _mocks;
		protected ILogger _logging;
		protected INamingService _naming;
		protected ISourceGenerator _source;
		protected ActionMapGenerator _generator;
		#endregion

		#region Test Setup and Teardown Methods
		[SetUp]
		public virtual void Setup()
		{
			_mocks = new MockRepository();
			_naming = new DefaultNamingService();
			_source = new DefaultSourceGenerator();
			_logging = new NullLogger();
			_generator = new ActionMapGenerator(_logging, _source, _naming, "TargetNamespace", typeof(IServiceProvider).FullName);
		}
		#endregion

		#region Test Methods
		[Test]
		public void VisitControllerNode_Always_CreatesType()
		{
			WizardControllerTreeNode node = new WizardControllerTreeNode("HomeController", "ControllerNamespace", new string[] { "Step1" });

			_mocks.ReplayAll();
			_generator.Visit(node);
			_mocks.VerifyAll();

			CodeDomAssert.AssertHasField(_source.Ccu.Namespaces[0].Types[0], "_services");
		}
		#endregion
	}
}
