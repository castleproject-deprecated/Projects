using System;
using System.Collections.Generic;
using Rhino.Mocks;
using NUnit.Framework;

namespace Eleutian.Tools.CodeGenerator.Services
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
      Assert.AreEqual(value, _service.ConvertArgument("value", value));
    }
    #endregion	
  }
}
