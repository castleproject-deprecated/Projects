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
	using System.CodeDom;
	using Model.TreeNodes;
	using NUnit.Framework;

	[TestFixture]
	public class ControllerMapGeneratorInAreaTests : ControllerMapGeneratorTests
	{
		private AreaTreeNode _root;

		public override void Setup()
		{
			base.Setup();
			
			_root = new AreaTreeNode("Root");
		}

		[Test]
		public void VisitAreaNode_AreaAsParent_CreatesType()
		{
			var node = new AreaTreeNode("Area");
			_root.AddChild(node);

			mocks.ReplayAll();
			generator.Visit(_root);
			mocks.VerifyAll();

			CodeDomAssert.AssertHasField(source.Ccu.Namespaces[0].Types[0], "_services");
		}

		[Test]
		public void VisitControllerTreeNode_Always_CreatesControllerType()
		{
			var node = new ControllerTreeNode("HomeController", "ControllerNamespace");
			_root.AddChild(node);

			mocks.ReplayAll();
			generator.Visit(_root);
			mocks.VerifyAll();

			var type = CodeDomAssert.AssertHasType(source.Ccu, "RootHomeControllerNode");
			CodeDomAssert.AssertNotHasField(type, "_services");
		}

		[Test]
		public void VisitControllerTreeNode_AreaExistsWithSameNameAsController_AppendsSuffixToAreaNodeField()
		{
			BuildTestTree();

			mocks.ReplayAll();
			generator.Visit(_root);
			mocks.VerifyAll();

			var type = CodeDomAssert.AssertHasType(source.Ccu, "RootParentAreaNode");
			var areaField = (CodeMemberField) type.Members[3];
			
			Assert.AreEqual("_childArea", areaField.Name);
		}

		[Test]
		public void VisitControllerTreeNode_AreaExistsWithSameNameAsController_AppendsSuffixToAreaNodeProperty()
		{
			BuildTestTree();

			mocks.ReplayAll();
			generator.Visit(_root);
			mocks.VerifyAll();

			var type = CodeDomAssert.AssertHasType(source.Ccu, "RootParentAreaNode");
			var areaProperty = (CodeMemberProperty) type.Members[2];
			
			Assert.AreEqual("ChildArea", areaProperty.Name);
			
			var statement = (CodeMethodReturnStatement) areaProperty.GetStatements[0];
			
			Assert.AreEqual("_childArea", ((CodeFieldReferenceExpression) statement.Expression).FieldName);
		}

		[Test]
		public void VisitControllerTreeNode_AreaExistsWithSameNameAsController_AppendsSuffixToControllerNodeField()
		{
			BuildTestTree();

			mocks.ReplayAll();
			generator.Visit(_root);
			mocks.VerifyAll();

			var type = CodeDomAssert.AssertHasType(source.Ccu, "RootParentAreaNode");
			var controllerField = (CodeMemberField) type.Members[5];
			
			Assert.AreEqual("_childController", controllerField.Name);
		}

		[Test]
		public void VisitControllerTreeNode_AreaExistsWithSameNameAsController_AppendsSuffixToControllerNodeProperty()
		{
			BuildTestTree();

			mocks.ReplayAll();
			generator.Visit(_root);
			mocks.VerifyAll();

			var type = CodeDomAssert.AssertHasType(source.Ccu, "RootParentAreaNode");
			var controllerProperty = (CodeMemberProperty) type.Members[4];
			
			Assert.AreEqual("ChildController", controllerProperty.Name);
			
			var statement = (CodeMethodReturnStatement) controllerProperty.GetStatements[0];
			
			Assert.AreEqual("_childController", ((CodeFieldReferenceExpression) statement.Expression).FieldName);
		}

		[Test]
		public void VisitControllerTreeNode_AreaExistsWithSameNameAsController_AppendsSuffixToConstructorFieldReferences()
		{
			BuildTestTree();

			mocks.ReplayAll();
			generator.Visit(_root);
			mocks.VerifyAll();

			var type = CodeDomAssert.AssertHasType(source.Ccu, "RootParentAreaNode");
			var constructor = (CodeConstructor) type.Members[1];
			
			Assert.AreEqual(3, constructor.Statements.Count);

			var areaAssignment = (CodeAssignStatement) constructor.Statements[1];
			var areaFieldReference = (CodeFieldReferenceExpression) areaAssignment.Left;
			
			Assert.AreEqual("_childArea", areaFieldReference.FieldName);

			var controllerAssignment = (CodeAssignStatement) constructor.Statements[2];
			var controllerFieldReference = (CodeFieldReferenceExpression) controllerAssignment.Left;
			
			Assert.AreEqual("_childController", controllerFieldReference.FieldName);
		}

		private void BuildTestTree()
		{
			var parentAreaNode = new AreaTreeNode("Parent");
			_root.AddChild(parentAreaNode);

			var childAreaNode = new AreaTreeNode("Child");
			parentAreaNode.AddChild(childAreaNode);

			var controllerNode = new ControllerTreeNode("ChildController", "ControllerNamespace");
			parentAreaNode.AddChild(controllerNode);
		}
	}
}