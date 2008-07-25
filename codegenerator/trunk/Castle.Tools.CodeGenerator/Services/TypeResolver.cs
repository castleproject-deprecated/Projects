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
	using System.Reflection;
	using System.Web;
	using ICSharpCode.NRefactory.Ast;

	public class TypeResolver : ITypeResolver
	{
		private static readonly Dictionary<string, Type> primitiveTypes = new Dictionary<string, Type>();

		private readonly Dictionary<string, string> aliases = new Dictionary<string, string>();
		private readonly List<TypeTableEntry> typeEntries = new List<TypeTableEntry>();
		private readonly List<string> usings = new List<string>();
		
		static TypeResolver()
		{
			primitiveTypes["int"] = typeof (int);
			primitiveTypes["int?"] = typeof (int?);
			primitiveTypes["short"] = typeof (short);
			primitiveTypes["short?"] = typeof (short?);
			primitiveTypes["long"] = typeof (long);
			primitiveTypes["long?"] = typeof (long?);
			primitiveTypes["string"] = typeof (string);
			primitiveTypes["char"] = typeof (char);
			primitiveTypes["char?"] = typeof (char?);
			primitiveTypes["uint"] = typeof (uint);
			primitiveTypes["uint?"] = typeof (uint?);
		}

		public TypeResolver()
		{
		}

		public TypeResolver(List<TypeTableEntry> typeEntries, List<string> usings, Dictionary<string, string> aliases)
		{
			this.typeEntries = typeEntries;
			this.usings = usings;
			this.aliases = aliases;
		}

		public void AddTableEntry(string fullName)
		{
			AddTableEntry(new TypeTableEntry(fullName));
		}

		public void AddTableEntry(TypeTableEntry entry)
		{
			typeEntries.Add(entry);
		}

		public void Clear()
		{
			usings.Clear();
			aliases.Clear();
		}

		public void UseNamespace(string ns, bool includeParents)
		{
			UseNamespace(ns);

			if (!includeParents) return;

			int index;
			while ((index = ns.LastIndexOf('.')) >= 0)
			{
				ns = ns.Substring(0, index);
				UseNamespace(ns);
			}
		}

		public void UseNamespace(string ns)
		{
			if (!usings.Contains(ns))
				usings.Add(ns);
		}

		public void AliasNamespace(string alias, string ns)
		{
			aliases[alias] = ns;
		}

		public string Resolve(string typeName)
		{
			foreach (var ns in usings)
			{
				var typePath = ns + "." + typeName;
				
				foreach (var entry in typeEntries)
					if (entry.FullName == typePath)
						return entry.FullName;
			}

			return null;
		}

		protected static IEnumerable<Assembly> GetAssemblies()
		{
			yield return typeof (HttpPostedFile).Assembly;

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				yield return assembly;
		}

		private static IEnumerable<Assembly> NonSystemAndCastleAssemblies()
		{
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
				if ((!asm.FullName.StartsWith("Castle")) && (!asm.FullName.StartsWith("System")))
					yield return asm;
		}

		public Type Resolve(string typeName, bool throwOnFail)
		{
			/* Resolve the standard primitive types using our map. */
			if (primitiveTypes.ContainsKey(typeName))
				return primitiveTypes[typeName];

			/* See if we can resolve this as a standard, qualified type name. */
			var type = Type.GetType(typeName, false);
			if (type != null)
				return type;

			foreach (var asm in NonSystemAndCastleAssemblies())
			{
				foreach (var ns in usings)
				{
					var typePath = ns + "." + typeName;
					type = asm.GetType(typePath);
					
					if (type == null) continue;

					AddTableEntry(typePath);
					return type;
				}
			}
			
			foreach (var assembly in GetAssemblies())
			{
				type = assembly.GetType(typeName);
				
				if (type != null)
					return type;
			}

			/* Attempt to qualify the type into the given namespaces that have been
			 * used in this module. */
			foreach (var ns in usings)
			{
				var typePath = ns + "." + typeName;
				type = FindType(typePath);

				if (type != null)
					return type;
			}

			/* We must be able to resolve ALL possible types. */
			if (throwOnFail)
				throw new TypeLoadException(String.Format("Unable to resolve: {0}", typeName));

			return null;
		}

		public string Resolve(TypeReference reference)
		{
			return ResolveTypeReference(reference).ToString();
		}

		public TypeReference ResolveTypeReference(TypeReference reference)
		{
			var temp = reference.SystemType;
			
			if (reference.GenericTypes.Count > 0)
				temp = temp + "`" + reference.GenericTypes.Count;
			
			var resolvedSystemType = HighLevelResolve(temp);
			var resolvedGenerics = new List<TypeReference>();
			
			foreach (var genericType in reference.GenericTypes)
			{
				var resolvedGenericArgument = ResolveTypeReference(genericType);
				resolvedGenerics.Add(resolvedGenericArgument);
			}

			return new TypeReference(resolvedSystemType, reference.PointerNestingLevel, reference.RankSpecifier, resolvedGenerics);
		}

		protected string HighLevelResolve(string name)
		{
			return Resolve(name) ?? Resolve(name, true).FullName;
		}

		protected static Type FindType(string name)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					foreach (var type in assembly.GetTypes())
						if (type.FullName == name)
							return type;
				}
				catch (ReflectionTypeLoadException)
				{
				}
			}

			return null;
		}
	}
}