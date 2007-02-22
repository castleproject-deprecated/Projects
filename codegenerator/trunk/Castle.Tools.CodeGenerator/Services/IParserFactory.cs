using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.NRefactory.Parser;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface IParserFactory
  {
    IParser CreateCSharpParser(TextReader reader);
  }
}
