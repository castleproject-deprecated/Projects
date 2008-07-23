using System;
using System.Collections.Generic;
using Rhino.Mocks;
using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class TypeTableEntryTests
  {
  	#region Test Setup and Teardown Methods
  	[SetUp]
  	public void Setup()
  	{
  	}
  	#endregion
  	
  	#region Test Methods
    [Test]
    public void Constructor_Initializes_Always()
    {
      TypeTableEntry entry = new TypeTableEntry("System.String", typeof(string));
      Assert.AreEqual(typeof(string), entry.CompiledType);
      Assert.AreEqual("System.String", entry.FullName);
      Assert.AreEqual("String", entry.Name);
      Assert.IsTrue(entry.HasCompiledType);
    }

    [Test]
    public void NonTypedConstructor_Initializes_Always()
    {
      TypeTableEntry entry = new TypeTableEntry("System.String");
      Assert.IsNull(entry.CompiledType);
      Assert.AreEqual("System.String", entry.FullName);
      Assert.AreEqual("String", entry.Name);
      Assert.IsFalse(entry.HasCompiledType);
    }
  	#endregion	
  }
}
