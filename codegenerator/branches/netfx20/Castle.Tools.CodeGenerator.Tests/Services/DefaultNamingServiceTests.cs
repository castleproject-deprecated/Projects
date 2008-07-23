using System;

using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class NamingServiceTests
  {
    #region Member Data
    private DefaultNamingService _service;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _service = new DefaultNamingService();
    }
    #endregion

    #region Test Methods
    [Test]
    public void ToVariableName_Always_LowercasesFirstLetter()
    {
      Assert.AreEqual("helloWorld", _service.ToVariableName("HelloWorld"));
      Assert.AreEqual("helloWorld", _service.ToVariableName("helloWorld"));
    }

    [Test]
    public void ToMemberVariableName_Always_LowercasesFirstLetterAndsPrefixes()
    {
      Assert.AreEqual("_helloWorld", _service.ToMemberVariableName("HelloWorld"));
      Assert.AreEqual("_helloWorld", _service.ToMemberVariableName("helloWorld"));
    }

    [Test]
    public void ToPropertyName_Always_UppercasesFirstLetter()
    {
      Assert.AreEqual("HelloWorld", _service.ToPropertyName("HelloWorld"));
      Assert.AreEqual("HelloWorld", _service.ToPropertyName("helloWorld"));
    }

    [Test]
    public void ToClassName_Always_UppercasesFirstLetter()
    {
      Assert.AreEqual("HelloWorld", _service.ToClassName("HelloWorld"));
      Assert.AreEqual("HelloWorld", _service.ToClassName("helloWorld"));
    }

    [Test]
    public void ToControllerName_Always_StripsController()
    {
      Assert.AreEqual("HelloWorld", _service.ToControllerName("HelloWorldController"));
      Assert.AreEqual("HelloWorld", _service.ToControllerName("HelloWorld"));
    }

    [Test]
    public void ToAreaWrapperName_Always_AppendsAreaNode()
    {
      Assert.AreEqual("HelloWorldAreaNode", _service.ToAreaWrapperName("HelloWorld"));
    }

    [Test]
    public void ToActionWrapperName_Always_AppendsAreaNode()
    {
      Assert.AreEqual("HelloWorldActionNode", _service.ToActionWrapperName("HelloWorld"));
    }

    [Test]
    public void ToViewWrapperName_Always_AppendsAreaNode()
    {
      Assert.AreEqual("HelloWorldViewNode", _service.ToViewWrapperName("HelloWorld"));
    }

    [Test]
    public void ToControllerWrapperName_Always_AppendsNode()
    {
      Assert.AreEqual("HelloWorldControllerNode", _service.ToControllerWrapperName("HelloWorldController"));
    }

    [Test]
    public void ToMemberSignature_SameName_AreDifferent()
    {
      string name1 = _service.ToMethodSignatureName("Method", new Type[] { typeof(string) });
      string name2 = _service.ToMethodSignatureName("Method", new Type[] { typeof(long) });
      Assert.AreNotEqual(name1, name2);
    }

    [Test]
    public void ToMemberSignature_SameTypes_AreSame()
    {
      string name1 = _service.ToMethodSignatureName("Method", new string[] { "string" });
      string name2 = _service.ToMethodSignatureName("Method", new string[] { "string" });
      Assert.AreEqual(name1, name2);
    }

    [Test]
    public void ToMemberSignature_SameTypesDifferentName_AreDifferent()
    {
      string name1 = _service.ToMethodSignatureName("Method", new Type[] { typeof(string) });
      string name2 = _service.ToMethodSignatureName("OtherMethod", new Type[] { typeof(string) });
      Assert.AreNotEqual(name1, name2);
    }

    [Test]
    public void ToMemberSignature_ArrayTypes_ArentStupid()
    {
      string name = _service.ToMethodSignatureName("Method", new Type[] { typeof(string[]) });
      Assert.AreEqual("Method_StringBB", name);
    }
    #endregion
  }
}