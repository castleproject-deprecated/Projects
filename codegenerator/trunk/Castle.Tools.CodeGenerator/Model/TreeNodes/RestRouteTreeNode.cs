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

namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	using System;

	public class RestRouteTreeNode : PatternRouteTreeNode
	{
		private readonly string requiredVerb;
		private readonly string restVerbResolver;

		public RestRouteTreeNode(string name, string pattern, string requiredVerb, string restVerbResolver) : base(name, pattern, new string[0])
		{
			this.requiredVerb = requiredVerb;
			this.restVerbResolver = restVerbResolver;
		}

		public string RequiredVerb
		{
			get { return requiredVerb; }
		}

		public string RestVerbResolver
		{
			get { return restVerbResolver; }
		}

		public override string ToString()
		{
			return String.Format("RestRouteRoute<{0}, {1}, {2}, {3}>", Name, Pattern, RequiredVerb, RestVerbResolver);
		}
	}
}