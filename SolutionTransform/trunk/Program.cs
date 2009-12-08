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

namespace SolutionTransform {
	using System;

	public class Program {
		public static SolutionFile GetSolutionFile(string path)
		{
			return new SolutionFile(path, path.FileContent());
		}

		public static void Main(string[] args)
		{
			if (args.Length < 2) {
				Console.WriteLine("Usage:  SolutionTransform <scriptPath> <solutionPath>");
			} else
			{
				var interpreter = new Boo.Lang.Interpreter.InteractiveInterpreter2();
				interpreter.SetValue("solution", GetSolutionFile(args[1]));
				var script = args[0].FileContent();
				var context = interpreter.Eval(script);
				foreach (var e in context.Errors)
				{
					Console.WriteLine(e.ToString());
				}
			}
		}
	}
}
