using System;
using System.Collections.Generic;
using Rhino.Mocks;
using NUnit.Framework;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class DefaultRuntimeInformationServiceTests
  {
    #region Member Data
    private MockRepository _mocks;
    private DefaultRuntimeInformationService _service;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _mocks = new MockRepository();
      _service = new DefaultRuntimeInformationService();
    }
    #endregion

    #region Test Methods
    [Test]
    [ExpectedException(typeof(MissingMethodException))]
    public void ResolveMethodInformation_BadMethodName_Throws()
    {
      _service.ResolveMethodInformation(typeof(RuntimeType), "BadMethod", new Type[0]);
    }

    [Test]
    [ExpectedException(typeof(MissingMethodException))]
    public void ResolveMethodInformation_BadMethodSignature_Throws()
    {
      _service.ResolveMethodInformation(typeof(RuntimeType), "MyMethod", new Type[0]);
    }

    [Test]
    public void ResolveMethodInformation_GoodSignature_ReturnsInformation()
    {
      MethodInformation information = _service.ResolveMethodInformation(typeof(RuntimeType), "MyMethod", new Type[] { typeof(string) });
      Assert.AreEqual("MyMethod", information.Method.Name);
      Assert.AreEqual(1, information.Parameters.Length);
      Assert.AreEqual("name", information.Parameters[0].Parameter.Name);
      Assert.AreEqual(1, information.Parameters[0].CustomAttributes.Length);
      Assert.AreEqual(1, information.CustomAttributes.Length);
    }

    [Test]
    public void GetCustomAttributes_OfType_Works()
    {
      MethodInformation information = _service.ResolveMethodInformation(typeof(RuntimeType), "MyMethod", new Type[] { typeof(string) });
      Assert.AreEqual(1, information.GetCustomAttributes(typeof(ExampleTestAttribute)).Length);
    }

    [Test]
    public void ResolveMethodInformation_SecondTime_ReturnsSameInformation()
    {
      MethodInformation information1 = _service.ResolveMethodInformation(typeof(RuntimeType), "MyMethod", new Type[] { typeof(string) });
      MethodInformation information2 = _service.ResolveMethodInformation(typeof(RuntimeType), "MyMethod", new Type[] { typeof(string) });
      Assert.AreSame(information1, information2);
    }
    #endregion	
  }

  public class RuntimeType
  {
    [ExampleTest]
    public void MyMethod([ExampleTest] string name)
    {
    }
  }

  public class ExampleTestAttribute : Attribute
  {
  }
}
