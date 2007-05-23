using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class SiteTreeGeneratorServiceTests
  {
    #region Member Data
    private MockRepository _mocks;
    private SiteTreeGeneratorService _service;
    private ILogger _logger;
    private ITypeResolver _typeResolver;
    private IAstVisitor _visitor;
    private IParser _parser;
    private IParserFactory _parserFactory;
    private string _path;
    private IParsedSourceStorageService _sources;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _mocks = new MockRepository();
      _logger = new NullLogger();
      _typeResolver = _mocks.CreateMock<ITypeResolver>();
      _visitor = _mocks.CreateMock<IAstVisitor>();
      _sources = _mocks.CreateMock<IParsedSourceStorageService>();
      _parserFactory = _mocks.CreateMock<IParserFactory>();
      _parser = _mocks.CreateMock<IParser>();
      _service = new SiteTreeGeneratorService(_logger, _typeResolver, _sources, _parserFactory);
      _path = "~~TemporarySource.cs";
      WriteSampleSource(_path);
    }

    [TearDown]
    public void Teardown()
    {
      File.Delete(_path);
    }
    #endregion

    #region Test Methods
    [Test]
    public void Parse()
    {
      CompilationUnit unit = new CompilationUnit();

      using (_mocks.Unordered())
      {
        Expect.Call(_parserFactory.CreateCSharpParser(null)).Constraints(Is.NotNull()).Return(_parser);
        _parser.ParseMethodBodies = true;
        _parser.Parse();
        _typeResolver.Clear();
        Expect.Call(_parser.CompilationUnit).Return(unit);
        Expect.Call(_visitor.VisitCompilationUnit(unit, null)).Return(null);
        _sources.Add(_path, _parser);
      }

      _mocks.ReplayAll();
      _service.Parse(_visitor, _path);
      _mocks.VerifyAll();
    }
    #endregion

    #region Methods
    protected static void WriteSampleSource(string path)
    {
      using (StreamWriter writer = File.CreateText(path))
      {
        writer.WriteLine("using System;");
        writer.WriteLine("public class Program {");
        writer.WriteLine("public static void Main(string[] args) { }");
        writer.WriteLine("}");
      }
    }
    #endregion
  }
}
