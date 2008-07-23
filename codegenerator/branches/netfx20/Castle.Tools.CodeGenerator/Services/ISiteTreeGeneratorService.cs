using ICSharpCode.NRefactory;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface ISiteTreeGeneratorService
  {
    void Parse(IAstVisitor visitor, string path);
  }
}
