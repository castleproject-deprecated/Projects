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
	using ICSharpCode.NRefactory.Ast;
	using NUnit.Framework;

	[TestFixture]
	public class TypeResolverTests
	{
		private TypeResolver typeResolver;

		[SetUp]
		public void Setup()
		{
			typeResolver = new TypeResolver();
		}

		[Test]
		public void Resolve_Integer_Works()
		{
			Assert.AreEqual(typeof (int), typeResolver.Resolve("int", true));
		}

		[Test]
		public void Resolve_Long_Works()
		{
			Assert.AreEqual(typeof (long), typeResolver.Resolve("long", true));
		}

		[Test]
		public void Resolve_Short_Works()
		{
			Assert.AreEqual(typeof (short), typeResolver.Resolve("short", true));
		}

		[Test]
		public void Resolve_Character_Works()
		{
			Assert.AreEqual(typeof (char), typeResolver.Resolve("char", true));
		}

		[Test]
		public void Resolve_String_Works()
		{
			Assert.AreEqual(typeof (string), typeResolver.Resolve("string", true));
		}

		[Test]
		public void Resolve_FromUsing_Works()
		{
			typeResolver.UseNamespace("System");
			Assert.AreEqual(typeof (DateTime), typeResolver.Resolve("DateTime", true));
		}

		[Test]
		[ExpectedException(typeof (TypeLoadException))]
		public void Resolve_MissingType_Throws()
		{
			typeResolver.UseNamespace("System.Collections");
			typeResolver.Resolve("DateTime", true);
		}

		[Test]
		public void Resolve_WithOnlyChildNamespaceButIncludeParentsTrue_Works()
		{
			typeResolver.UseNamespace("System.Collections", true);
			Assert.AreEqual(typeof (DateTime), typeResolver.Resolve("DateTime", true));
		}

		[Test]
		public void Resolve_WithOnlyChildNamespaceButIncludeParentsMoreThanOneLevelAway_Works()
		{
			typeResolver.UseNamespace("System.Collections.Generic", true);
			Assert.AreEqual(typeof (System.Collections.ArrayList), typeResolver.Resolve("ArrayList", true));
		}

		[Test]
		public void Resolve_MissingTypeNoThrow_ReturnsNull()
		{
			Assert.IsNull(typeResolver.Resolve("DateTime", false));
		}

		[Test]
		[ExpectedException(typeof (TypeLoadException))]
		public void Clear_Always_RemovesEntries()
		{
			typeResolver.UseNamespace("System");
			typeResolver.Clear();
			typeResolver.Resolve("DateTime", true);
		}

		[Test]
		public void Resolve_TypeTableEntry_Works()
		{
			typeResolver.AddTableEntry("Eleutian.Namespace.Type");
			typeResolver.UseNamespace("Eleutian.Namespace");
			Assert.AreEqual("Eleutian.Namespace.Type", typeResolver.Resolve("Type"));
		}

		[Test]
		public void Resolve_MissingTypeTableEntry_ReturnsNull()
		{
			typeResolver.AddTableEntry(new TypeTableEntry("Eleutian.Namespace.AnotherType"));
			typeResolver.UseNamespace("Eleutian.Namespace");
			Assert.IsNull(typeResolver.Resolve("Type"));
		}

		[Test]
		public void Resolve_AbsoluteQualified_Works()
		{
			Assert.IsNotNull(typeResolver.Resolve("System.DateTime", false));
		}

		[Test]
		public void AliasNamespace_Always_AddsAlias()
		{
			typeResolver.AliasNamespace("Bob", "System");
			typeResolver.Resolve("DateTime");
		}

		[Test]
		public void ResolveStringOnly_ArrayOfDateTimes_Fails()
		{
			typeResolver.UseNamespace("System");
			Assert.IsNull(typeResolver.Resolve("DateTime[]"));
		}

		[Test]
		public void Resolve_ArrayOfDateTimes_Works()
		{
			typeResolver.UseNamespace("System");
			Assert.IsNotNull(typeResolver.Resolve("DateTime[]", true));
		}

		[Test]
		public void ResolveTypeReference_Simple_Works()
		{
			var reference = new TypeReference("DateTime");

			typeResolver.UseNamespace("System");

			Assert.AreEqual("DateTime", reference.ToString());
			Assert.AreEqual("System.DateTime", typeResolver.Resolve(reference));
		}

		[Test]
		public void ResolveTypeReference_SimplePrimitiveTypeArray_Works()
		{
			var reference = new TypeReference("long", new[] {0});

			typeResolver.UseNamespace("System");

			Assert.AreEqual("long[]", reference.ToString());
			Assert.AreEqual("System.Int64[]", typeResolver.Resolve(reference));
		}

		[Test]
		public void ResolveTypeReference_SimpleArray_Works()
		{
			var reference = new TypeReference("DateTime", new[] {0});

			typeResolver.UseNamespace("System");

			Assert.AreEqual("DateTime[]", reference.ToString());
			Assert.AreEqual("System.DateTime[]", typeResolver.Resolve(reference));
		}

		[Test]
		public void ResolveTypeReference_TwoDimArray_Works()
		{
			var reference = new TypeReference("DateTime", new[] {0, 0});

			typeResolver.UseNamespace("System");

			Assert.AreEqual("DateTime[][]", reference.ToString());
			Assert.AreEqual("System.DateTime[][]", typeResolver.Resolve(reference));
		}

		[Test]
		public void ResolveTypeReference_ArrayOfSourceType_Works()
		{
			var reference = new TypeReference("RadClass", new[] {0, 0});

			typeResolver.AddTableEntry("SomeNamespace.Utility.RadClass");

			typeResolver.UseNamespace("System");
			typeResolver.UseNamespace("SomeNamespace.Utility");

			Assert.AreEqual("RadClass[][]", reference.ToString());
			Assert.AreEqual("SomeNamespace.Utility.RadClass[][]", typeResolver.Resolve(reference));
		}

		[Test]
		public void ResolveTypeReference_GenericList_Works()
		{
			var reference = new TypeReference("List", new List<TypeReference>(new[] {new TypeReference("string")}));
			typeResolver.UseNamespace("System");
			typeResolver.UseNamespace("System.Collections.Generic");

			Assert.AreEqual("List<string>", reference.ToString());
		}

		[Test]
		public void ResolveTypeReference_GenericDictionary_Works()
		{
			var reference = new TypeReference("Dictionary", new List<TypeReference>(new[] {new TypeReference("string"), new TypeReference("DateTime")}));
			typeResolver.UseNamespace("System");
			typeResolver.UseNamespace("System.Collections.Generic");

			Assert.AreEqual("Dictionary<string,DateTime>", reference.ToString());
		}

		[Test]
		public void Resolve_Enumeration_Works()
		{
			typeResolver.UseNamespace("System");
			Assert.IsNull(typeResolver.Resolve("DayOfWeek"));
		}
	}
}