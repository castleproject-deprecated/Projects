using System;

namespace Castle.Tools.CodeGenerator
{
  [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
  public class DictionaryAdapterKeyPrefixAttribute : Attribute
  {
    #region Member Data
    private string keyPrefix;
    #endregion

    #region DictionaryAdapterKeyPrefixAttribute()
    public DictionaryAdapterKeyPrefixAttribute()
    {
    }

    public DictionaryAdapterKeyPrefixAttribute(string keyPrefix)
    {
      this.keyPrefix = keyPrefix;
    }
    #endregion

    #region Properties
    public string KeyPrefix
    {
      get { return keyPrefix; }
      set { keyPrefix = value; }
    }
    #endregion
  }
}
