using System;
using System.Collections.Generic;
using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class DefaultTreeCreationService : ITreeCreationService
  {
    #region Member Data
    private Stack<TreeNode> _nodes = new Stack<TreeNode>();
    private TreeNode _root;
    #endregion

    #region Properties
    public TreeNode Root
    {
      get { return _root; }
    }

    public TreeNode Peek
    {
      get { return _nodes.Peek(); }
    }
    #endregion

    #region DefaultTreeCreationService()
    public DefaultTreeCreationService()
    {
      _root = new AreaTreeNode("Root");
      _nodes.Push(_root);
    }
    #endregion

    #region Methods
    public void PushNode(TreeNode node)
    {
      _nodes.Peek().AddChild(node);
      _nodes.Push(node);
    }

    public TreeNode FindNode(string name)
    {
      foreach (TreeNode node in _nodes.Peek().Children)
      {
        if (node.Name == name)
        {
          return node;
        }
      }
      return null;
    }

    public void PopNode()
    {
      if (_nodes.Count == 1)
        throw new InvalidOperationException();
      _nodes.Pop();
    }

    public void PopToRoot()
    {
      while (_nodes.Count > 1)
      {
        _nodes.Pop();
      }
    }

    public void PushArea(string name)
    {
      TreeNode node = FindNode(name);
      if (node == null)
      {
        node = new AreaTreeNode(name);
      }
      PushNode(node);
    }
    #endregion
  }
}
