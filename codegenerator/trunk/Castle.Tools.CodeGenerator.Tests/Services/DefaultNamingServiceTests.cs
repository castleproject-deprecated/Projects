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
	public class NamingServiceTests
	{
		private DefaultNamingService service;

		[SetUp]
		public void Setup()
		{
			service = new DefaultNamingService();
		}

		[Test]
		public void ToVariableName_Always_LowercasesFirstLetter()
		{
			Assert.AreEqual("helloWorld", service.ToVariableName("HelloWorld"));
			Assert.AreEqual("helloWorld", service.ToVariableName("helloWorld"));
		}

		[Test]
		public void ToMemberVariableName_Always_LowercasesFirstLetterAndsPrefixes()
		{
			Assert.AreEqual("_helloWorld", service.ToMemberVariableName("HelloWorld"));
			Assert.AreEqual("_helloWorld", service.ToMemberVariableName("helloWorld"));
		}

		[Test]
		public void ToPropertyName_Always_UppercasesFirstLetter()
		{
			Assert.AreEqual("HelloWorld", service.ToPropertyName("HelloWorld"));
			Assert.AreEqual("HelloWorld", service.ToPropertyName("helloWorld"));
		}

		[Test]
		public void ToClassName_Always_UppercasesFirstLetter()
		{
			Assert.AreEqual("HelloWorld", service.ToClassName("HelloWorld"));
			Assert.AreEqual("HelloWorld", service.ToClassName("helloWorld"));
		}

		[Test]
		public void ToControllerName_Always_StripsController()
		{
			Assert.AreEqual("HelloWorld", service.ToControllerName("HelloWorldController"));
			Assert.AreEqual("HelloWorld", service.ToControllerName("HelloWorld"));
		}

		[Test]
		public void ToAreaWrapperName_Always_AppendsAreaNode()
		{
			Assert.AreEqual("HelloWorldAreaNode", service.ToAreaWrapperName("HelloWorld"));
		}

		[Test]
		public void ToActionWrapperName_Always_AppendsAreaNode()
		{
			Assert.AreEqual("HelloWorldActionNode", service.ToActionWrapperName("HelloWorld"));
		}

		[Test]
		public void ToViewWrapperName_Always_AppendsAreaNode()
		{
			Assert.AreEqual("HelloWorldViewNode", service.ToViewWrapperName("HelloWorld"));
		}

		[Test]
		public void ToControllerWrapperName_Always_AppendsNode()
		{
			Assert.AreEqual("HelloWorldControllerNode", service.ToControllerWrapperName("HelloWorldController"));
		}

		[Test]
		public void ToMemberSignature_SameName_AreDifferent()
		{
			var name1 = service.ToMethodSignatureName("Method", new[] {typeof (string)});
			var name2 = service.ToMethodSignatureName("Method", new[] {typeof (long)});
			Assert.AreNotEqual(name1, name2);
		}

		[Test]
		public void ToMemberSignature_SameTypes_AreSame()
		{
			var name1 = service.ToMethodSignatureName("Method", new[] {"string"});
			var name2 = service.ToMethodSignatureName("Method", new[] {"string"});
			Assert.AreEqual(name1, name2);
		}

		[Test]
		public void ToMemberSignature_SameTypesDifferentName_AreDifferent()
		{
			var name1 = service.ToMethodSignatureName("Method", new[] {typeof (string)});
			var name2 = service.ToMethodSignatureName("OtherMethod", new[] {typeof (string)});
			Assert.AreNotEqual(name1, name2);
		}

		[Test]
		public void ToMemberSignature_ArrayTypes_ArentStupid()
		{
			var name = service.ToMethodSignatureName("Method", new[] {typeof (string[])});
			Assert.AreEqual("Method_StringBB", name);
		}
	}
}