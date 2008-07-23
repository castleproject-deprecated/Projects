using System;

namespace Castle.Tools.CodeGenerator.Attributes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class StaticRouteAttribute : Attribute
	{
		private readonly string name;
		private readonly string url;

		public StaticRouteAttribute(string name, string url)
		{
			this.name = name;
			this.url = url;
		}

		public string Name
		{
			get { return name; }
		}

		public string Url
		{
			get { return url; }
		}
	}
}
