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

namespace Castle.Tools.CodeGenerator.Services.Visitors
{
	using ICSharpCode.NRefactory.Ast;
	using ICSharpCode.NRefactory.Visitors;

	public class TypeResolvingVisitor : AbstractAstVisitor
	{
		public TypeResolvingVisitor(ITypeResolver typeResolver)
		{
			TypeResolver = typeResolver;
		}

		public ITypeResolver TypeResolver { get; private set; }

		public override object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			foreach (var usingLine in usingDeclaration.Usings)
				if (usingLine.IsAlias)
					TypeResolver.AliasNamespace(usingLine.Name, usingLine.Alias.Type);
				else
					TypeResolver.UseNamespace(usingLine.Name);
			
			return base.VisitUsingDeclaration(usingDeclaration, data);
		}


		public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			TypeResolver.UseNamespace(namespaceDeclaration.Name, true);

			return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
		}

		protected static string GetNamespace(TypeDeclaration typeDeclaration)
		{
			var namespaceDeclaration = typeDeclaration.Parent as NamespaceDeclaration;
			
			return namespaceDeclaration != null ? namespaceDeclaration.Name : null;
		}
	}
}