using System;

namespace Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration.RouteParameters
{
	public class GuidRouteParameterType : IRouteParameterType
	{
		public bool RequiresRestriction
		{
			get { return true; }
		}

		public Type RequiredMethodParameterType
		{
			get { return typeof(Guid); }
		}

		public Type OptionalMethodParameterType
		{
			get { return typeof(Guid?); }
		}
	}
}