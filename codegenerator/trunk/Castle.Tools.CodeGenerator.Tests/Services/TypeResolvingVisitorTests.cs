namespace Castle.Tools.CodeGenerator.Services
{
	using System;
	using System.Collections.Generic;

	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	using Rhino.Mocks;

	[TestFixture]
	public class TypeResolvingVisitorTests
	{
		[Test]
		public void VisitingNamespace_MustAddParentNamespacesToTypeResolver()
		{
			List<TypeTableEntry> typeTables = new List<TypeTableEntry>();
			List<String> usings = new List<String>();
			Dictionary<String, String> aliases = new Dictionary<string, string>();

			TypeResolver resolver = new TypeResolver(typeTables,usings,aliases);
						
			NamespaceDeclaration nsDecl = new NamespaceDeclaration("System.Collections.Specialized");
			
			TypeResolvingVisitor visitor = new TypeResolvingVisitor(resolver);
			
			visitor.VisitNamespaceDeclaration(nsDecl, null);

			Assert.AreEqual(3, usings.Count);
			
		}
	}
}
