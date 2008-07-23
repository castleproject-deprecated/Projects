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
	using Model.TreeNodes;
	using Visitors;
	using ICSharpCode.NRefactory.Ast;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Rhino.Mocks.Constraints;
	using Attribute = ICSharpCode.NRefactory.Ast.Attribute;

	[TestFixture]
	public class ControllerVisitorTests
	{
		private MockRepository mocks;
		private ITreeCreationService treeService;
		private ILogger logger;
		private ITypeResolver typeResolver;
		private ControllerVisitor visitor;

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
			logger = new NullLogger();
			typeResolver = mocks.DynamicMock<ITypeResolver>();
			treeService = mocks.DynamicMock<ITreeCreationService>();
			visitor = new ControllerVisitor(logger, typeResolver, treeService);
		}

		[Test]
		public void VisitTypeDeclaration_NotController_DoesNothing()
		{
			var type = new TypeDeclaration(Modifiers.Public, new List<AttributeSection>()) { Name = "SomeRandomType" };

			mocks.ReplayAll();
			visitor.VisitTypeDeclaration(type, null);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitTypeDeclaration_AControllerNotPartial_DoesNothing()
		{
			var type = new TypeDeclaration(Modifiers.Public, new List<AttributeSection>()) { Name = "SomeRandomController" };

			mocks.ReplayAll();
			visitor.VisitTypeDeclaration(type, null);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitTypeDeclaration_AControllerNoChildren_PushesAndPops()
		{
			var type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>()) { Name = "SomeRandomController" };

			using (mocks.Unordered())
			{
				treeService.PushNode(new ControllerTreeNode("SomeRandomController", "SomeNamespace"));
				treeService.PopNode();
			}

			mocks.ReplayAll();
			visitor.VisitTypeDeclaration(type, null);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitTypeDeclaration_AControllerNoChildrenWithArea_PushesAndPops()
		{
			var type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>()) { Name = "SomeRandomController" };
			type.Attributes.Add(CreateAreaAttributeCode("ControllerDetails", "Area", new PrimitiveExpression("AnArea", "AnArea")));

			using (mocks.Unordered())
			{
				Expect.Call(treeService.FindNode("AnArea")).Return(null);
				treeService.PushNode(new AreaTreeNode("AnArea"));
				treeService.PushNode(new ControllerTreeNode("SomeRandomController", "SomeNamespace"));
				treeService.PopNode();
				treeService.PopNode();
			}

			mocks.ReplayAll();
			visitor.VisitTypeDeclaration(type, null);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitTypeDeclaration_AControllerNoChildrenTrickyAreaValue_IgnoresAreaAttribute()
		{
			var type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>()) { Name = "SomeRandomController" };
			type.Attributes.Add(CreateAreaAttributeCode("ControllerDetails", "Area", new AddressOfExpression(new PrimitiveExpression("ThisNeverHappens", "Ok?"))));

			using (mocks.Unordered())
			{
				treeService.PushNode(new ControllerTreeNode("SomeRandomController", "SomeNamespace"));
				treeService.PopNode();
			}

			mocks.ReplayAll();
			visitor.VisitTypeDeclaration(type, null);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitTypeDeclaration_AControllerTrickyAttribute_IgnoresAttribute()
		{
			var type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>()) { Name = "SomeRandomController" };
			type.Attributes.Add(CreateAreaAttributeCode("NotControllerDetails", "NotArea", new PrimitiveExpression("NotAnArea", "NotAnArea")));

			using (mocks.Unordered())
			{
				treeService.PushNode(new ControllerTreeNode("SomeRandomController", "SomeNamespace"));
				treeService.PopNode();
			}

			mocks.ReplayAll();
			visitor.VisitTypeDeclaration(type, null);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitMethodDeclaration_ProtectedMember_DoesNothing()
		{
			var method = new MethodDeclaration("Action", Modifiers.Protected, null, new List<ParameterDeclarationExpression>(), new List<AttributeSection>());

			using (mocks.Unordered())
			{
			}

			mocks.ReplayAll();
			visitor.VisitMethodDeclaration(method, null);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitMethodDeclaration_ActionMemberNoArguments_CreatesEntryInNode()
		{
			var method = new MethodDeclaration("Action", Modifiers.Public, null, new List<ParameterDeclarationExpression>(), new List<AttributeSection>());
			var node = new ControllerTreeNode("SomeController", "SomeNamespace");

			using (mocks.Unordered())
				Expect.Call(treeService.Peek).Return(node);
			
			mocks.ReplayAll();
			visitor.VisitMethodDeclaration(method, null);
			mocks.VerifyAll();

			Assert.AreEqual("Action", node.Children[0].Name);
			Assert.AreEqual(0, node.Children[0].Children.Count);
		}

		[Test]
		public void VisitMethodDeclaration_ActionMemberNoArgumentsIsVirtual_CreatesEntryInNode()
		{
			var method = new MethodDeclaration("Action", Modifiers.Public | Modifiers.Virtual, null, new List<ParameterDeclarationExpression>(), new List<AttributeSection>());
			var node = new ControllerTreeNode("SomeController", "SomeNamespace");

			using (mocks.Unordered())
				Expect.Call(treeService.Peek).Return(node);
			
			mocks.ReplayAll();
			visitor.VisitMethodDeclaration(method, null);
			mocks.VerifyAll();

			Assert.AreEqual("Action", node.Children[0].Name);
			Assert.AreEqual(0, node.Children[0].Children.Count);
		}

		[Test]
		public void VisitMethodDeclaration_ActionMemberStandardArgument_CreatesEntryInNode()
		{
			var method = new MethodDeclaration("Action", Modifiers.Public, null, new List<ParameterDeclarationExpression>(), new List<AttributeSection>());
			method.Parameters.Add(new ParameterDeclarationExpression(new TypeReference("bool"), "parameter"));
			var node = new ControllerTreeNode("SomeController", "SomeNamespace");

			using (mocks.Unordered())
			{
				Expect.Call(treeService.Peek).Return(node);
				Expect.Call(typeResolver.Resolve(new TypeReference("bool"), true))
					.Constraints(
						Is.Matching((TypeReference reference) => reference.SystemType == "System.Boolean"),
						Is.Matching((bool throwOnFail) => throwOnFail))
					.Return(typeof (bool));
			}

			mocks.ReplayAll();
			visitor.VisitMethodDeclaration(method, null);
			mocks.VerifyAll();

			Assert.AreEqual("Action", node.Children[0].Name);
			Assert.AreEqual("parameter", node.Children[0].Children[0].Name);
		}

		private static AttributeSection CreateAreaAttributeCode(string attributeName, string argumentName,
		                                                        Expression valueExpression)
		{
			var argument = new NamedArgumentExpression(argumentName, valueExpression);
			
			var attribute = new Attribute(attributeName, new List<Expression>(), new List<NamedArgumentExpression>());
			attribute.NamedArguments.Add(argument);
			
			var attributeSection = new AttributeSection("IDontKnow", new List<Attribute>());
			attributeSection.Attributes.Add(attribute);
			
			return attributeSection;
		}
	}
}