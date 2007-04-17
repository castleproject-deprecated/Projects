using System;
using System.Collections.Generic;

using Castle.Tools.CodeGenerator.Model;

using NUnit.Framework;
using Rhino.Mocks;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class ControllerReferenceFactoryTests
  {
    #region Member Data
    private MockRepository _mocks;
    private DefaultControllerReferenceFactory _factory;
    private TestController _controller;
    private ICodeGeneratorServices _services;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public virtual void Setup()
    {
      _mocks = new MockRepository();
      _services = _mocks.CreateMock<ICodeGeneratorServices>();
      _controller = _mocks.CreateMock<TestController>();
      _factory = new DefaultControllerReferenceFactory();
    }
    #endregion

    #region Test Methods
    [Test]
    public void CreateActionReference_Always_CreatesValidReference()
    {
      ControllerActionReference reference =
        (ControllerActionReference)_factory.CreateActionReference(_services, typeof(TestController), "Area", "Controller", "Action", null,
                                       new ActionArgument[0]);
      Assert.AreEqual("Controller", reference.ControllerName);
      Assert.AreEqual("Area", reference.AreaName);
      Assert.AreEqual("Action", reference.ActionName);
      Assert.AreEqual(typeof(TestController), reference.ControllerType);
      Assert.IsEmpty(reference.Arguments);
    }

    [Test]
    public void CreateViewReference_Always_CreatesValidReference()
    {
      ControllerViewReference reference =
        (ControllerViewReference)_factory.CreateViewReference(_services, typeof(TestController), "Area", "Controller", "Action");
      Assert.AreEqual("Controller", reference.ControllerName);
      Assert.AreEqual("Area", reference.AreaName);
      Assert.AreEqual("Action", reference.ActionName);
      Assert.AreEqual(typeof(TestController), reference.ControllerType);
    }
    #endregion
  }
}
