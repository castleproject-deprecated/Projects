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
	using Attribute = ICSharpCode.NRefactory.Ast.Attribute;
	using MonoRail.Framework;
	using MonoRail.Rest;

	[TestFixture]
	public class ControllerVisitorTests
	{
		private const string ControllerArea = "AnArea";
		private const string ControllerName = "SomeRandomController";
		private const string ControllerNamespace = "SomeNamespace";

		private ITreeCreationService treeService;
		private ILogger logger;
		private ITypeResolver typeResolver;
		private ControllerVisitor visitor;

		[SetUp]
		public void Setup()
		{
			logger = new NullLogger();
			typeResolver = MockRepository.GenerateMock<ITypeResolver>();
			treeService = MockRepository.GenerateMock<ITreeCreationService>();
			visitor = new ControllerVisitor(logger, typeResolver, treeService);
		}

		[Test]
		public void VisitTypeDeclaration_NotController_DoesNothing()
		{
			var type = new TypeDeclaration(Modifiers.Public, new List<AttributeSection>()) { Name = "SomeRandomType" };

			visitor.VisitTypeDeclaration(type, null);
		}

		[Test]
		public void VisitTypeDeclaration_AControllerNotPartial_DoesNothing()
		{
			var type = new TypeDeclaration(Modifiers.Public, new List<AttributeSection>()) { Name = ControllerName };

			visitor.VisitTypeDeclaration(type, null);
		}

		[Test]
		public void VisitTypeDeclaration_AControllerNoChildren_PushesAndPops()
		{
			var type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>()) { Name = ControllerName };

			visitor.VisitTypeDeclaration(type, null);

			treeService.AssertWasCalled(s => s.PushNode(Arg<ControllerTreeNode>.Matches(n => n.Name == ControllerName)));
			treeService.AssertWasCalled(s => s.PopNode());
		}

		[Test]
		public void VisitTypeDeclaration_AControllerNoChildrenWithArea_PushesAndPops()
		{
			
			var type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>()) { Name = ControllerName };
			type.Attributes.Add(CreateControllerAttributeCode("ControllerDetails", "Area", new PrimitiveExpression(ControllerArea, ControllerArea)));

			treeService.Expect(s => s.FindNode(ControllerArea)).Return(null);

			visitor.VisitTypeDeclaration(type, null);

			treeService.AssertWasCalled(s => s.PushNode(Arg<TreeNode>.Matches(n => n is AreaTreeNode && n.Name == ControllerArea)));
			treeService.AssertWasCalled(s => s.PushNode(Arg<TreeNode>.Matches(n => n is ControllerTreeNode && n.Name == ControllerName)));
			treeService.AssertWasCalled(s => s.PopNode(), o => o.Repeat.Twice());
		}

		[Test]
		public void VisitTypeDeclaration_AControllerNoChildrenTrickyAreaValue_IgnoresAreaAttribute()
		{
			var type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>()) { Name = ControllerName };
			type.Attributes.Add(CreateControllerAttributeCode("ControllerDetails", "Area", new AddressOfExpression(new PrimitiveExpression("ThisNeverHappens", "Ok?"))));

			visitor.VisitTypeDeclaration(type, null);

			treeService.AssertWasCalled(s => s.PushNode(Arg<ControllerTreeNode>.Matches(n => n.Name == ControllerName)));
			treeService.AssertWasCalled(s => s.PopNode());
		}

		[Test]
		public void VisitTypeDeclaration_AControllerTrickyAttribute_IgnoresAttribute()
		{
			var type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>()) { Name = ControllerName };
			type.Attributes.Add(CreateControllerAttributeCode("NotControllerDetails", "NotArea", new PrimitiveExpression("NotAnArea", "NotAnArea")));

			visitor.VisitTypeDeclaration(type, null);

			treeService.AssertWasCalled(s => s.PushNode(Arg<ControllerTreeNode>.Matches(n => n.Name == ControllerName)));
			treeService.AssertWasCalled(s => s.PopNode());
		}

		[Test]
		public void VisitTypeDeclaration_AControllerWithRestRoutesAttribute_ExtractsRestRoutesData()
		{
			var routeName = "restController";
			var routeCollection = "/part1/<key>/part2";
			var routeIdentifier = "<id>";
			var routeRestVerbResolverType = typeof(CustomRestVerbResolver).FullName;

			var type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>()) { Name = ControllerName };
			var attribute = new Attribute("RestRoutes", new List<Expression>(), new List<NamedArgumentExpression>());
			attribute.PositionalArguments.AddRange(new Expression[]
			{
				new PrimitiveExpression("restController", "restController"),
				new PrimitiveExpression("/part1/<key>/part2", "/part1/<key>/part2"),
				new PrimitiveExpression("<id>", "<id>"),
				new TypeOfExpression(new TypeReference(routeRestVerbResolverType))
			});

			var section = new AttributeSection();
			section.Attributes.Add(attribute);
			type.Attributes.Add(section);

			typeResolver.Expect(r => r.Resolve(Arg<TypeReference>.Matches(t => t.Type == routeRestVerbResolverType))).Return(
				routeRestVerbResolverType);

			visitor.VisitTypeDeclaration(type, null);

			treeService.AssertWasCalled(
				s => s.PushNode(Arg<ControllerTreeNode>.Matches(
					n => n.Name == ControllerName && 
						 n.RestRoutesDescriptor.Name == routeName &&
						 n.RestRoutesDescriptor.Collection == routeCollection &&
						 n.RestRoutesDescriptor.Identifier == routeIdentifier &&
						 n.RestRoutesDescriptor.RestVerbResolverType == routeRestVerbResolverType)));
			treeService.AssertWasCalled(s => s.PopNode());
		}

		[Test]
		public void VisitMethodDeclaration_ProtectedMember_DoesNothing()
		{
			var method = new MethodDeclaration {Name = "Action", Modifier = Modifiers.Protected};

			visitor.VisitMethodDeclaration(method, null);
		}

		[Test]
		public void VisitMethodDeclaration_ActionMemberNoArguments_CreatesEntryInNode()
		{
			var method = new MethodDeclaration { Name = "Action", Modifier = Modifiers.Public };
			var node = new ControllerTreeNode(ControllerName, ControllerNamespace);

			treeService.Expect(s => s.Peek).Return(node);

			visitor.VisitMethodDeclaration(method, null);
			
			Assert.AreEqual("Action", node.Children[0].Name);
			Assert.AreEqual(0, node.Children[0].Children.Count);
		}

		[Test]
		public void VisitMethodDeclaration_ActionMemberNoArgumentsIsVirtual_CreatesEntryInNode()
		{
			var method = new MethodDeclaration { Name = "Action", Modifier = Modifiers.Public | Modifiers.Virtual };
			var node = new ControllerTreeNode(ControllerName, ControllerNamespace);

			treeService.Expect(s => s.Peek).Return(node);

			visitor.VisitMethodDeclaration(method, null);
			
			Assert.AreEqual("Action", node.Children[0].Name);
			Assert.AreEqual(0, node.Children[0].Children.Count);
		}

		[Test]
		public void VisitMethodDeclaration_ActionMemberStandardArgument_CreatesEntryInNode()
		{
			var method = new MethodDeclaration { Name = "Action", Modifier = Modifiers.Public };
			method.Parameters.Add(new ParameterDeclarationExpression(new TypeReference("bool"), "parameter"));
			var node = new ControllerTreeNode(ControllerName, ControllerNamespace);

			treeService.Expect(s => s.Peek).Return(node);
			typeResolver.Expect(r => r.Resolve(Arg<TypeReference>.Matches(t => t.SystemType == "System.Boolean"))).Return("System.Boolean");

			visitor.VisitMethodDeclaration(method, null);

			Assert.AreEqual("Action", node.Children[0].Name);
			Assert.AreEqual("parameter", node.Children[0].Children[0].Name);
		}

		private static AttributeSection CreateControllerAttributeCode(string attributeName, string argumentName, Expression valueExpression)
		{
			var argument = new NamedArgumentExpression(argumentName, valueExpression);
			
			var attribute = new Attribute(attributeName, new List<Expression>(), new List<NamedArgumentExpression>());
			attribute.NamedArguments.Add(argument);
			
			var attributeSection = new AttributeSection();
			attributeSection.Attributes.Add(attribute);
			
			return attributeSection;
		}

		private class CustomRestVerbResolver : IRestVerbResolver
		{
			public string Resolve(IRequest request)
			{
				return "GET";
			}
		}
	}
}