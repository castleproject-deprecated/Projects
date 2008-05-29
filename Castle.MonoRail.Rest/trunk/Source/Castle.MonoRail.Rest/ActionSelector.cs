// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System;
	using System.Collections;

	public static class ActionSelector
	{
		public static string GetRestActionName(string action, IDictionary actions, string httpMethod)
		{
			if (IsCollectionAction(action))
			{
				switch (httpMethod.ToUpper())
				{
					case "GET":
						return "Index";						
					case "POST":
						return "Create";
				}
			}

			if (IsNewAction(action))
			{
				return "New";
			}

			if (!actions.Contains(action))
			{
				switch (httpMethod.ToUpper())
				{
					case "GET":
						return "Show";
					case "PUT":
						return "Update";
					case "DELETE":
						return "Destroy";						
				}
			}

			return null;
		}

		public static bool IsCollectionAction(string action)
		{
			return String.Equals("collection", action, StringComparison.InvariantCultureIgnoreCase) ||
			       String.IsNullOrEmpty(action);
		}

		public static bool IsNewAction(string action)
		{
			return String.Equals("new", action, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}