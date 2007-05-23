using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace Castle.Tools.CodeGenerator.Services
{
  public class TypeResolvingVisitor : AbstractAstVisitor
  {
    #region Member Data
    private ITypeResolver _typeResolver;
    #endregion

    #region Properties
    public ITypeResolver TypeResolver
    {
      get { return _typeResolver; }
    }
    #endregion

    #region TypeResolvingVisitor()
    public TypeResolvingVisitor(ITypeResolver typeResolver)
    {
      _typeResolver = typeResolver;
    }
    #endregion

    #region AbstractAstVisitor Members
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
		_typeResolver.UseNamespace(namespaceDeclaration.Name);
		return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
  	}
    #endregion

    #region Private Members
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
    #endregion
  }
}
