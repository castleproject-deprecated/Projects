using System;
using System.Collections.Generic;
using Castle.Tools.CodeGenerator.Model.TreeNodes;
using Rhino.Mocks;
using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class DefaultTreeCreationServiceTests
  {
    #region Member Data
  	private MockRepository _mocks;
    private DefaultTreeCreationService _service;
  	#endregion
  	
  	#region Test Setup and Teardown Methods
  	[SetUp]
  	public void Setup()
  	{
  		_mocks = new MockRepository();
      _service = new DefaultTreeCreationService();
  	}
  	#endregion
  	
  	#region Test Methods
    [Test]
    public void Constructor_Always_CreatesRoot()
    {
      Assert.AreEqual("Root", _service.Root.Name);
      Assert.AreEqual(_service.Root, _service.Peek);
    }

    [Test]
    public void PushNode_Always_AddsChildAndSetsCurrent()
    {
      TreeNode node = new TreeNode("AnotherNode");
      _service.PushNode(node);
      Assert.Contains(node, _service.Root.Children);
      Assert.AreEqual(_service.Root, node.Parent);
      Assert.AreEqual(node, _service.Peek);
    }

    [Test]
    public void PopNode_NotUnderRoot_PopsTopNode()
    {
      TreeNode node = new TreeNode("AnotherNode");
      _service.PushNode(node);
      Assert.AreEqual(node, _service.Peek);
      _service.PopNode();
      Assert.AreNotEqual(node, _service.Peek);
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PopNode_UnderRoot_Throws()
    {
      _service.PopNode();
    }

    [Test]
    public void FindNode_NodeNotThere_ReturnsNull()
    {
      _service.PushNode(new TreeNode("NotTheNode"));
      Assert.IsNull(_service.FindNode("AnotherNode"));
    }

    [Test]
    public void FindNode_NodeThere_ReturnsTheNode()
    {
      TreeNode node = new TreeNode("AnotherNode");
      _service.PushNode(node);
      _service.PopNode();
      Assert.AreEqual(node, _service.FindNode("AnotherNode"));
    }

    [Test]
    public void PushArea_FirstTime_PushesNewNode()
    {
      _service.PushArea("Area");
      Assert.AreEqual("Area", _service.Peek.Name);
    }

    [Test]
    public void PushArea_SecondTime_PushesLastNode()
    {
      _service.PushArea("Area");
      TreeNode node = _service.Peek;
      _service.PopNode();
      _service.PushArea("Area");
      Assert.AreEqual(node, _service.Peek);
    }

    [Test]
    public void PopToRoot_Always_PopsUntilRootCurrent()
    {
      _service.PushArea("Area");
      _service.PushArea("SubArea");
      _service.PushArea("SubSubArea");
      _service.PopToRoot();
      Assert.AreEqual(_service.Root, _service.Peek);
    }
  	#endregion	
  }
}
