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
	using System.Collections.Generic;
	using Visitors;
	using ICSharpCode.NRefactory.Ast;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class TypeInspectionVisitorTests
	{
		private MockRepository mocks;
		private ITypeResolver typeResolver;
		private TypeInspectionVisitor visitor;
		private NamespaceDeclaration @namespace;
		private TypeDeclaration type;

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
			typeResolver = mocks.DynamicMock<ITypeResolver>();
			visitor = new TypeInspectionVisitor(typeResolver);

			type = new TypeDeclaration(Modifiers.Public, new List<AttributeSection>()) {Name = "SomeType"};
			@namespace = new NamespaceDeclaration("SomeNamespace");
			@namespace.AddChild(type);
			type.Parent = @namespace;
		}

		[Test]
		public void VisitTypeDeclaration_NoNamespace_DoesNothing()
		{
			type.Parent = null;
			@namespace.Children.Clear();

			mocks.ReplayAll();
			visitor.VisitTypeDeclaration(type, null);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitNamespaceDeclaration_Always_AddsToUsing()
		{
			type.Parent = null;
			@namespace.Children.Clear();

			using (mocks.Unordered())
				typeResolver.UseNamespace("SomeNamespace", true);
			
			mocks.ReplayAll();
			visitor.VisitNamespaceDeclaration(@namespace, null);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitTypeDeclaration_WithNamespace_AddsTableEntry()
		{
			using (mocks.Unordered())
				typeResolver.AddTableEntry("SomeNamespace.SomeType");
			
			mocks.ReplayAll();
			visitor.VisitTypeDeclaration(type, null);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitTypeDeclaration_NonNamespaceParent_Ignores()
		{
			var childType = new TypeDeclaration(Modifiers.Public, new List<AttributeSection>()) {Parent = type};

			using (mocks.Unordered())
			{
			}

			mocks.ReplayAll();
			visitor.VisitTypeDeclaration(childType, null);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitUsingDeclaration_NonAlias_AddsToUsing()
		{
			var usings = new UsingDeclaration("System");
			usings.Usings.Add(new Using("System.Collections.Generic"));

			using (mocks.Unordered())
			{
				typeResolver.UseNamespace("System");
				typeResolver.UseNamespace("System.Collections.Generic");
			}

			mocks.ReplayAll();
			visitor.VisitUsingDeclaration(usings, null);
			mocks.VerifyAll();
		}

		[Test]
		public void VisitUsingDeclaration_Alias_AddsToUsing()
		{
			var usings = new UsingDeclaration("System");
			usings.Usings.Add(new Using("Bob", new TypeReference("System")));

			using (mocks.Unordered())
			{
				typeResolver.UseNamespace("System");
				typeResolver.AliasNamespace("Bob", "System");
			}

			mocks.ReplayAll();
			visitor.VisitUsingDeclaration(usings, null);
			mocks.VerifyAll();
		}
	}
}