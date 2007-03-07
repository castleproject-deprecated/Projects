// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.AspView.Compiler
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.IO;
	using System.Text.RegularExpressions;
	using System.CodeDom.Compiler;
	using System.Collections.Specialized;
	using Microsoft.CSharp;
	using Castle.MonoRail.Views.AspView;
	using System.Configuration;
	using System.Diagnostics;
	using System.Reflection;
	using System.Web.Configuration;
	using System.Xml;
	using System.Xml.XPath;

	class vcompile
	{
		static AspViewEngineOptions options;
		static string siteRoot;
		static int Main(string[] args)
		{
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			siteRoot = baseDirectory.Substring(0, baseDirectory.LastIndexOf("\\bin"));

			InitializeConfig();

			if (!Directory.Exists(siteRoot))
			{
				PrintHelpMessage(string.Format("The path '{0}' does not exist", siteRoot));
				Console.ReadLine();
				return 1;
			}

			AspViewCompiler compiler = new AspViewCompiler(options.CompilerOptions);

			if (compiler == null)
			{
				PrintHelpMessage("Could not start the compiler.");
				return 2;
			}
			try
			{
				compiler.CompileSite(siteRoot);
				Console.WriteLine("finish");
				return 0;
			}
			catch (Exception ex)
			{
				PrintHelpMessage("Could not compile." + Environment.NewLine + ex.ToString());
				return 3;
			}

		}

		private static void PrintHelpMessage(string message)
		{
			if (message != null)
			{
				Console.WriteLine("message from the compiler:");
				Console.WriteLine(message);
				Console.WriteLine();
				Console.WriteLine();
			}
			Console.WriteLine("usage: vcompile [options]");
			Console.WriteLine();
			Console.WriteLine("valid options:");
			Console.WriteLine("\t/r:siteRoot\t\twill compile the site at siteRoot directory");
			Console.WriteLine();
			Console.WriteLine("example:");
			Console.WriteLine("\tvcompile /r:C:\\Dev\\Sites\\MySite\t\twill compile the site at 'C:\\Dev\\Sites\\MySite' directory");
			Console.WriteLine();
		}

		private static void PrintHelpMessage()
		{
			PrintHelpMessage(null);
		}
		private static void InitializeConfig()
		{
			InitializeConfig("aspView");
			if (options == null)
				InitializeConfig("aspview");
			if (options == null)
				options = new AspViewEngineOptions();
		}
		private static void InitializeConfig(string configName)
		{
			string path = Path.Combine(siteRoot, "web.config");
			if (!File.Exists(path))
			{
				Console.WriteLine("Cannot locate web.config" + Environment.NewLine +
					"VCompile should run from the bin directory of the website");
				Environment.Exit(1);
			}
			XmlNode aspViewNode = null;
			using (XmlTextReader reader = new XmlTextReader(path))
			{
				XmlDocument xml = new XmlDocument();
				xml.Load(reader);
				aspViewNode = xml.SelectSingleNode("/configuration/aspview");
			}
			AspViewConfigurationSection section = new AspViewConfigurationSection();
			options = (AspViewEngineOptions)section.Create(null, null, aspViewNode);
			if (options != null)
				Console.WriteLine(options.CompilerOptions.Debug);
		}

	}
}
