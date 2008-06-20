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
	using DirectoryStructure;
	using Services;
	using NUnit.Framework;

	[TestFixture]
	public class BuildDirectoryStructureServiceFixture
	{
		[Test]
		public void Execute_Always_Works()
		{
			IFileSystemAdapter fileSystem = new MockFileSystemAdapter();
			IBuildDirectoryStructureService service = new BuildDirectoryStructureService(fileSystem);

			IDirInfo site = service.Execute("C:\\");

			Assert.AreEqual(2, site.Directories.Length);

			Assert.AreEqual("grandpa", site.Directories[0].Name);

			Assert.AreEqual("grandma", site.Directories[1].Name);

			string[] files = new string[]
			{
				@"script1.js" ,
				@"script2.js" ,
				@"style1.css",
				@"image1.jpg" ,
				@"image2.gif" 
			};

			foreach (string file in files)
			{
				Assert.IsTrue(IsDirContainsFile(site, file), "File [{0}] is missed", file);
			}

			Assert.IsFalse(IsDirContainsFile(site, "Boggey"));


		}

		private static bool IsDirContainsFile(IDirInfo dir, string fileName)
		{
			if (dir.Files.Exists(delegate(string file)
			{
				return file == fileName;
			}))
				return true;

			foreach (IDirInfo sub in dir.Directories)
			{
				if (IsDirContainsFile(sub, fileName))
					return true;
			}
			return false;
		}
	}
}