using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.NRefactory;

namespace Castle.Tools.CodeGenerator.Services
{
  public class DefaultSourceStorageService : IParsedSourceStorageService
  {
    #region Member Data
    private Dictionary<string, IParser> _cache = new Dictionary<string, IParser>(StringComparer.CurrentCultureIgnoreCase);
    #endregion

    #region IParsedCodeCacheService Members
    public void Add(string path, IParser parser)
    {
      if (_cache.ContainsKey(path))
        throw new ArgumentException(String.Format("Source for {0} already cached!", path));
      _cache[path] = parser;
    }

    public IParser GetParsedSource(string path)
    {
      if (!_cache.ContainsKey(path))
        throw new KeyNotFoundException(path);
      return _cache[path];
    }
    #endregion
  }
}
