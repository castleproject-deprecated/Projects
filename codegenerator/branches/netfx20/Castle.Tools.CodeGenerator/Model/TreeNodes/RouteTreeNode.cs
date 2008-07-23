namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	public abstract class RouteTreeNode : TreeNode
	{
		private readonly string pattern;

		protected RouteTreeNode(string name, string pattern) : base(name)
		{
			this.pattern = pattern;
		}

		public string Pattern
		{
			get { return pattern; }
		}

		public ActionTreeNode Action
		{
			get { return (ActionTreeNode) Parent; }
		}
	}
}
