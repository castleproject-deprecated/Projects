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

namespace Castle.Tools.CodeGenerator.Services
{
	using System;
	using System.Text;

	public class DefaultNamingService : INamingService
	{
		public string ToVariableName(string name)
		{
			return name[0].ToString().ToLower() + name.Substring(1);
		}

		public string ToPropertyName(string name)
		{
			return name[0].ToString().ToUpper() + name.Substring(1);
		}

		public string ToRouteWrapperName(string name)
		{
			return name + "RouteNode";
		}

		public string ToMemberVariableName(string name)
		{
			return "_" + ToVariableName(name);
		}

		public string ToClassName(string name)
		{
			return ToPropertyName(name);
		}

		public string ToControllerName(string name)
		{
			return name.EndsWith("Controller") ? name.Replace("Controller", "") : name;
		}

		public string ToAreaWrapperName(string name)
		{
			return name + "AreaNode";
		}

		public string ToControllerWrapperName(string name)
		{
			return name + "Node";
		}

		public string ToActionWrapperName(string name)
		{
			return name + "ActionNode";
		}

		public string ToViewWrapperName(string name)
		{
			return name + "ViewNode";
		}

		public string ToMethodSignatureName(string name, Type[] types)
		{
			var names = new string[types.Length];
			
			for (var i = 0; i < types.Length; i++)
				names[i] = types[i].Name;
			
			return ToMethodSignatureName(name, names);
		}

		public string ToWizardStepWrapperName(string name)
		{
			return name + "WizardStepNode";
		}

		public string ToMethodSignatureName(string name, string[] types)
		{
			var sb = new StringBuilder();
			sb.Append(name);
			
			foreach (var type in types)
				sb.Append("_").Append(type.Replace(".", "_").Replace("[]", "BB"));
			
			return sb.ToString();
		}
	}
}