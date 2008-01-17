using System;

namespace Castle.Tools.CodeGenerator.Attributes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class PatternRouteAttribute : Attribute
	{
		private readonly string name;
		private readonly string pattern;
		private readonly string[] parameters;

		public PatternRouteAttribute(string name, string pattern, params string[] parameters)
		{
			this.name = name;
			this.pattern = pattern;
			this.parameters = parameters;
		}

		public string Name
		{
			get { return name; }
		}

		public string Pattern
		{
			get { return pattern; }
		}

		public string[] Parameters
		{
			get { return parameters; }
		}
	}
}
