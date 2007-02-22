using System;

using ICSharpCode.NRefactory.Parser.AST;

namespace Castle.Tools.CodeGenerator.Services
{
  public class TypeInspectionVisitor : TypeResolvingVisitor
  {
    #region TypeInspectionVisitor()
    public TypeInspectionVisitor(ITypeResolver typeResolver)
      : base(typeResolver)
    {
    }
    #endregion

    #region AbstractAstVisitor Members
    public override object Visit(TypeDeclaration typeDeclaration, object data)
    {
      string typeNamespace = GetNamespace(typeDeclaration);
      if (String.IsNullOrEmpty(typeNamespace))
        return base.Visit(typeDeclaration, data);

      string name = typeNamespace + "." + typeDeclaration.Name;
      this.TypeResolver.AddTableEntry(name);

      return base.Visit(typeDeclaration, data);
    }
    #endregion
  }
}