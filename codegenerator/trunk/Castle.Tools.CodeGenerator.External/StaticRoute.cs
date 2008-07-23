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

namespace Castle.Tools.CodeGenerator.External
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Text;
	using MonoRail.Framework;
	using MonoRail.Framework.Routing;

	[DebuggerDisplay("StaticRoute {url}")]
	public class StaticRoute : IRoutingRule
	{
		private readonly string routeName;
		private readonly string area;
		private readonly string controller;
		private readonly string action;
		private readonly string[] routeParts;

		public StaticRoute(string routeName, string url, string area, string controller, string action)
		{
			this.routeName = routeName;
			this.area = area;
			this.controller = controller;
			this.action = action;

			routeParts = GetParts(url);
		}

		public string RouteName
		{
			get { return routeName; }
		}

		public string CreateUrl(IDictionary parameters)
		{
			var text = new StringBuilder();

			foreach (var part in routeParts)
			{
				AppendSlash(text);
				text.Append(part);
			}

			AppendSlash(text);

			return text.ToString();
		}

		public int Matches(string url, IRouteContext context, RouteMatch match)
		{
			var parts = GetParts(url);

			if (parts.Length != routeParts.Length)
				return 0;
			
			for (var i = 0; i < parts.Length; i++)
				if (string.Compare(parts[i], routeParts[i], true) != 0)
					return 0;
			
			match.Parameters.Add("area", area);
			match.Parameters.Add("controller", controller);
			match.Parameters.Add("action", action);

			return 100;
		}

		private static void AppendSlash(StringBuilder text)
		{
			if (text.Length == 0 || text[text.Length - 1] != '/')
				text.Append('/');
		}

		private static string[] GetParts(string url)
		{
			return url.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}