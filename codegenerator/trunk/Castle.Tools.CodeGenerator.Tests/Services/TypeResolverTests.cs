using System;
using System.Collections.Generic;
using System.Reflection;
using ICSharpCode.NRefactory.Parser.AST;
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
    public void Resolve_WithOnlyChildNamespaceButIncludeParentsTrue_Works()
    {
      _typeResolver.UseNamespace("System.Collections", true);
      Assert.AreEqual(typeof(DateTime), _typeResolver.Resolve("DateTime", true));
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

    [Test]
    public void ResolveStringOnly_ArrayOfDateTimes_Fails()
    {
      _typeResolver.UseNamespace("System");
      Assert.IsNull(_typeResolver.Resolve("DateTime[]"));
    }

    [Test]
    public void Resolve_ArrayOfDateTimes_Works()
    {
      _typeResolver.UseNamespace("System");
      Assert.IsNotNull(_typeResolver.Resolve("DateTime[]", true));
    }

    [Test]
    public void ResolveTypeReference_Simple_Works()
    {
      TypeReference reference = new TypeReference("DateTime");

      _typeResolver.UseNamespace("System");

      Assert.AreEqual("DateTime", reference.ToString());
      Assert.AreEqual("System.DateTime", _typeResolver.Resolve(reference));
    }

    [Test]
    public void ResolveTypeReference_SimplePrimitiveTypeArray_Works()
    {
      TypeReference reference = new TypeReference("long", new int[] { 0 });

      _typeResolver.UseNamespace("System");

      Assert.AreEqual("long[]", reference.ToString());
      Assert.AreEqual("System.Int64[]", _typeResolver.Resolve(reference));
    }

    [Test]
    public void ResolveTypeReference_SimpleArray_Works()
    {
      TypeReference reference = new TypeReference("DateTime", new int[] { 0 });

      _typeResolver.UseNamespace("System");

      Assert.AreEqual("DateTime[]", reference.ToString());
      Assert.AreEqual("System.DateTime[]", _typeResolver.Resolve(reference));
    }

    [Test]
    public void ResolveTypeReference_TwoDimArray_Works()
    {
      TypeReference reference = new TypeReference("DateTime", new int[] { 0, 0 });

      _typeResolver.UseNamespace("System");

      Assert.AreEqual("DateTime[][]", reference.ToString());
      Assert.AreEqual("System.DateTime[][]", _typeResolver.Resolve(reference));
    }

    [Test]
    public void ResolveTypeReference_ArrayOfSourceType_Works()
    {
      TypeReference reference = new TypeReference("RadClass", new int[] { 0, 0 });

      _typeResolver.AddTableEntry("SomeNamespace.Utility.RadClass");

      _typeResolver.UseNamespace("System");
      _typeResolver.UseNamespace("SomeNamespace.Utility");

      Assert.AreEqual("RadClass[][]", reference.ToString());
      Assert.AreEqual("SomeNamespace.Utility.RadClass[][]", _typeResolver.Resolve(reference));
    }

    [Test]
    public void ResolveTypeReference_GenericList_Works()
    {
      TypeReference reference = new TypeReference("List", new List<TypeReference>(new TypeReference[] { new TypeReference("string") }));
      _typeResolver.UseNamespace("System");
      _typeResolver.UseNamespace("System.Collections.Generic");

      Assert.AreEqual("List<string>", reference.ToString());
      string actual = _typeResolver.Resolve(reference);
    }

    [Test]
    public void ResolveTypeReference_GenericDictionary_Works()
    {
      TypeReference reference = new TypeReference("Dictionary", new List<TypeReference>(new TypeReference[] { new TypeReference("string"), new TypeReference("DateTime") }));
      _typeResolver.UseNamespace("System");
      _typeResolver.UseNamespace("System.Collections.Generic");

      Assert.AreEqual("Dictionary<string,DateTime>", reference.ToString());
      string actual = _typeResolver.Resolve(reference);
    }

    [Test]
    public void Resolve_Enumeration_Works()
    {
      _typeResolver.UseNamespace("System");
      Assert.IsNull(_typeResolver.Resolve("DayOfWeek"));
    }
    #endregion
  }
}