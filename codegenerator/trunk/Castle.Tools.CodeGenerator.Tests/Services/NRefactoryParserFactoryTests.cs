using System.IO;
using ICSharpCode.NRefactory;
using NUnit.Framework;
using Rhino.Mocks;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class NRefactoryParserFactoryTests
  {
    #region Member Data
    private MockRepository _mocks;
    private NRefactoryParserFactory _factory;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _mocks = new MockRepository();
      _factory = new NRefactoryParserFactory();
    }
    #endregion

    #region Test Methods
    [Test]
    public void CreateCSharpParser_Always_CreatesParser()
    {
      IParser parser = _factory.CreateCSharpParser(new StringReader(""));
      Assert.IsNotNull(parser);
    }
    #endregion
  }
}
