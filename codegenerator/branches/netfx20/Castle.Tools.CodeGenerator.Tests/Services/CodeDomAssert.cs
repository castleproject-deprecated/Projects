using System;
using System.CodeDom;
using System.Collections.Generic;

using Rhino.Mocks;
using NUnit.Framework;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public static class CodeDomAssert
  {
    public static bool HasMemberOfTypeAndName(CodeTypeDeclaration typeDeclaration, Type type, string name)
    {
      foreach (CodeTypeMember member in typeDeclaration.Members)
      {
        if (type.IsInstanceOfType(member) && member.Name == name)
        {
          return true;
        }
      }
      return false;
    }

    public static void AssertHasProperty(CodeTypeDeclaration typeDeclaration, string name)
    {
      Assert.IsTrue(HasMemberOfTypeAndName(typeDeclaration, typeof(CodeMemberProperty), name), "Expected proprety: " + name);
    }

    public static void AssertHasField(CodeTypeDeclaration typeDeclaration, string name)
    {
      Assert.IsTrue(HasMemberOfTypeAndName(typeDeclaration, typeof(CodeMemberField), name), "Expected field: " + name);
    }

    public static void AssertHasMethod(CodeTypeDeclaration typeDeclaration, string name)
    {
      Assert.IsTrue(HasMemberOfTypeAndName(typeDeclaration, typeof(CodeMemberMethod), name), "Expected method: " + name);
    }

    public static void AssertNotHasProperty(CodeTypeDeclaration typeDeclaration, string name)
    {
      Assert.IsFalse(HasMemberOfTypeAndName(typeDeclaration, typeof(CodeMemberProperty), name), "Unexpected proprety: " + name);
    }

    public static void AssertNotHasField(CodeTypeDeclaration typeDeclaration, string name)
    {
      Assert.IsFalse(HasMemberOfTypeAndName(typeDeclaration, typeof(CodeMemberField), name), "Unexpected field: " + name);
    }

    public static void AssertNotHasMethod(CodeTypeDeclaration typeDeclaration, string name)
    {
      Assert.IsFalse(HasMemberOfTypeAndName(typeDeclaration, typeof(CodeMemberMethod), name), "Unexpected method: " + name);
    }

    public static CodeTypeDeclaration AssertHasType(CodeCompileUnit ccu, string name)
    {
      foreach (CodeNamespace ns in ccu.Namespaces)
      {
        foreach (CodeTypeDeclaration type in ns.Types)
        {
          if (type.Name == name)
          {
            return type;
          }
        }
      }
      Assert.Fail("Expected type: " + name);
      return null;
    }
  }
}
