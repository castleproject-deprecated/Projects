using ICSharpCode.NRefactory;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface IParsedSourceStorageService
  {
    void Add(string path, IParser parser);
    IParser GetParsedSource(string path);
  }
}
