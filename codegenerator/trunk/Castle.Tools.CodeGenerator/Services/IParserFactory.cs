using System.IO;
using ICSharpCode.NRefactory;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface IParserFactory
  {
    IParser CreateCSharpParser(TextReader reader);
  }
}
