using System;
using System.Collections.Generic;
using Rhino.Mocks;
using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.Model
{
  [TestFixture]
  public class MethodSignatureTests
  {
    #region Test Methods
    [Test]
    public void Equals_SameNameSameTypes_Works()
    {
      MethodSignature ms1 = new MethodSignature(GetType(), "Method", new Type[] { typeof(string) });
      MethodSignature ms2 = new MethodSignature(GetType(), "Method", new Type[] { typeof(string) });
      Assert.IsTrue(ms1.Equals(ms2));
      Assert.AreEqual(ms1.GetHashCode(), ms2.GetHashCode());
      Assert.AreEqual(ms1.ToString(), ms2.ToString());
    }

    [Test]
    public void Equals_SameNameDifferentTypes_NotEqual()
    {
      MethodSignature ms1 = new MethodSignature(GetType(), "Method", new Type[] { typeof(string) });
      MethodSignature ms2 = new MethodSignature(GetType(), "Method", new Type[] { typeof(long) });
      Assert.IsFalse(ms1.Equals(ms2));
      Assert.AreNotEqual(ms1.GetHashCode(), ms2.GetHashCode());
      Assert.AreNotEqual(ms1.ToString(), ms2.ToString());
    }

    [Test]
    public void Equals_SameNameDifferentNumberOfTypes_NotEqual()
    {
      MethodSignature ms1 = new MethodSignature(GetType(), "Method", new Type[] { typeof(string) });
      MethodSignature ms2 = new MethodSignature(GetType(), "Method", new Type[] { typeof(string), typeof(string) });
      Assert.IsFalse(ms1.Equals(ms2));
      Assert.AreNotEqual(ms1.GetHashCode(), ms2.GetHashCode());
      Assert.AreNotEqual(ms1.ToString(), ms2.ToString());
    }

    [Test]
    public void Equals_DifferentNameSameTypes_NotEqual()
    {
      MethodSignature ms1 = new MethodSignature(GetType(), "Method1", new Type[] { typeof(string) });
      MethodSignature ms2 = new MethodSignature(GetType(), "Method2", new Type[] { typeof(string) });
      Assert.IsFalse(ms1.Equals(ms2));
      Assert.AreNotEqual(ms1.GetHashCode(), ms2.GetHashCode());
      Assert.AreNotEqual(ms1.ToString(), ms2.ToString());
    }

    [Test]
    public void Equals_SameEverythingButType_NotEqual()
    {
      MethodSignature ms1 = new MethodSignature(GetType(), "Method", new Type[] { typeof(string) });
      MethodSignature ms2 = new MethodSignature(typeof(string), "Method", new Type[] { typeof(string) });
      Assert.IsFalse(ms1.Equals(ms2));
      Assert.AreNotEqual(ms1.GetHashCode(), ms2.GetHashCode());
      Assert.AreNotEqual(ms1.ToString(), ms2.ToString());
    }
    #endregion	
  }
}
