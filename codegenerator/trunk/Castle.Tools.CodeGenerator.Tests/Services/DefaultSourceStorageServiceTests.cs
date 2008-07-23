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
	using System.Collections.Generic;
	using ICSharpCode.NRefactory;
	using Rhino.Mocks;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultSourceStorageServiceTests
	{
		private MockRepository mocks;
		private DefaultSourceStorageService service;

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
			service = new DefaultSourceStorageService();
		}

		[Test]
		[ExpectedException(typeof (KeyNotFoundException))]
		public void GetParsedSource_NoSource_Throws()
		{
			service.GetParsedSource("Source.cs");
		}

		[Test]
		public void Add_NewSource_Works()
		{
			service.Add("Source.cs", mocks.DynamicMock<IParser>());
			Assert.IsNotNull(service.GetParsedSource("Source.cs"));
		}

		[Test]
		public void Add_TwpSources_Works()
		{
			service.Add("Source1.cs", mocks.DynamicMock<IParser>());
			service.Add("Source2.cs", mocks.DynamicMock<IParser>());
			Assert.IsNotNull(service.GetParsedSource("Source1.cs"));
			Assert.IsNotNull(service.GetParsedSource("Source2.cs"));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void Add_DuplicateSource_Throws()
		{
			service.Add("Source.cs", mocks.DynamicMock<IParser>());
			service.Add("Source.cs", mocks.DynamicMock<IParser>());
		}
	}
}