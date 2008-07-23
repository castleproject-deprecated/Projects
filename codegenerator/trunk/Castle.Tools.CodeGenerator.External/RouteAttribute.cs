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

	/// <summary>
	/// Declares a route to an action.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class RouteAttribute : Attribute
	{
		/// <summary>
		/// Constructs a RouteAttribute, with the supplied order, name and pattern.
		/// </summary>
		/// <param name="order">The order in which all routes are tested (ie. 1 means this route will be tested first).</param>
		/// <param name="name">The name of the route.</param>
		/// <param name="pattern">The route pattern.</param>
		public RouteAttribute(int order, string name, string pattern)
		{
			Order = order;
			Name = name;
			Pattern = pattern;
		}

		/// <summary>
		/// The order in which all routes are tested (ie. 1 means this route will be tested first).
		/// </summary>
		public int Order { get; private set; }

		/// <summary>
		/// The name of the route.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The route pattern.
		/// </summary>
		public string Pattern { get; private set; }
	}
}