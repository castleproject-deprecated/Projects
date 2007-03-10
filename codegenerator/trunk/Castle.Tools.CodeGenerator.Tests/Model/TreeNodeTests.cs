using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.Model
{
  [TestFixture]
  public class TreeNodeTests
  {
    #region Member Data
    #endregion

    #region Test Methods
    [Test]
    public void ToString_Always_GiveInformation()
    {
      foreach (Type type in new Type[] { typeof(TreeNode), typeof(AreaTreeNode), typeof(ActionTreeNode), typeof(ParameterTreeNode), typeof(ViewTreeNode) })
      {
        TreeNode node = (TreeNode)Activator.CreateInstance(type, "Node");
        Assert.AreNotEqual(type.FullName, node.ToString());
      }
    }

    [Test]
    public void ControllerTreeNode_ToString_GiveInformation()
    {
      ControllerTreeNode node = new ControllerTreeNode("HomeController", "ControllerServices");
      AreaTreeNode rootNode = new AreaTreeNode("Root");
      rootNode.AddChild(node);
      Assert.AreNotEqual(node.GetType().FullName, node.ToString());
      Assert.AreEqual("ControllerServices", node.Namespace);
    }

    [Test]
    public void ViewComponentTreeNode_ToString_GiveInformation()
    {
      ViewComponentTreeNode node = new ViewComponentTreeNode("SomeComponent", "ControllerServices");
      AreaTreeNode rootNode = new AreaTreeNode("Root");
      rootNode.AddChild(node);
      Assert.AreNotEqual(node.GetType().FullName, node.ToString());
      Assert.AreEqual("ControllerServices", node.Namespace);
    }

    [Test]
    public void ControllerTreeNode_MultipleToString_GiveInformation()
    {
      ControllerTreeNode node = new ControllerTreeNode("HomeController", "ControllerServices");
      AreaTreeNode rootNode = new AreaTreeNode("Root");
      AreaTreeNode areaNode = new AreaTreeNode("Area");
      areaNode.AddChild(node);
      rootNode.AddChild(areaNode);
      Assert.AreNotEqual(node.GetType().FullName, node.ToString());
    }

    [Test]
    public void GetHashCode_WhenSame_Equal()
    {
      TreeNode node1 = new TreeNode("Node1");
      TreeNode node2 = new TreeNode("Node1");
      Assert.AreEqual(node1.GetHashCode(), node2.GetHashCode());
    }

    [Test]
    public void GetHashCode_WhenDifferent_NotEqual()
    {
      TreeNode node1 = new TreeNode("Node1");
      TreeNode node2 = new TreeNode("Node2");
      Assert.AreNotEqual(node1.GetHashCode(), node2.GetHashCode());
    }

    [Test]
    public void ControllerTreeNodeArea_Always_WalksUpwards()
    {
      ControllerTreeNode node = new ControllerTreeNode("HomeController", "ControllerServices");
      AreaTreeNode rootNode = new AreaTreeNode("Root");
      AreaTreeNode area1Node = new AreaTreeNode("Area1");
      AreaTreeNode area2Node = new AreaTreeNode("Area2");
      rootNode.AddChild(area1Node);
      area1Node.AddChild(area2Node);
      area2Node.AddChild(node);
      Assert.AreEqual("Area1/Area2", node.Area);
    }

    [Test]
    public void ControllerTreeNodePath_Always_WalksUpwards()
    {
      ControllerTreeNode node = new ControllerTreeNode("HomeController", "ControllerServices");
      AreaTreeNode rootNode = new AreaTreeNode("Root");
      AreaTreeNode area1Node = new AreaTreeNode("Area1");
      AreaTreeNode area2Node = new AreaTreeNode("Area2");
      rootNode.AddChild(area1Node);
      area1Node.AddChild(area2Node);
      area2Node.AddChild(node);
      Assert.AreEqual("Root/Area1/Area2", node.Path);
    }
    #endregion
  }
}
