using System;

namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	public class ParameterTreeNode : TreeNode
	{
		private string _type;

		public string Type
		{
			get { return _type; }
		}

		public ParameterTreeNode(string name)
			: base(name)
		{
		}

		public ParameterTreeNode(string name, string type)
			: base(name)
		{
			_type = type;
		}

		public override string ToString()
		{
			return String.Format("Parameter<{0}, {1}>", this.Name, _type);
		}
	}
}