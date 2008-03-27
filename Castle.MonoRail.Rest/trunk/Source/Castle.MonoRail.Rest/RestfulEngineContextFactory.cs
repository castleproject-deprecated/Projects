using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MonoRail.Framework.Services;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Container;
using System.Web;
using Castle.MonoRail.Framework.Routing;
using System.Collections;
using Castle.MonoRail.Framework.Adapters;

namespace Castle.MonoRail.Rest
{
	public class RestfulEngineContextFactory : IEngineContextFactory
	{
		/// <summary>
		/// Pendent.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="urlInfo"></param>
		/// <param name="context"></param>
		/// <param name="routeMatch"></param>
		/// <returns></returns>
		public IEngineContext Create(IMonoRailContainer container, UrlInfo urlInfo, HttpContext context, RouteMatch routeMatch)
		{
			IDictionary session = ResolveRequestSession(container, urlInfo, context);

			IUrlBuilder urlBuilder = container.UrlBuilder;

			ServerUtilityAdapter serverUtility = new ServerUtilityAdapter(context.Server);

			string referrer = context.Request.Headers["Referer"];

			return new DefaultEngineContext(container, urlInfo, context,
											serverUtility,
											new RestfulRequestAdapter(context.Request),
											new ResponseAdapter(context.Response, urlInfo, urlBuilder, serverUtility, routeMatch, referrer),
											new TraceAdapter(context.Trace), session);
		}

		/// <summary>
		/// Resolves the request session.
		/// </summary>
		protected virtual IDictionary ResolveRequestSession(IMonoRailContainer container, UrlInfo urlInfo, HttpContext context)
		{
			return null;
		}
	}
}
