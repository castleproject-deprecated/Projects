using System;
using System.Collections.Generic;
using System.Reflection;

namespace Castle.Tools.CodeGenerator.Services
{
  public class TypeTableEntry
  {
    #region Member Data
    private string _fullName;
    private Type _type;
    #endregion

    #region Properties
    public string FullName
    {
      get { return _fullName; }
    }

    public string Name
    {
      get { return _fullName.Substring(_fullName.LastIndexOf('.') + 1); }
    }

    public Type CompiledType
    {
      get { return _type; }
    }

    public bool HasCompiledType
    {
      get { return _type != null; }
    }
    #endregion

    #region TypeTableEntry()
    public TypeTableEntry(string fullName, Type type)
    {
      _fullName = fullName;
      _type = type;
    }

    public TypeTableEntry(string fullName)
    {
      _fullName = fullName;
    }
    #endregion
  }
}
