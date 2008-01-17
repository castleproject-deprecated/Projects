namespace Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration.RouteParameters
{
	public class RequiredRouteParameters : RouteParameters
	{
		protected override string OpeningParameterDelimiter
		{
			get { return "<"; }
		}

		protected override string ClosingParameterDelimiter
		{
			get { return ">"; }
		}
	}
}
