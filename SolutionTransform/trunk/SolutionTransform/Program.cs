// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using SolutionTransform.Solutions;

[assembly:InternalsVisibleTo("SolutionTransform.Tests")]

namespace SolutionTransform {
	using System;

	public class Program {
		public static SolutionFile GetSolutionFile(string path)
		{
			return new SolutionFile(path, path.FileContent());
		}

        public static string FullPath(string file)
        {
            if (file.Contains("\\"))
            {
                if (file.Contains(":"))
                {
                    return file;
                }
                return Path.Combine(GetCurrentPath(), file);
            }
            return FindScript(GetCurrentPath(), file);
        }

	    private static string FindScript(string path, string file)
	    {
            string fullPath = Path.Combine(path, file);
	        if (File.Exists(fullPath))
	        {
                return fullPath;
	        }
            fullPath = Path.Combine(Path.Combine(path, "Scripts"), file);
            if (File.Exists(fullPath)) {
                return fullPath;
            }
            string parent = Path.GetDirectoryName(path);
            if (parent == null)
            {
                return null;
            }
            return FindScript(parent, file);
	    }

	    public static string GetCurrentPath()
        {
            var executablePath = Assembly.GetExecutingAssembly().CodeBase;
            return ToLocal(Path.GetDirectoryName(executablePath));
        }

        static string ToLocal(string uriStyle)
        {
            string result = uriStyle.Replace("/", "\\");
            return result.Substring(6);
        }

		public static int Main(string[] args)
		{
			if (args.Length < 2) {
				Console.WriteLine("Usage:  SolutionTransform <scriptPath> <solutionPath>");
			} else
			{
                try
                {

                    var interpreter = new Boo.Lang.Interpreter.InteractiveInterpreter2();
                    interpreter.SetValue("solution", GetSolutionFile(args[1]));
                    string scriptFile = args[0];
                    if (!scriptFile.Contains("."))
                    {
                        scriptFile += ".boo";
                    }
                    var script = FullPath(scriptFile).FileContent();

                    var context = interpreter.Eval(script);
                    foreach (var e in context.Errors)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    if (context.Errors.Count != 0)
                    {
                        return 2;
                    }
            	} catch (Exception ex) {
                    WriteException(Console.Out, ex);
                    return 1;
                }
			}
            return 0;
		}

        static void WriteException(TextWriter writer, Exception ex)
        {
            writer.WriteLine(ex.Message);
            writer.WriteLine();
            writer.WriteLine(ex.StackTrace);
            var inner = ex.InnerException;
            if (inner != null)
            {
                writer.WriteLine("======");
                WriteException(writer, inner);
            }
            
        }
	}
}
