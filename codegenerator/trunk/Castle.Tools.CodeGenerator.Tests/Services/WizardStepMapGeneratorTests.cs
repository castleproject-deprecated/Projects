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
	using Model.TreeNodes;
	using Generators;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class WizardStepMapGeneratorTests
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
			source = new DefaultSourceGenerator();
			logging = new NullLogger();
			generator = new ActionMapGenerator(logging, source, naming, "TargetNamespace", typeof (IServiceProvider).FullName);
		}

		[Test]
		public void VisitControllerNode_Always_CreatesType()
		{
			var node = new WizardControllerTreeNode("HomeController", "ControllerNamespace", new[] {"Step1"});

			mocks.ReplayAll();
			generator.Visit(node);
			mocks.VerifyAll();

			CodeDomAssert.AssertHasField(source.Ccu.Namespaces[0].Types[0], "_services");
		}
	}
}