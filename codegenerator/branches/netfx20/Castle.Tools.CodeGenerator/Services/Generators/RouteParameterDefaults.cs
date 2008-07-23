using System.Collections.Generic;

namespace Castle.Tools.CodeGenerator.Services.Generators
{
	public class RouteParameterDefaults : Dictionary<string, string>
	{
		public RouteParameterDefaults GetFrom(string[] defaults)
		{
			foreach (string @default in defaults)
			{
				if (@default.Contains("="))
				{
					string[] parts = @default.Split('=');
					Add(parts[0], parts[1]);
				}
			}

			return this;
		}
	}
}