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

using System;

namespace Castle.Tools.CodeGenerator.Attributes
{
	/// <summary>
	/// Declares a route to an action.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class RouteAttribute : Attribute
	{
		private readonly int order;
		private readonly string name;
		private readonly string pattern;

		/// <summary>
		/// Constructs a RouteAttribute, with the supplied order, name and pattern.
		/// </summary>
		/// <param name="order">The order in which all routes are tested (ie. 1 means this route will be tested first).</param>
		/// <param name="name">The name of the route.</param>
		/// <param name="pattern">The route pattern.</param>
		public RouteAttribute(int order, string name, string pattern)
		{
			this.order = order;
			this.name = name;
			this.pattern = pattern;
		}

		/// <summary>
		/// The order in which all routes are tested (ie. 1 means this route will be tested first).
		/// </summary>
		public int Order
		{
			get { return order; }
		}

		/// <summary>
		/// The name of the route.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// The route pattern.
		/// </summary>
		public string Pattern
		{
			get { return pattern; }
		}
	}
}