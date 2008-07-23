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
	using System;
	using NUnit.Framework;
	using External;

	[TestFixture]
	public class DefaultRuntimeInformationServiceTests
	{
		private DefaultRuntimeInformationService service;

		[SetUp]
		public void Setup()
		{
			service = new DefaultRuntimeInformationService();
		}

		[Test]
		[ExpectedException(typeof (MissingMethodException))]
		public void ResolveMethodInformation_BadMethodName_Throws()
		{
			service.ResolveMethodInformation(typeof (RuntimeType), "BadMethod", new Type[0]);
		}

		[Test]
		[ExpectedException(typeof (MissingMethodException))]
		public void ResolveMethodInformation_BadMethodSignature_Throws()
		{
			service.ResolveMethodInformation(typeof (RuntimeType), "MyMethod", new Type[0]);
		}

		[Test]
		public void ResolveMethodInformation_GoodSignature_ReturnsInformation()
		{
			var information = service.ResolveMethodInformation(typeof (RuntimeType), "MyMethod", new[] {typeof (string)});
			Assert.AreEqual("MyMethod", information.Method.Name);
			Assert.AreEqual(1, information.Parameters.Length);
			Assert.AreEqual("name", information.Parameters[0].Parameter.Name);
			Assert.AreEqual(1, information.Parameters[0].CustomAttributes.Length);
			Assert.AreEqual(1, information.CustomAttributes.Length);
		}

		[Test]
		public void GetCustomAttributes_OfType_Works()
		{
			var information = service.ResolveMethodInformation(typeof (RuntimeType), "MyMethod", new[] {typeof (string)});
			Assert.AreEqual(1, information.GetCustomAttributes(typeof (ExampleTestAttribute)).Length);
		}

		[Test]
		public void ResolveMethodInformation_SecondTime_ReturnsSameInformation()
		{
			var information1 = service.ResolveMethodInformation(typeof (RuntimeType), "MyMethod", new[] {typeof (string)});
			var information2 = service.ResolveMethodInformation(typeof (RuntimeType), "MyMethod", new[] {typeof (string)});
			Assert.AreSame(information1, information2);
		}
	}

	public class RuntimeType
	{
		[ExampleTest]
		public void MyMethod([ExampleTest] string name)
		{
		}
	}

	public class ExampleTestAttribute : Attribute
	{
	}
}