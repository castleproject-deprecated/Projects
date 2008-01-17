using System;
using System.Collections.Generic;
using Castle.Tools.CodeGenerator.Model.TreeNodes;
using Rhino.Mocks;
using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class ViewSourceMapperTests
  {
    #region Member Data
    private MockRepository _mocks;
    private ViewSourceMapper _mapper;
    private ITreeCreationService _treeService;
    private INamingService _naming;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _mocks = new MockRepository();
      _treeService = _mocks.CreateMock<ITreeCreationService>();
      _naming = _mocks.CreateMock<INamingService>();
      _mapper = new ViewSourceMapper(new NullLogger(), _treeService, _naming);
    }
    #endregion

    #region Test Methods
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void AddViewSource_PathWithNoViewsDirectory_ThrowsException()
    {
      using (_mocks.Unordered())
      {
      }

      _mocks.ReplayAll();
      _mapper.AddViewSource(@"Projects\Eleutian.Web.Site\Home\Index.brail");
      _mocks.VerifyAll();
    }

    [Test]
    public void AddViewSource_ViewForMissingController_DoesNothing()
    {
      using (_mocks.Unordered())
      {
        Expect.Call(_treeService.FindNode("HomeController")).Return(null);
        Expect.Call(_treeService.FindNode("HomeComponent")).Return(null);
        Expect.Call(_treeService.FindNode("Home")).Return(null);
        _treeService.PopToRoot();
      }

      _mocks.ReplayAll();
      _mapper.AddViewSource(@"Projects\Eleutian.Web.Site\Views\Home\Index.brail");
      _mocks.VerifyAll();
    }

    [Test]
    public void AddViewSource_ViewForValidTopLevelController_AddsViewNode()
    {
      ControllerTreeNode node = new ControllerTreeNode("HomeController", "SomeNamespace");
      AreaTreeNode root = new AreaTreeNode("Root");
      root.AddChild(node);

      using (_mocks.Unordered())
      {
        Expect.Call(_treeService.FindNode("HomeController")).Return(node);
        _treeService.PushNode(node);
        _treeService.PopToRoot();
      }

      _mocks.ReplayAll();
      _mapper.AddViewSource(@"Projects\Eleutian.Web.Site\Views\Home\Index.brail");
      _mocks.VerifyAll();

      AssertHasViewNode(node);
    }

    [Test]
    public void AddViewSource_ViewForValidSubAreaController_AddsViewNode()
    {
      ControllerTreeNode node = new ControllerTreeNode("HomeController", "SomeNamespace");
      AreaTreeNode root = new AreaTreeNode("Root");
      AreaTreeNode area = new AreaTreeNode("Root");
      root.AddChild(area);
      area.AddChild(node);

      using (_mocks.Unordered())
      {
        Expect.Call(_treeService.FindNode("AreaController")).Return(null);
        Expect.Call(_treeService.FindNode("AreaComponent")).Return(null);
        Expect.Call(_treeService.FindNode("Area")).Return(area);
        _treeService.PushNode(area);
        Expect.Call(_treeService.FindNode("HomeController")).Return(node);
        _treeService.PushNode(node);
        _treeService.PopToRoot();
      }

      _mocks.ReplayAll();
      _mapper.AddViewSource(@"Projects\Eleutian.Web.Site\Views\Area\Home\Index.brail");
      _mocks.VerifyAll();

      AssertHasViewNode(node);
    }
    #endregion

    #region Methods
    private static void AssertHasViewNode(ControllerTreeNode node)
    {
      Assert.IsInstanceOfType(typeof(ViewTreeNode), node.Children[0]);
      Assert.AreEqual("Index", node.Children[0].Name);
    }
    #endregion
  }
}
