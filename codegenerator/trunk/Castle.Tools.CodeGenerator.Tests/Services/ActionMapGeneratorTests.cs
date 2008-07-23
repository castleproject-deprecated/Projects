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
	using Generators;
	using Rhino.Mocks;
	using NUnit.Framework;

	public class ActionMapGeneratorTests
	{
		protected MockRepository mocks;
		protected ILogger logging;
		protected INamingService naming;
		protected ISourceGenerator source;
		protected ActionMapGenerator generator;

		[SetUp]
		public virtual void Setup()
		{
			mocks = new MockRepository();
			naming = new DefaultNamingService();
			source = new DefaultSourceGenerator(); // I found a more integration style of testing was better, I started off
			// mocking calls to ISourceGenerator, and that was just stupid, we want the classes and types and members.
			// and the assertions here ensure that.
			logging = new NullLogger();
			generator = new ActionMapGenerator(logging, source, naming, "TargetNamespace", typeof (IServiceProvider).FullName);
		}
	}
}