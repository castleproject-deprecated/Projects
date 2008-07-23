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
	using System.CodeDom;
	using NUnit.Framework;

	public static class CodeDomAssert
	{
		public static bool HasMemberOfTypeAndName(CodeTypeDeclaration typeDeclaration, Type type, string name)
		{
			foreach (CodeTypeMember member in typeDeclaration.Members)
				if (type.IsInstanceOfType(member) && member.Name == name)
					return true;
			
			return false;
		}

		public static void AssertHasProperty(CodeTypeDeclaration typeDeclaration, string name)
		{
			Assert.IsTrue(HasMemberOfTypeAndName(typeDeclaration, typeof (CodeMemberProperty), name), "Expected proprety: " + name);
		}

		public static void AssertHasField(CodeTypeDeclaration typeDeclaration, string name)
		{
			Assert.IsTrue(HasMemberOfTypeAndName(typeDeclaration, typeof (CodeMemberField), name), "Expected field: " + name);
		}

		public static void AssertHasMethod(CodeTypeDeclaration typeDeclaration, string name)
		{
			Assert.IsTrue(HasMemberOfTypeAndName(typeDeclaration, typeof (CodeMemberMethod), name), "Expected method: " + name);
		}

		public static void AssertNotHasProperty(CodeTypeDeclaration typeDeclaration, string name)
		{
			Assert.IsFalse(HasMemberOfTypeAndName(typeDeclaration, typeof (CodeMemberProperty), name), "Unexpected proprety: " + name);
		}

		public static void AssertNotHasField(CodeTypeDeclaration typeDeclaration, string name)
		{
			Assert.IsFalse(HasMemberOfTypeAndName(typeDeclaration, typeof (CodeMemberField), name), "Unexpected field: " + name);
		}

		public static void AssertNotHasMethod(CodeTypeDeclaration typeDeclaration, string name)
		{
			Assert.IsFalse(HasMemberOfTypeAndName(typeDeclaration, typeof (CodeMemberMethod), name), "Unexpected method: " + name);
		}

		public static CodeTypeDeclaration AssertHasType(CodeCompileUnit ccu, string name)
		{
			foreach (CodeNamespace ns in ccu.Namespaces)
				foreach (CodeTypeDeclaration type in ns.Types)
					if (type.Name == name)
						return type;
			
			Assert.Fail("Expected type: " + name);
			
			return null;
		}
	}
}