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


using System;

namespace ViewUpgrader
{
	using System.IO;
	using System.Collections.Generic;

	class Program
	{
		static void Main(string[] args)
		{
			string siteRoot = GetSiteRoot(args, Environment.CurrentDirectory);
			
			string viewsFolder = Path.Combine(siteRoot, "Views");

			string[] viewFiles = Directory.GetFiles(viewsFolder, "*.aspx", SearchOption.AllDirectories);

			foreach (string viewFile in viewFiles)
			{
				string source = File.ReadAllText(viewFile);

				string newSource = new Upgrader(source)
					.TransformPropertyDecleration()
					.TransformSubViewProperties()
					.ToString();

				File.WriteAllText(viewFile, newSource);
			}
		}

		static string GetSiteRoot(IEnumerable<string> args, string defaultValue)
		{
			foreach (string arg in args)
			{
				if (arg.StartsWith("/r:"))
					return arg.Replace("/r:", "");
			}
			return defaultValue;
		}
	}

}
