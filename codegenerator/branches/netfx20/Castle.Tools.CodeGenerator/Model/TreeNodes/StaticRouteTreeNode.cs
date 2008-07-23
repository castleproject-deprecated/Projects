using System;

namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	public class StaticRouteTreeNode : RouteTreeNode
	{
		public StaticRouteTreeNode(string name, string pattern) : base(name, pattern)
		{
		}

		public override string ToString()
		{
			return String.Format("StaticRoute<{0}, {1}>", Name, Pattern);
		}
	}
}