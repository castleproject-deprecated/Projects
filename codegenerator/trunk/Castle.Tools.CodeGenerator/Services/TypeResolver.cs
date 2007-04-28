using System;
using System.Collections.Generic;
using System.Reflection;
using ICSharpCode.NRefactory.Parser.AST;

namespace Castle.Tools.CodeGenerator.Services
{
  public class TypeResolver : ITypeResolver
  {
    #region Member Data
    private static Dictionary<string, Type> _primitiveTypes = new Dictionary<string, Type>();
    private List<TypeTableEntry> _typeEntries = new List<TypeTableEntry>();
    private List<string> _usings = new List<string>();
    private Dictionary<string, string> _aliases = new Dictionary<string, string>();
    #endregion

    #region TypeResolver()
    static TypeResolver()
    {
      _primitiveTypes["int"] = typeof(int);
      _primitiveTypes["int?"] = typeof(int?);
      _primitiveTypes["short"] = typeof(short);
      _primitiveTypes["short?"] = typeof(short?);
      _primitiveTypes["long"] = typeof(long);
      _primitiveTypes["long?"] = typeof(long?);
      _primitiveTypes["string"] = typeof(string);
      _primitiveTypes["char"] = typeof(char);
      _primitiveTypes["char?"] = typeof(char?);
      _primitiveTypes["uint"] = typeof(uint);
      _primitiveTypes["uint?"] = typeof(uint?);
    }
    #endregion

    #region Methods
    public void AddTableEntry(string fullName)
    {
      AddTableEntry(new TypeTableEntry(fullName));
    }

    public void AddTableEntry(TypeTableEntry entry)
    {
      _typeEntries.Add(entry);
    }

    public void Clear()
    {
      _usings.Clear();
      _aliases.Clear();
    }

    public void UseNamespace(string ns, bool includeParents)
    {
      UseNamespace(ns);

      if (includeParents)
      {
        int index;
        while ((index = ns.IndexOf('.')) >= 0)
        {
          ns = ns.Substring(0, index);
          UseNamespace(ns);
        }
      }
    }

    public void UseNamespace(string ns)
    {
      if (!_usings.Contains(ns))
      {
        _usings.Add(ns);
      }
    }

    public void AliasNamespace(string alias, string ns)
    {
      _aliases[alias] = ns;
    }

    public string Resolve(string typeName)
    {
      foreach (string ns in _usings)
      {
        string typePath = ns + "." + typeName;
        foreach (TypeTableEntry entry in _typeEntries)
        {
          if (entry.FullName == typePath)
            return entry.FullName;
        }
      }
      return null;
    }

    protected static IEnumerable<Assembly> GetAssemblies()
    {
      yield return typeof(System.Web.HttpPostedFile).Assembly;
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        yield return assembly;
      }
    }

    private IEnumerable<Assembly> NonSystemAndCastleAssemblies()
    {
      foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
      {
        if ((!asm.FullName.StartsWith("Castle")) && (!asm.FullName.StartsWith("System")))
        {
          yield return asm;
        }
      }
    }

    public Type Resolve(string typeName, bool throwOnFail)
    {
      /* Resolve the standard primitive types using our map. */
      if (_primitiveTypes.ContainsKey(typeName))
        return _primitiveTypes[typeName];

      /* See if we can resolve this as a standard, qualified type name. */
      Type type = Type.GetType(typeName, false);
      if (type != null)
        return type;

      foreach (Assembly asm in NonSystemAndCastleAssemblies())
      {
        foreach (String ns in _usings)
        {
          string typePath = ns + "." + typeName;
          type = asm.GetType(typePath);
          if (type != null)
          {
            AddTableEntry(typePath);
            return type;
          }
        }
      }
      foreach (Assembly assembly in GetAssemblies())
      {
        type = assembly.GetType(typeName);
        if (type != null)
          return type;
      }

      /* Attempt to qualify the type into the given namespaces that have been
       * used in this module. */
      foreach (string ns in _usings)
      {
        string typePath = ns + "." + typeName;

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
      string temp = reference.SystemType;
      if (reference.GenericTypes.Count > 0)
      {
        temp = temp + "`" + reference.GenericTypes.Count;
      }
      string resolvedSystemType = HighLevelResolve(temp);

      List<TypeReference> resolvedGenerics = new List<TypeReference>();
      foreach (TypeReference genericType in reference.GenericTypes)
      {
        TypeReference resolvedGenericArgument = ResolveTypeReference(genericType);
        resolvedGenerics.Add(resolvedGenericArgument);
      }

      return new TypeReference(resolvedSystemType, reference.PointerNestingLevel, reference.RankSpecifier, resolvedGenerics);
    }
    #endregion

    #region Methods
    protected string HighLevelResolve(string name)
    {
      string baseName = Resolve(name);
      if (baseName == null)
      {
        return Resolve(name, true).FullName;
      }
      return baseName;
    }

    protected static Type FindType(string name)
    {
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        try
        {
          foreach (Type type in assembly.GetTypes())
          {
            if (type.FullName == name)
              return type;
          }
        }
        catch (ReflectionTypeLoadException)
        {
        }
      }
      return null;
    }
    #endregion
  }
}