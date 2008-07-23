using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration.RouteParameters
{
	public abstract class RouteParameters : Dictionary<string, IRouteParameterType>
	{
		public RouteParameters GetFrom(string pattern) 
		{
			MatchCollection matches = ParameterRegex.Matches(pattern);

			foreach (Match match in matches)
			{
				string value = match.Value.Substring(1, match.Value.Length - 2);
				string name = value;
				IRouteParameterType type = new StringRouteParameterType();				

				if (value.Contains(":"))
				{					
					string[] parts = value.Split(':');
					name = parts[0];

					switch(parts[1])
					{
						case "int":
							type = new IntRouteParameterType();
							break;
						case "guid":
							type = new GuidRouteParameterType();
							break;
						default:
							type = new StringRouteParameterType(GetAnyOfChoices(parts[1]));
							break;
					}
				}
				
				Add(name, type);				
			}

			return this;
		}

		private static string[] GetAnyOfChoices(string pattern)
		{
			Regex regex = new Regex(@"\(((?<choice>[\w\d]+)\|?)+\)");
			Match match = regex.Match(pattern);
			Group choices = match.Groups["choice"];
			List<string> anyOfChoices = new List<string>();

			foreach (Capture capture in choices.Captures)
				anyOfChoices.Add(capture.Value);

			return anyOfChoices.ToArray();
		}

		private Regex ParameterRegex
		{
			get
			{
				return new Regex(string.Format(@"{0}((\w+)|(\w+:(int|guid|(anyof\(([\w\d]+\|)*[\w\d]+\))))){1}", OpeningParameterDelimiter, ClosingParameterDelimiter));
			}
		}

		protected abstract string OpeningParameterDelimiter { get; }
		protected abstract string ClosingParameterDelimiter { get; }
	}
}