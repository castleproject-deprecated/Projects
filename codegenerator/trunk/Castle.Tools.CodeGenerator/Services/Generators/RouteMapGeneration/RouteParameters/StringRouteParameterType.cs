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
	using System;

	public class StringRouteParameterType : IRouteParameterType
	{
		public string[] anyOf = new string[0];

		public StringRouteParameterType()
		{
		}

		public StringRouteParameterType(string[] anyOf)
		{
			this.anyOf = anyOf;
		}

		public bool RequiresRestriction
		{
			get { return anyOf.Length > 0; }
		}

		public Type RequiredMethodParameterType
		{
			get { return typeof (string); }
		}

		public Type OptionalMethodParameterType
		{
			get { return typeof (string); }
		}
	}
}