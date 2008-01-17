using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace Castle.Tools.CodeGenerator.Services.Visitors
{
	public class TypeResolvingVisitor : AbstractAstVisitor
	{
		private ITypeResolver _typeResolver;
		
		public ITypeResolver TypeResolver
		{
			get { return _typeResolver; }
		}
		
		public TypeResolvingVisitor(ITypeResolver typeResolver)
		{
			_typeResolver = typeResolver;
		}
		
		public override object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			foreach (Using usingLine in usingDeclaration.Usings)
			{
				if (usingLine.IsAlias)
					_typeResolver.AliasNamespace(usingLine.Name, usingLine.Alias.Type);
				else
					_typeResolver.UseNamespace(usingLine.Name);
			}
			return base.VisitUsingDeclaration(usingDeclaration, data);
		}


		public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			_typeResolver.UseNamespace(namespaceDeclaration.Name,true);
			return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
		}
		
		protected static string GetNamespace(TypeDeclaration typeDeclaration)
		{
			/*
		  INode iterator = typeDeclaration.Parent;
		  while (iterator != null)
		  {
			NamespaceDeclaration namespaceDeclaration = iterator as NamespaceDeclaration;
			if (namespaceDeclaration != null)
			  return namespaceDeclaration.Name;
			iterator = iterator.Parent;
		  }
		  return null;
		  */
			NamespaceDeclaration namespaceDeclaration = typeDeclaration.Parent as NamespaceDeclaration;
			if (namespaceDeclaration != null)
				return namespaceDeclaration.Name;
			return null;
		}
	}
}