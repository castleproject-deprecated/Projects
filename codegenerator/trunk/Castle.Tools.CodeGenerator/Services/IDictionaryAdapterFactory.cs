using System;

using System.Collections;

namespace Castle.Tools.CodeGenerator
{
  public interface IDictionaryAdapterFactory
  {
    T GetAdapter<T>(IDictionary session) where T : IDictionaryAdapter;
  }
}
