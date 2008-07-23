using System;
using System.Collections;
using System.Collections.Generic;

using Castle.Tools.CodeGenerator.Model;

using Rhino.Mocks;
using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class DefaultArgumentConversionServiceTests
  {
    #region Member Data
    private MockRepository _mocks;
    private DefaultArgumentConversionService _service;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _mocks = new MockRepository();
      _service = new DefaultArgumentConversionService();
    }
    #endregion

    #region Test Methods
    [Test]
    public void ConvertValue_Always_ReturnsSameValue()
    {
      string value = "Hello, World!";
      ActionArgument argument = new ActionArgument(0, "name", value);
      IDictionary parameters = new Hashtable();
      Assert.IsTrue(_service.ConvertArgument(null, argument, parameters));
      Assert.AreEqual(value, parameters["name"]);
    }
    #endregion	
  }
}
