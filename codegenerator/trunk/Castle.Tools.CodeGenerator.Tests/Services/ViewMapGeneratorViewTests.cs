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
	using Model.TreeNodes;
	using NUnit.Framework;

	[TestFixture]
	public class ViewMapGeneratorViewTests : ViewMapGeneratorTests
	{
		private AreaTreeNode root;
		private ControllerTreeNode controller;

		public override void Setup()
		{
			base.Setup();
			root = new AreaTreeNode("Root");
			controller = new ControllerTreeNode("HomeController", "ControllerNamespace");
			root.AddChild(controller);
		}

		[Test]
		public void VisitViewNode_UnderNoController_DoesNothing()
		{
			var node = new ViewTreeNode("Index");

			mocks.ReplayAll();
			generator.Visit(node);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitViewNode_NoParameters_CreatesMethod()
		{
			var node = new ViewTreeNode("Index");
			controller.AddChild(node);

			mocks.ReplayAll();
			generator.Visit(controller);
			mocks.VerifyAll();

			CodeDomAssert.AssertHasField(source.Ccu.Namespaces[0].Types[0], "_services");
			CodeDomAssert.AssertHasProperty(source.Ccu.Namespaces[0].Types[0], "Index");
		}

		[Test]
		public void VisitViewNode_OneParameters_CreatesMethod()
		{
			var node = new ViewTreeNode("Index");
			controller.AddChild(node);
			node.AddChild(new ParameterTreeNode("id", "System.Int32"));

			using (mocks.Unordered())
			{
			}

			mocks.ReplayAll();
			generator.Visit(controller);
			mocks.VerifyAll();

			CodeDomAssert.AssertHasField(source.Ccu.Namespaces[0].Types[0], "_services");
			CodeDomAssert.AssertHasProperty(source.Ccu.Namespaces[0].Types[0], "Index");
		}
	}
}