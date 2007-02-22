using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.Tools.CodeGenerator.Model
{
  public class TreeNode
  {
    #region Static Member Data
    public static readonly string RootName = "Root";
    #endregion

    #region Member Data
    private List<TreeNode> _children = new List<TreeNode>();
    private TreeNode _parent;
    private string _name;
    #endregion

    #region Properties
    public TreeNode Parent
    {
      get { return _parent; }
      protected set { _parent = value; }
    }
    public List<TreeNode> Children
    {
      get { return _children; }
    }
    public string Name
    {
      get { return _name; }
    }
    public string Path
    {
      get
      {
        List<string> parts = new List<string>();
        TreeNode node = this.Parent;
        StringBuilder sb = new StringBuilder();
        while (node != null)
        {
          parts.Add(node.Name);
          node = node.Parent;
        }
        parts.Reverse();
        foreach (string part in parts)
        {
          if (sb.Length != 0)
          {
            sb.Append("/");
          }
          sb.Append(part);
        }
        return sb.ToString();
      }
    }
    public string PathNoSlashes
    {
      get { return this.Path.Replace("/", ""); }
    }
    #endregion

    #region TreeNode()
    public TreeNode(string name)
    {
      _name = name;
    }
    #endregion

    #region Methods
    public void AddChild(TreeNode node)
    {
      AddChild(node, false);
    }

    public void AddChild(TreeNode node, bool force)
    {
      if (!force && _children.Contains(node))
      {
        return;
      }
      node.Parent = this;
      _children.Add(node); 
    }

    public override int GetHashCode()
    {
      return this.Name.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      TreeNode node = obj as TreeNode;
      if (node == null)
        return base.Equals(obj);
      if (node.GetType() != GetType())
        return base.Equals(obj);
      return node.Name.Equals(this.Name);
    }
    #endregion

    #region Methods
    public override string ToString()
    {
      return String.Format("Node<{0}>", this.Name);
    }
    #endregion
  }

  public class AreaTreeNode : TreeNode
  {
    #region Member Data
    #endregion

    #region AreaTreeNode()
    public AreaTreeNode(string name)
     : base(name)
    {
    }
    #endregion

    #region Methods
    public override string ToString()
    {
      return String.Format("Area<{0}>", this.Name);
    }
    #endregion
  }

  public class ControllerTreeNode : TreeNode
  {
    #region Member Data
    private string _namespace;
    #endregion

    #region Properties
    public string Namespace
    {
      get { return _namespace; }
    }

    public string FullName
    {
      get { return this.Namespace + "." + this.Name; }
    }
    
    public string Area
    {
      get
      {
        List<string> parts = new List<string>();
        TreeNode node = this.Parent;
        StringBuilder sb = new StringBuilder();
        while (node.Name != TreeNode.RootName)
        {
          parts.Add(node.Name);
          node = node.Parent;
        }
        parts.Reverse();
        foreach (string part in parts)
        {
          if (sb.Length != 0)
          {
            sb.Append("/");
          }
          sb.Append(part);
        }
        return sb.ToString();
      }
    }
    #endregion

    #region ControllerTreeNode()
    public ControllerTreeNode(string name, string controllerNamespace)
     : base(name)
    {
      _namespace = controllerNamespace;
    }
    #endregion

    #region Methods
    public override string ToString()
    {
      string area = this.Area;
      if (!String.IsNullOrEmpty(area))
      {
        return String.Format("Controller<{0}/{1}>", area, this.Name);
      }
      return String.Format("Controller<{0}>", this.Name);
    }
    #endregion
  }

  public class ActionTreeNode : TreeNode
  {
    #region Properties
    public ControllerTreeNode Controller
    {
      get { return (ControllerTreeNode)this.Parent; }
    }
    #endregion

    #region ActionTreeNode()
    public ActionTreeNode(string name)
     : base(name)
    {
    }
    #endregion

    #region Methods
    public override string ToString()
    {
      return String.Format("Action<{0}>", this.Name);
    }
    #endregion
  }

  public class ParameterTreeNode : TreeNode
  {
    #region Member Data
    private string _type;
    #endregion

    #region Properties
    public string Type
    {
      get { return _type; }
    }
    #endregion

    #region ParameterTreeNode()
    public ParameterTreeNode(string name)
     : base(name)
    {
    }

    public ParameterTreeNode(string name, string type)
     : base(name)
    {
      _type = type;
    }
    #endregion

    #region Methods
    public override string ToString()
    {
      return String.Format("Parameter<{0}, {1}>", this.Name, _type);
    }
    #endregion
  }

  public class ViewTreeNode : TreeNode
  {
    #region Properties
    public ControllerTreeNode Controller
    {
      get { return (ControllerTreeNode)this.Parent; }
    }
    #endregion

    #region ViewTreeNode()
    public ViewTreeNode(string name)
     : base(name)
    {
    }
    #endregion

    #region Methods
    public override string ToString()
    {
      return String.Format("View<{0}>", this.Name);
    }
    #endregion
  }

  public class ViewComponentTreeNode : TreeNode
  {
    #region Member Data
    private string _namespace;
    #endregion

    #region Properties
    public string Namespace
    {
      get { return _namespace; }
    }
    #endregion

    #region ViewComponentTreeNode()
    public ViewComponentTreeNode(string name, string componentNamespace)
     : base(name)
    {
      _namespace = componentNamespace;
    }
    #endregion

    #region Methods
    public override string ToString()
    {
      return String.Format("ViewComponent<{0}>", this.Name);
    }
    #endregion
  }
}
