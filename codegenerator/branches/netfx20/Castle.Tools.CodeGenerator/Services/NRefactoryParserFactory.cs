using System.IO;
using ICSharpCode.NRefactory;

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
