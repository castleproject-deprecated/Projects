using System;

namespace Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration.RouteParameters
{
	public interface IRouteParameterType
	{
		bool RequiresRestriction { get; }
		Type RequiredMethodParameterType { get; }
		Type OptionalMethodParameterType { get; }
	}
}