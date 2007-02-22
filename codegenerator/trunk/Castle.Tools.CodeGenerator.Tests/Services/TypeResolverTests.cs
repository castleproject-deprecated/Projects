using System;

using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class TypeResolverTests
  {
    #region Member Data
    private TypeResolver _typeResolver;
    #endregion

    #region Test SetUp and TearDown Methods
    [SetUp]
    public void Setup()
    {
      _typeResolver = new TypeResolver();
    }
    #endregion

    #region Test Methods
    [Test]
    public void Resolve_Integer_Works()
    {
      Assert.AreEqual(typeof(int), _typeResolver.Resolve("int", true));
    }

    [Test]
    public void Resolve_Long_Works()
    {
      Assert.AreEqual(typeof(long), _typeResolver.Resolve("long", true));
    }

    [Test]
    public void Resolve_Short_Works()
    {
      Assert.AreEqual(typeof(short), _typeResolver.Resolve("short", true));
    }

    [Test]
    public void Resolve_Character_Works()
    {
      Assert.AreEqual(typeof(char), _typeResolver.Resolve("char", true));
    }

    [Test]
    public void Resolve_String_Works()
    {
      Assert.AreEqual(typeof(string), _typeResolver.Resolve("string", true));
    }

    [Test]
    public void Resolve_FromUsing_Works()
    {
      _typeResolver.UseNamespace("System");
      Assert.AreEqual(typeof(DateTime), _typeResolver.Resolve("DateTime", true));
    }

    [Test]
    [ExpectedException(typeof(TypeLoadException))]
    public void Resolve_MissingType_Throws()
    {
      _typeResolver.UseNamespace("System.Collections");
      _typeResolver.Resolve("DateTime", true);
    }

    [Test]
    public void Resolve_MissingTypeNoThrow_ReturnsNull()
    {
      Assert.IsNull(_typeResolver.Resolve("DateTime", false));
    }

    [Test]
    [ExpectedException(typeof(TypeLoadException))]
    public void Clear_Always_RemovesEntries()
    {
      _typeResolver.UseNamespace("System");
      _typeResolver.Clear();
      _typeResolver.Resolve("DateTime", true);
    }

    [Test]
    public void Resolve_TypeTableEntry_Works()
    {
      _typeResolver.AddTableEntry("Eleutian.Namespace.Type");
      _typeResolver.UseNamespace("Eleutian.Namespace");
      Assert.AreEqual("Eleutian.Namespace.Type", _typeResolver.Resolve("Type"));
    }

    [Test]
    public void Resolve_MissingTypeTableEntry_ReturnsNull()
    {
      _typeResolver.AddTableEntry(new TypeTableEntry("Eleutian.Namespace.AnotherType"));
      _typeResolver.UseNamespace("Eleutian.Namespace");
      Assert.IsNull(_typeResolver.Resolve("Type"));
    }

    [Test]
    public void Resolve_AbsoluteQualified_Works()
    {
      Assert.IsNotNull(_typeResolver.Resolve("System.DateTime", false));
    }

    [Test]
    public void AliasNamespace_Always_AddsAlias()
    {
      _typeResolver.AliasNamespace("Bob", "System");
      _typeResolver.Resolve("DateTime");
    }

    /*
    [Test]
    public void Resolve_AbsoluteQualifiedRequireAssemblySearch_Works()
    {
      Type type = typeof(Eleutian.Shared.DateTimeHelper);
      Assert.AreEqual(type, _typeResolver.Resolve("Eleutian.Shared.DateTimeHelper", false));
    }
    */
    #endregion
  }
}