using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	public class TreeNode
	{
		public static readonly string RootName = "Root";

		private List<TreeNode> _children = new List<TreeNode>();
		private TreeNode _parent;
		private string _name;

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

		public TreeNode(string name)
		{
			_name = name;
		}

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

		public override string ToString()
		{
			return String.Format("Node<{0}>", this.Name);
		}
	}
}