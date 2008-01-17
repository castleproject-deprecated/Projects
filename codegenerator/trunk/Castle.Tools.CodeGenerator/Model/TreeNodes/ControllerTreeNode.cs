using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	public class ControllerTreeNode : TreeNode
	{
		private string _namespace;

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
				while (node.Name != RootName)
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

		public ControllerTreeNode(string name, string controllerNamespace)
			: base(name)
		{
			_namespace = controllerNamespace;
		}

		public override string ToString()
		{
			string area = this.Area;
			if (!String.IsNullOrEmpty(area))
			{
				return String.Format("Controller<{0}/{1}>", area, this.Name);
			}
			return String.Format("Controller<{0}>", this.Name);
		}
	}
}