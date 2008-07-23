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
	public class DefaultTreeCreationServiceTests
	{
		private DefaultTreeCreationService service;

		[SetUp]
		public void Setup()
		{
			service = new DefaultTreeCreationService();
		}

		[Test]
		public void Constructor_Always_CreatesRoot()
		{
			Assert.AreEqual("Root", service.Root.Name);
			Assert.AreEqual(service.Root, service.Peek);
		}

		[Test]
		public void PushNode_Always_AddsChildAndSetsCurrent()
		{
			var node = new TreeNode("AnotherNode");
			service.PushNode(node);
			Assert.Contains(node, service.Root.Children);
			Assert.AreEqual(service.Root, node.Parent);
			Assert.AreEqual(node, service.Peek);
		}

		[Test]
		public void PopNode_NotUnderRoot_PopsTopNode()
		{
			var node = new TreeNode("AnotherNode");
			service.PushNode(node);
			Assert.AreEqual(node, service.Peek);
			service.PopNode();
			Assert.AreNotEqual(node, service.Peek);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException))]
		public void PopNode_UnderRoot_Throws()
		{
			service.PopNode();
		}

		[Test]
		public void FindNode_NodeNotThere_ReturnsNull()
		{
			service.PushNode(new TreeNode("NotTheNode"));
			Assert.IsNull(service.FindNode("AnotherNode"));
		}

		[Test]
		public void FindNode_NodeThere_ReturnsTheNode()
		{
			var node = new TreeNode("AnotherNode");
			service.PushNode(node);
			service.PopNode();
			Assert.AreEqual(node, service.FindNode("AnotherNode"));
		}

		[Test]
		public void PushArea_FirstTime_PushesNewNode()
		{
			service.PushArea("Area");
			Assert.AreEqual("Area", service.Peek.Name);
		}

		[Test]
		public void PushArea_SecondTime_PushesLastNode()
		{
			service.PushArea("Area");
			var node = service.Peek;
			service.PopNode();
			service.PushArea("Area");
			Assert.AreEqual(node, service.Peek);
		}

		[Test]
		public void PopToRoot_Always_PopsUntilRootCurrent()
		{
			service.PushArea("Area");
			service.PushArea("SubArea");
			service.PushArea("SubSubArea");
			service.PopToRoot();
			Assert.AreEqual(service.Root, service.Peek);
		}
	}
}