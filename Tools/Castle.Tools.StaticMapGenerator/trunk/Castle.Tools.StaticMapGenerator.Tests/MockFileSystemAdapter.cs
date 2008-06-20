#region license
// Copyright 2008 Ken Egozi http://www.kenegozi.com/blog
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

namespace Castle.Tools.StaticMapGenerator.Tests
{
	using System.Collections.Generic;
	using Services;

	public class MockFileSystemAdapter : FileSystemAdapter
	{
		readonly Dictionary<string, string[]> directories = new Dictionary<string, string[]>();
		readonly Dictionary<string, string[]> scriptFiles = new Dictionary<string, string[]>();
		readonly Dictionary<string, string[]> styleFiles = new Dictionary<string, string[]>();
		readonly Dictionary<string, string[]> imageFiles = new Dictionary<string, string[]>();

		public MockFileSystemAdapter()
		{
			directories.Add(@"C:\", new string[] { @"C:\grandpa", @"C:\grandma" });
			directories.Add(@"C:\grandma", new string[] { @"C:\grandma\mom", @"C:\grandma\dad" });
			directories.Add(@"C:\grandma\dad", new string[] { @"C:\grandma\dad\Joe"});

			scriptFiles.Add(@"C:\grandpa", new string[] { @"script1.js" });
			scriptFiles.Add(@"C:\grandma\dad", new string[] { @"script2.js" });

			styleFiles.Add(@"C:\grandpa", new string[] { @"style1.css" });

			imageFiles.Add(@"C:\grandpa", new string[] { @"image1.jpg" });
			imageFiles.Add(@"C:\grandma\mom", new string[] { @"image2.gif" });
		}

		public override string[] GetScriptFiles(string dir)
		{
			if (scriptFiles.ContainsKey(dir))
				return scriptFiles[dir];
			return new string[0];
		}
		public override string[] GetStyleFiles(string dir)
		{
			if (styleFiles.ContainsKey(dir))
				return styleFiles[dir];
			return new string[0];
		}
		public override string[] GetImageFiles(string dir)
		{
			if (imageFiles.ContainsKey(dir))
				return imageFiles[dir];
			return new string[0];
		}
		public override string[] GetDirectories(string dir)
		{
			if (directories.ContainsKey(dir))
				return directories[dir];
			return new string[0];
		}
	}
}
