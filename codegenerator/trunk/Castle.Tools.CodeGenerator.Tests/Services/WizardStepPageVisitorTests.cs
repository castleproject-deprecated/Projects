using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;
using Rhino.Mocks;

namespace Castle.Tools.CodeGenerator.Services
{
	[TestFixture]
	public class WizardStepPageVisitorTests
	{
		#region Member Data
		private MockRepository _mocks;
		private ITreeCreationService _treeService;
		private ILogger _logger;
		private ITypeResolver _typeResolver;
		private WizardStepPageVisitor _visitor;
		#endregion

		#region Test Setup and Teardown Methods
		[SetUp]
		public void Setup()
		{
			_mocks = new MockRepository();
			_logger = new NullLogger();
			_typeResolver = _mocks.CreateMock<ITypeResolver>();
			_treeService = _mocks.CreateMock<ITreeCreationService>();
			_visitor = new WizardStepPageVisitor(_logger, _typeResolver, _treeService);
		}
		#endregion

		#region Test Methods
		[Test]
		public void VisitTypeDeclaration_NotWizardStepPage_DoesNothing()
		{
			TypeDeclaration type = new TypeDeclaration(Modifiers.Public, new List<AttributeSection>());
			type.Name = "SomeRandomType";

			_mocks.ReplayAll();
			_visitor.VisitTypeDeclaration(type, null);
			_mocks.VerifyAll();
		}

		[Test]
		public void VisitTypeDeclaration_AWizardStepPageNotPartial_DoesNothing()
		{
			TypeDeclaration type = new TypeDeclaration(Modifiers.Public, new List<AttributeSection>());
			type.Name = "SomeRandomWizardStepPage";
			type.BaseTypes.Add(new TypeReference("WizardStepPage"));

			_mocks.ReplayAll();
			_visitor.VisitTypeDeclaration(type, null);
			_mocks.VerifyAll();
		}
		#endregion
	}
}
