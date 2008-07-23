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
	using ICSharpCode.NRefactory.Ast;

	public interface ITypeResolver
	{
		void Clear();
		void AddTableEntry(string fullName);
		void UseNamespace(string ns);
		void UseNamespace(string ns, bool includeParents);
		void AliasNamespace(string alias, string ns);
		Type Resolve(string typeName, bool throwOnFail);
		Type Resolve(TypeReference reference, bool throwOnFail);
		string Resolve(string typeName);
		string Resolve(TypeReference reference);
	}
}