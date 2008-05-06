#region license
// Copyright 2006-2008 Ken Egozi http://www.kenegozi.com/
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
#endregion

namespace Castle.MonoRail.AspView.VCompile
{
	using System;
	using System.IO;
	using System.Xml;

	using Views.AspView;
	using Views.AspView.Compiler;
	using Views.AspView.Compiler.Adapters;
	using Views.AspView.Compiler.Factories;

	class vcompile
	{
		static AspViewEngineOptions options;
		static string siteRoot;
		static int Main(string[] args)
		{
			if (args != null && args.Length == 1)
			{
				if (args[0].Equals("-w", StringComparison.InvariantCultureIgnoreCase) ||
				    args[0].Equals("-wait", StringComparison.InvariantCultureIgnoreCase))
					Console.ReadLine();
				else
					siteRoot = args[0];
			}
			if (siteRoot == null)
			{
				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				siteRoot = baseDirectory.Substring(0, baseDirectory.LastIndexOf("\\bin", StringComparison.InvariantCultureIgnoreCase));
			}
			Console.WriteLine("Compiling [" + siteRoot + "] ...");

			InitializeConfig();

			if (!Directory.Exists(siteRoot))
			{
				PrintHelpMessage(string.Format("The path '{0}' does not exist", siteRoot));
				Console.ReadLine();
				return -1;
			}

			OfflineCompiler compiler;
			try
			{
				ICompilationContext compilationContext = new CompilationContext(
					new DirectoryInfo(Path.Combine(siteRoot, "bin")),
					new DirectoryInfo(siteRoot),
					new DirectoryInfo(Path.Combine(siteRoot, "views")),
					new DirectoryInfo(options.CompilerOptions.TemporarySourceFilesDirectory));

				compiler = new OfflineCompiler(
					new CSharpCodeProviderAdapterFactory(),
					new PreProcessor(), 
					compilationContext, options.CompilerOptions, new DefaultFileSystemAdapter());
			}
			catch
			{
				PrintHelpMessage("Could not start the compiler.");
				return -2;
			}

			try
			{
				string path = compiler.Execute();

				Console.WriteLine("[{0}] compiled into [{1}].", siteRoot, path);
				return 0;
			}
			catch (Exception ex)
			{
				PrintHelpMessage("Could not compile." + Environment.NewLine + ex);
				return -3;
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

		private static void InitializeConfig()
		{
			InitializeConfig("aspView");
			if (options == null)
				InitializeConfig("aspview");
			if (options == null)
				options = new AspViewEngineOptions();

			Console.WriteLine(options.CompilerOptions.Debug ? "Compiling in DEBUG mode" : "");
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
			XmlNode aspViewNode;
			using (XmlTextReader reader = new XmlTextReader(path))
			{
				reader.Namespaces = false;
				XmlDocument xml = new XmlDocument();
				xml.Load(reader);
				aspViewNode = xml.SelectSingleNode("/configuration/" + configName);
			}

			if (aspViewNode != null)
			{
				AspViewConfigurationSection section = new AspViewConfigurationSection();
				options = (AspViewEngineOptions) section.Create(null, null, aspViewNode);
			}
		}

	}
}
