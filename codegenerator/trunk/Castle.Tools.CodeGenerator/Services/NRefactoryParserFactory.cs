using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.NRefactory.Parser;

namespace Castle.Tools.CodeGenerator.Services
{
  public class NRefactoryParserFactory : IParserFactory
  {
    #region IParserFactory Members
    public IParser CreateCSharpParser(TextReader reader)
    {
      return ParserFactory.CreateParser(SupportedLanguage.CSharp, reader);
    }
    #endregion
  }
}
