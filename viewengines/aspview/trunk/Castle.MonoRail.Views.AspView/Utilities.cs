// Copyright 2006-2007 Ken Egozi http://www.kenegozi.com/
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

namespace Castle.MonoRail.Views.AspView
{
	using System.Collections.Generic;
	using Framework;

	internal static class Utilities
	{
		internal static IDictionary<string, object> ConvertArgumentsToParameters(object[] arguments)
		{
			if (arguments.Length % 2 != 0)
				throw new AspViewException("Parameters should be arranged as key and value pairs");
			int i = 0;
			IDictionary<string, object> parameters = new Dictionary<string, object>(arguments.Length / 2, CaseInsensitiveStringComparer.Default);
			while (i < arguments.Length)
			{
				string name = arguments[i] as string;
				if (name == null)
					throw new AspViewException("Parameters should be arranged as key and value pairs");
				object key = arguments[i + 1];
				parameters.Add(name, key);
				i += 2;
			}
			return parameters;
		}
	}
}
