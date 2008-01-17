using System;
using Castle.Tools.CodeGenerator.Model.TreeNodes;

namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	public class ViewTreeNode : TreeNode
	{
		public ControllerTreeNode Controller
		{
			get { return (ControllerTreeNode) this.Parent; }
		}

		public ViewTreeNode(string name)
			: base(name)
		{
		}

		public override string ToString()
		{
			return String.Format("View<{0}>", this.Name);
		}
	}
}