using System;
using Castle.Tools.CodeGenerator.Model.TreeNodes;

namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	public class ActionTreeNode : TreeNode
	{
		public ControllerTreeNode Controller
		{
			get { return (ControllerTreeNode) this.Parent; }
		}

		public ActionTreeNode(string name)
			: base(name)
		{
		}

		public override string ToString()
		{
			return String.Format("Action<{0}>", this.Name);
		}
	}
}