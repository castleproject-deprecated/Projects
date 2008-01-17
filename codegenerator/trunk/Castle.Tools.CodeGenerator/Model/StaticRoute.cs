using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Routing;

namespace Castle.Tools.CodeGenerator.Model
{
	[DebuggerDisplay("StaticRoute {url}")]
	public class StaticRoute : IRoutingRule
	{
		private readonly string routeName;
		private readonly string area;
		private readonly string controller;
		private readonly string action;
		private readonly string url;
		private readonly string[] routeParts;

		public StaticRoute(string routeName, string url, string area, string controller, string action)
		{
			this.routeName = routeName;
			this.url = url;
			this.area = area;
			this.controller = controller;
			this.action = action;

			routeParts = GetParts(url);
		}

		public string RouteName
		{
			get { return routeName; }
		}

		public string CreateUrl(string hostname, string virtualPath, IDictionary parameters)
		{
			if ((parameters["area"] != area) || (parameters["controller"] != controller) || (parameters["action"] != action))
				return null;

			StringBuilder text = new StringBuilder(virtualPath);

			foreach (string part in routeParts)
			{
				AppendSlash(text);

				text.Append(part);
			}

			AppendSlash(text);

			return text.ToString();
		}

		public bool Matches(string url, IRouteContext context, RouteMatch match)
		{
			string[] parts = GetParts(url);

			if (parts.Length != routeParts.Length)
			{
				return false;
			}

			for (int i = 0; i < parts.Length; i++)
			{
				if (parts[i] != routeParts[i])
				{
					return false;
				}
			}

			match.Parameters.Add("area", area);
			match.Parameters.Add("controller", controller);
			match.Parameters.Add("action", action);

			return true;
		}

		private static void AppendSlash(StringBuilder text)
		{
			if (text.Length == 0 || text[text.Length - 1] != '/')
			{
				text.Append('/');
			}
		}

		private static string[] GetParts(string url)
		{
			return url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}
