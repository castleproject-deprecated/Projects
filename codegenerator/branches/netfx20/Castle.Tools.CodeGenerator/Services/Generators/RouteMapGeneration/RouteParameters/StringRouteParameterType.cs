using System;

namespace Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration.RouteParameters
{
	public class StringRouteParameterType : IRouteParameterType
	{
		public string[] anyOf = new string[0];

		public StringRouteParameterType()
		{
		}

		public StringRouteParameterType(string[] anyOf)
		{
			this.anyOf = anyOf;
		}

		public bool RequiresRestriction
		{
			get { return anyOf.Length > 0; }
		}

		public Type RequiredMethodParameterType
		{
			get { return typeof(string); }
		}

		public Type OptionalMethodParameterType
		{
			get { return typeof(string); }
		}
	}
}