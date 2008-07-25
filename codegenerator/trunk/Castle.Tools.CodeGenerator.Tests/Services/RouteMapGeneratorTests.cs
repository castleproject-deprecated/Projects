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
	using NUnit.Framework;

	public class RouteMapGeneratorTests
	{
		protected ILogger logging;
		protected INamingService naming;
		protected ISourceGenerator source;
		protected RouteMapGenerator generator;

		[SetUp]
		public virtual void Setup()
		{
			naming = new DefaultNamingService();
			source = new DefaultSourceGenerator();
			logging = new NullLogger();
			generator = new RouteMapGenerator(logging, source, naming, "TargetNamespace", typeof (IServiceProvider).FullName);
		}
	}
}