using System;
using System.Collections.Generic;
using System.Reflection;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class TreeWalker
  {
    public virtual void Accept(TreeNode node)
    {
      Type type = node.GetType();
      MethodInfo method = GetType().GetMethod("Visit", new Type[] { type });
      if (method != null)
      {
        method.Invoke(this, new object[] { node });
      }
    }

    public virtual void Accept(ICollection<TreeNode> nodes)
    {
      foreach (TreeNode child in nodes)
      {
        Accept(child);
      }
    }

    public virtual void Visit(AreaTreeNode node)
    {
      Accept(node.Children);
    }

    public virtual void Visit(ControllerTreeNode node)
    {
      Accept(node.Children);
    }

    public virtual void Visit(ActionTreeNode node)
    {
      Accept(node.Children);
    }

    public virtual void Visit(ViewComponentTreeNode node)
    {
      Accept(node.Children);
    }

    public virtual void Visit(ViewTreeNode node)
    {
      Accept(node.Children);
    }

	  public virtual void Visit(WizardControllerTreeNode node)
	  {
	  	Accept(node.Children);
	  }
  }
}
