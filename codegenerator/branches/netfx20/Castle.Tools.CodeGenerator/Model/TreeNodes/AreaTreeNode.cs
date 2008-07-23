using System;

namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	public class AreaTreeNode : TreeNode
	{
		public AreaTreeNode(string name)
			: base(name)
		{
		}

		public override string ToString()
		{
			return String.Format("Area<{0}>", this.Name);
		}
	}
}