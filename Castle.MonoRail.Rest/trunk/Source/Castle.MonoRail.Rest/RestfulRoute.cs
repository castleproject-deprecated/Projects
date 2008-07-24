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

namespace Castle.MonoRail.Rest
{
	using System.Diagnostics;
	using Framework;
	using Framework.Routing;
	
	[DebuggerDisplay("RestfulRoute {pattern}")]
	public class RestfulRoute : PatternRoute
	{
		private readonly string requiredVerb;
		private IRestVerbResolver restVerbResolver = new DefaultRestVerbResolver();

		public RestfulRoute(string pattern, string requiredVerb) : base(pattern)
		{
			this.requiredVerb = requiredVerb;
		}

		public RestfulRoute(string name, string pattern, string requiredVerb) : base(name, pattern)
		{
			this.requiredVerb = requiredVerb;
		}

		public override int Matches(string url, IRouteContext context, RouteMatch match)
		{
			var points = base.Matches(url, context, match);
			
			if ((points == 0) || (string.Compare(restVerbResolver.Resolve(context.Request), requiredVerb, true) != 0))
				return 0;
			
			return points;
		}

		public RestfulRoute WithRestVerbResolver(IRestVerbResolver resolver)
		{
			restVerbResolver = resolver;
			return this;
		}
	}
}