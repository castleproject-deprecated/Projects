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

namespace Castle.Tools.CodeGenerator.Model
{
	using NUnit.Framework;
	using External;

	[TestFixture]
	public class MethodSignatureTests
	{
		[Test]
		public void Equals_SameNameSameTypes_Works()
		{
			var ms1 = new MethodSignature(GetType(), "Method", new[] {typeof (string)});
			var ms2 = new MethodSignature(GetType(), "Method", new[] {typeof (string)});
			Assert.IsTrue(ms1.Equals(ms2));
			Assert.AreEqual(ms1.GetHashCode(), ms2.GetHashCode());
			Assert.AreEqual(ms1.ToString(), ms2.ToString());
		}

		[Test]
		public void Equals_SameNameDifferentTypes_NotEqual()
		{
			var ms1 = new MethodSignature(GetType(), "Method", new[] {typeof (string)});
			var ms2 = new MethodSignature(GetType(), "Method", new[] {typeof (long)});
			Assert.IsFalse(ms1.Equals(ms2));
			Assert.AreNotEqual(ms1.GetHashCode(), ms2.GetHashCode());
			Assert.AreNotEqual(ms1.ToString(), ms2.ToString());
		}

		[Test]
		public void Equals_SameNameDifferentNumberOfTypes_NotEqual()
		{
			var ms1 = new MethodSignature(GetType(), "Method", new[] {typeof (string)});
			var ms2 = new MethodSignature(GetType(), "Method", new[] {typeof (string), typeof (string)});
			Assert.IsFalse(ms1.Equals(ms2));
			Assert.AreNotEqual(ms1.GetHashCode(), ms2.GetHashCode());
			Assert.AreNotEqual(ms1.ToString(), ms2.ToString());
		}

		[Test]
		public void Equals_DifferentNameSameTypes_NotEqual()
		{
			var ms1 = new MethodSignature(GetType(), "Method1", new[] {typeof (string)});
			var ms2 = new MethodSignature(GetType(), "Method2", new[] {typeof (string)});
			Assert.IsFalse(ms1.Equals(ms2));
			Assert.AreNotEqual(ms1.GetHashCode(), ms2.GetHashCode());
			Assert.AreNotEqual(ms1.ToString(), ms2.ToString());
		}

		[Test]
		public void Equals_SameEverythingButType_NotEqual()
		{
			var ms1 = new MethodSignature(GetType(), "Method", new[] {typeof (string)});
			var ms2 = new MethodSignature(typeof (string), "Method", new[] {typeof (string)});
			Assert.IsFalse(ms1.Equals(ms2));
			Assert.AreNotEqual(ms1.GetHashCode(), ms2.GetHashCode());
			Assert.AreNotEqual(ms1.ToString(), ms2.ToString());
		}
	}
}