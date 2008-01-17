using System;
using ICSharpCode.NRefactory.Ast;

namespace Castle.Tools.CodeGenerator.Services.Visitors
{
	public class TypeInspectionVisitor : TypeResolvingVisitor
	{
		public TypeInspectionVisitor(ITypeResolver typeResolver)
			: base(typeResolver)
		{
		}
		
		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			string typeNamespace = GetNamespace(typeDeclaration);
			if (String.IsNullOrEmpty(typeNamespace))
				return base.VisitTypeDeclaration(typeDeclaration, data);

			string name = typeNamespace + "." + typeDeclaration.Name;
			TypeResolver.AddTableEntry(name);

			return base.VisitTypeDeclaration(typeDeclaration, data);
		}
	}
}