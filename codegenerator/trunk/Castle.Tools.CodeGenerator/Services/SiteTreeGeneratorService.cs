using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.NRefactory.Parser;

namespace Castle.Tools.CodeGenerator.Services
{
  public class SiteTreeGeneratorService : ISiteTreeGeneratorService
  {
    #region Member Data
    private ITypeResolver _typeResolver;
    private IParsedSourceStorageService _cache;
    private ILogger _logger;
    private IParserFactory _parserFactory;
    #endregion

    #region SiteTreeGeneratorService()
    public SiteTreeGeneratorService(ILogger logger, ITypeResolver typeResolver, IParsedSourceStorageService cache,  IParserFactory parserFactory)
    {
      _logger = logger;
      _typeResolver = typeResolver;
      _cache = cache;
      _parserFactory = parserFactory;
    }
    #endregion

    #region Methods
    public void Parse(IAstVisitor visitor, string path)
    {
      using (TextReader reader = File.OpenText(path))
      {
        IParser parser = _parserFactory.CreateCSharpParser(reader);
        parser.ParseMethodBodies = false;
        parser.Parse();

        _typeResolver.Clear();
        visitor.Visit(parser.CompilationUnit, null);
        _cache.Add(path, parser);
      }
    }
    #endregion
  }
}
