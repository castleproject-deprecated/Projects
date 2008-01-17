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
	using NUnit.Framework;

	[TestFixture]
	public class ResourceDirInfoFixture
	{
		ResourceDirInfo site;
		ResourceDirInfo son1;
		ResourceDirInfo son2;
		ResourceDirInfo grandson;

		[SetUp]
		public void SetUp()
		{
			site = new ResourceDirInfo("/");
			son1 = new ResourceDirInfo("/son1/");
			son2 = new ResourceDirInfo("/son2/");
			grandson = new ResourceDirInfo("/son1/grandson/");

			site.AddSubDirectory(son1);
			site.AddSubDirectory(son2);
			son1.AddSubDirectory(grandson);
		}

		[Test]
		public void RemoveEmptyDirectories_WhenAllHasFiles_DoesNothing()
		{
			site.HasFiles = true;
			son1.HasFiles = true;
			son2.HasFiles = true;
			grandson.HasFiles = true;

			site.RemoveEmptyDirectories();

			Assert.AreEqual(2, site.Directories.Length);
			Assert.AreEqual(1, son1.Directories.Length);
		}

		[Test]
		public void RemoveEmptyDirectories_WhenHasEmptyLeafs_RemovesLeafs()
		{
			site.HasFiles = true;
			son1.HasFiles = true;
			son2.HasFiles = false;
			grandson.HasFiles = false;

			site.RemoveEmptyDirectories();

			Assert.AreEqual(1, site.Directories.Length);
			Assert.AreEqual(son1, site.Directories[0]);
			Assert.AreEqual(0, son1.Directories.Length);
		}

		[Test]
		public void RemoveEmptyDirectories_WhenHasEmptyNodes_RemovesNodes()
		{
			site.HasFiles = true;
			son1.HasFiles = false;
			son2.HasFiles = true;
			grandson.HasFiles = false;

			site.RemoveEmptyDirectories();

			Assert.AreEqual(1, site.Directories.Length);
			Assert.AreEqual(son2, site.Directories[0]);
			Assert.AreEqual(0, son2.Directories.Length);
		}

		[Test]
		public void SetTrue_Always_SetAnccestors()
		{
			grandson.HasFiles = true;

			Assert.IsTrue(grandson.HasFiles);
			Assert.IsTrue(son1.HasFiles);
			Assert.IsTrue(site.HasFiles);
			Assert.IsFalse(son2.HasFiles);
		}
	}
}
