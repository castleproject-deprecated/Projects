using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory.Parser;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface ISiteTreeGeneratorService
  {
    void Parse(IAstVisitor visitor, string path);
  }
}
