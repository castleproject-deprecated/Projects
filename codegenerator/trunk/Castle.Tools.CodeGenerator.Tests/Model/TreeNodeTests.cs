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

namespace Castle.Tools.CodeGenerator.Model
{
	using System;
	using TreeNodes;
	using NUnit.Framework;

	[TestFixture]
	public class TreeNodeTests
	{
		[Test]
		public void ToString_Always_GiveInformation()
		{
			foreach (var type in new[] { typeof (TreeNode), typeof (AreaTreeNode), typeof (ActionTreeNode), typeof (ParameterTreeNode), typeof (ViewTreeNode) })
			{
				var node = (TreeNode) Activator.CreateInstance(type, "Node");
				Assert.AreNotEqual(type.FullName, node.ToString());
			}
		}

		[Test]
		public void ControllerTreeNode_ToString_GiveInformation()
		{
			var node = new ControllerTreeNode("HomeController", "ControllerServices");
			var rootNode = new AreaTreeNode("Root");
			rootNode.AddChild(node);
			Assert.AreNotEqual(node.GetType().FullName, node.ToString());
			Assert.AreEqual("ControllerServices", node.Namespace);
		}

		[Test]
		public void ViewComponentTreeNode_ToString_GiveInformation()
		{
			var node = new ViewComponentTreeNode("SomeComponent", "ControllerServices");
			var rootNode = new AreaTreeNode("Root");
			rootNode.AddChild(node);
			Assert.AreNotEqual(node.GetType().FullName, node.ToString());
			Assert.AreEqual("ControllerServices", node.Namespace);
		}

		[Test]
		public void ControllerTreeNode_MultipleToString_GiveInformation()
		{
			var node = new ControllerTreeNode("HomeController", "ControllerServices");
			var rootNode = new AreaTreeNode("Root");
			var areaNode = new AreaTreeNode("Area");
			areaNode.AddChild(node);
			rootNode.AddChild(areaNode);
			Assert.AreNotEqual(node.GetType().FullName, node.ToString());
		}

		[Test]
		public void GetHashCode_WhenSame_Equal()
		{
			var node1 = new TreeNode("Node1");
			var node2 = new TreeNode("Node1");
			Assert.AreEqual(node1.GetHashCode(), node2.GetHashCode());
		}

		[Test]
		public void GetHashCode_WhenDifferent_NotEqual()
		{
			var node1 = new TreeNode("Node1");
			var node2 = new TreeNode("Node2");
			Assert.AreNotEqual(node1.GetHashCode(), node2.GetHashCode());
		}

		[Test]
		public void ControllerTreeNodeArea_Always_WalksUpwards()
		{
			var node = new ControllerTreeNode("HomeController", "ControllerServices");
			var rootNode = new AreaTreeNode("Root");
			var area1Node = new AreaTreeNode("Area1");
			var area2Node = new AreaTreeNode("Area2");
			rootNode.AddChild(area1Node);
			area1Node.AddChild(area2Node);
			area2Node.AddChild(node);
			Assert.AreEqual("Area1/Area2", node.Area);
		}

		[Test]
		public void ControllerTreeNodePath_Always_WalksUpwards()
		{
			var node = new ControllerTreeNode("HomeController", "ControllerServices");
			var rootNode = new AreaTreeNode("Root");
			var area1Node = new AreaTreeNode("Area1");
			var area2Node = new AreaTreeNode("Area2");
			rootNode.AddChild(area1Node);
			area1Node.AddChild(area2Node);
			area2Node.AddChild(node);
			Assert.AreEqual("Root/Area1/Area2", node.Path);
		}
	}
}