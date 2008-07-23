// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Tools.CodeGenerator.Services
{
	using NUnit.Framework;

	[TestFixture]
	public class TypeTableEntryTests
	{
		[Test]
		public void Constructor_Initializes_Always()
		{
			var entry = new TypeTableEntry("System.String", typeof (string));
			Assert.AreEqual(typeof (string), entry.CompiledType);
			Assert.AreEqual("System.String", entry.FullName);
			Assert.AreEqual("String", entry.Name);
			Assert.IsTrue(entry.HasCompiledType);
		}

		[Test]
		public void NonTypedConstructor_Initializes_Always()
		{
			var entry = new TypeTableEntry("System.String");
			Assert.IsNull(entry.CompiledType);
			Assert.AreEqual("System.String", entry.FullName);
			Assert.AreEqual("String", entry.Name);
			Assert.IsFalse(entry.HasCompiledType);
		}
	}
}