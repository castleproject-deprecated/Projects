using System;

namespace Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration.RouteParameters
{
	public class IntRouteParameterType : IRouteParameterType
	{
		public bool RequiresRestriction
		{
			get { return true; }
		}

		public Type RequiredMethodParameterType
		{
			get { return typeof(int); }
		}

		public Type OptionalMethodParameterType
		{
			get { return typeof(int?); }
		}
	}
}