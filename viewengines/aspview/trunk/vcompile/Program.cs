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

namespace vcompile
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

    class Program
    {
        static AspViewEngineOptions options;
        static void Main(string[] args)
        {
            try
            {
                InitializeConfig();

                string siteRoot = options.SiteRoot ?? args[0].Substring(3);
                string[] references = options.AssembliesToReference.ToArray();
                if (!Directory.Exists(siteRoot))
                {
                    PrintHelpMessage(string.Format("The path '{0}' does not exist", siteRoot));
                    Console.ReadLine();
                    return;
                }
                AspViewCompiler compiler = new AspViewCompiler(
                    new AspViewCompilerOptions(true, false, false));
                compiler.CompileSite(siteRoot, references);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
            Console.ReadLine();

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
            options = (AspViewEngineOptions)ConfigurationManager.GetSection(configName);
        }
    }
}
