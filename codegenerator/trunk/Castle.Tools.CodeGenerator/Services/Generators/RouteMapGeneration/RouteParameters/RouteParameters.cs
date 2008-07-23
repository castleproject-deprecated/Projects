// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Tools.CodeGenerator.Services.Generators.RouteMapGeneration.RouteParameters
{
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	public abstract class RouteParameters : Dictionary<string, IRouteParameterType>
	{
		public RouteParameters GetFrom(string pattern)
		{
			var matches = ParameterRegex.Matches(pattern);

			foreach (Match match in matches)
			{
				var value = match.Value.Substring(1, match.Value.Length - 2);
				var name = value;
				IRouteParameterType type = new StringRouteParameterType();

				if (value.Contains(":"))
				{
					var parts = value.Split(':');
					name = parts[0];

					switch (parts[1])
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
			var regex = new Regex(@"\(((?<choice>[\w\d]+)\|?)+\)");
			var match = regex.Match(pattern);
			var choices = match.Groups["choice"];
			var anyOfChoices = new List<string>();

			foreach (Capture capture in choices.Captures)
				anyOfChoices.Add(capture.Value);

			return anyOfChoices.ToArray();
		}

		private Regex ParameterRegex
		{
			get
			{
				return
					new Regex(string.Format(@"{0}((\w+)|(\w+:(int|guid|(anyof\(([\w\d]+\|)*[\w\d]+\))))){1}", OpeningParameterDelimiter,
					                        ClosingParameterDelimiter));
			}
		}

		protected abstract string OpeningParameterDelimiter { get; }
		protected abstract string ClosingParameterDelimiter { get; }
	}
}