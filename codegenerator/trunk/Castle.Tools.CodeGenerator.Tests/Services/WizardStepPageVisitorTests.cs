// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Tools.CodeGenerator.Services
{
	using System.Collections.Generic;
	using Visitors;
	using ICSharpCode.NRefactory.Ast;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class WizardStepPageVisitorTests
	{
		private MockRepository mocks;
		private ITreeCreationService treeService;
		private ILogger logger;
		private ITypeResolver typeResolver;
		private WizardStepPageVisitor visitor;

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
			logger = new NullLogger();
			typeResolver = mocks.DynamicMock<ITypeResolver>();
			treeService = mocks.DynamicMock<ITreeCreationService>();
			visitor = new WizardStepPageVisitor(logger, typeResolver, treeService);
		}

		[Test]
		public void VisitTypeDeclaration_NotWizardStepPage_DoesNothing()
		{
			var type = new TypeDeclaration(Modifiers.Public, new List<AttributeSection>()) {Name = "SomeRandomType"};

			mocks.ReplayAll();
			visitor.VisitTypeDeclaration(type, null);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitTypeDeclaration_AWizardStepPageNotPartial_DoesNothing()
		{
			var type = new TypeDeclaration(Modifiers.Public, new List<AttributeSection>()) {Name = "SomeRandomWizardStepPage"};
			type.BaseTypes.Add(new TypeReference("WizardStepPage"));

			mocks.ReplayAll();
			visitor.VisitTypeDeclaration(type, null);
			mocks.VerifyAll();
		}
	}
}