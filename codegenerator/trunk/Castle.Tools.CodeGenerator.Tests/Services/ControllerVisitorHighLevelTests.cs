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
	using System;
	using System.IO;
	using Model.TreeNodes;
	using Visitors;
	using ICSharpCode.NRefactory.Ast;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Rhino.Mocks.Constraints;

	[TestFixture]
	public class ControllerVisitorHighLevelTests
	{
		private MockRepository mocks;
		private ControllerVisitor visitor;
		private ITreeCreationService treeService;
		private ITypeResolver typeResolver;

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
			typeResolver = new TypeResolver();
			typeResolver = mocks.DynamicMock<ITypeResolver>();
			treeService = new DefaultTreeCreationService();
			visitor = new ControllerVisitor(new NullLogger(), typeResolver, treeService);
		}

		[Test]
		public void Parsing_MethodWithSystemTypeArrayParameter_YieldsCorrectType()
		{
			using (mocks.Unordered())
			{
				typeResolver.UseNamespace("SomeNamespace", true);
				typeResolver.UseNamespace("System");
				Expect.Call(typeResolver.Resolve(new TypeReference("DateTime")))
					.Constraints(Is.Matching((TypeReference reference) => reference.SystemType == "DateTime"))
					.Return("System.DateTime[]");
			}

			mocks.ReplayAll();
			Parse(MethodWithSystemTypeArrayParameter);
			mocks.VerifyAll();
		}

		[Test]
		public void Parsing_WizardController_()
		{
			using (mocks.Unordered())
			{
				typeResolver.UseNamespace("SomeNamespace", true);
				typeResolver.UseNamespace("System");
			}

			mocks.ReplayAll();
			Parse(WizardControllerType, true);
			mocks.VerifyAll();

			var node = treeService.Peek;

			Assert.AreEqual(1, node.Children.Count);

			var wizardControllerTreeNode = (WizardControllerTreeNode) node.Children[0];

			Assert.AreEqual(2, wizardControllerTreeNode.WizardStepPages.Length);
			Assert.AreEqual("FirstStep", wizardControllerTreeNode.WizardStepPages[0]);
			Assert.AreEqual("SecondStep", wizardControllerTreeNode.WizardStepPages[1]);
		}

		protected void Parse(string source)
		{
			Parse(source, false);
		}

		protected void Parse(string source, bool parseMethodBodies)
		{
			using (var reader = new StringReader(source))
			{
				var parser = new NRefactoryParserFactory().CreateCSharpParser(reader);
				parser.ParseMethodBodies = parseMethodBodies;
				parser.Parse();
				visitor.VisitCompilationUnit(parser.CompilationUnit, null);
			}
		}

		public static string MethodWithSystemTypeArrayParameter =
			@"
using System;
namespace SomeNamespace
{
  public partial class SomeController
  {
    public void SomeMethod(DateTime[] values) { }
  }
}
";

		public static string MethodWithSystemTypeListParameter =
			@"
using System;
using System.Collections.Generic;
namespace SomeNamespace
{
  public partial class SomeController
  {
    public void SomeMethod(List<DateTime> values) { }
  }
}
";

		public static string WizardControllerType =
			@"
using System;
namespace SomeNamespace
{
  public partial class SomeController : IWizardController
  {
		public void OnWizardStart()
		{
			// Some comments
			int i = 10;
			int j = i + 10;
		}

		public bool OnBeforeStep(string wizardName, string stepName, WizardStepPage step)
		{
			return true;
		}

		public void OnAfterStep(string wizardName, string stepName, WizardStepPage step)
		{
		}

		public WizardStepPage[] GetSteps(IRailsEngineContext context)
		{
			return new WizardStepPage[]
			{
				new FirstStep(),
				IoC.Resolve<SecondStep>()
			};
		}
  }
}
";
	}
}