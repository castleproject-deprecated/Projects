using System;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface ITypeResolver
  {
    void Clear();
    void AddTableEntry(string fullName);
    void UseNamespace(string ns);
    void AliasNamespace(string alias, string ns);
    Type Resolve(string typeName, bool throwOnFail);
    string Resolve(string typeName);
  }
}