using System;

namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	public class PatternRouteTreeNode : RouteTreeNode
	{
		private readonly string[] defaults;

		public PatternRouteTreeNode(string name, string pattern, string[] defaults) : base(name, pattern)
		{
			this.defaults = defaults;
		}

		public string[] Defaults
		{
			get { return defaults; }
		}

		public override string ToString()
		{
			string d = string.Join(", ", defaults);

			return String.Format("PatternRoute<{0}, {1}, {2}>", Name, Pattern, d);
		}
	}
}