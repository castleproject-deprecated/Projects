using System;

namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	public class ViewComponentTreeNode : TreeNode
	{
		private string _namespace;

		public string Namespace
		{
			get { return _namespace; }
		}

		public ViewComponentTreeNode(string name, string componentNamespace)
			: base(name)
		{
			_namespace = componentNamespace;
		}

		public override string ToString()
		{
			return String.Format("ViewComponent<{0}>", this.Name);
		}
	}
}