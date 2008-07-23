using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.Tools.CodeGenerator.Model
{
  public class MethodSignature
  {
    #region Member Data
    private Type _type;
    private string _name;
    private Type[] _types;
    #endregion

    #region Properties
    public Type Type
    {
      get { return _type; }
    }

    public string Name
    {
      get { return _name; }
    }

    public Type[] Types
    {
      get { return _types; }
    }
    #endregion

    #region MethodSignature()
    public MethodSignature(Type type, string name, Type[] types)
    {
      _type = type;
      _name = name;
      _types = types;
    }
    #endregion

    #region Methods
    public override bool Equals(object obj)
    {
      MethodSignature ms = obj as MethodSignature;
      if (ms == null)
      {
        return base.Equals(obj);
      }
      if (!this.Type.Equals(ms.Type) || !this.Name.Equals(ms.Name) || ms.Types.Length != this.Types.Length)
      {
        return false;
      }
      for (int i = 0; i < this.Types.Length; i++)
      {
        if (!this.Types[i].Equals(ms.Types[i]))
        {
          return false;
        }
      }
      return true;
    }

    public override int GetHashCode()
    {
      int hash = this.Type.GetHashCode() ^ this.Name.GetHashCode();
      foreach (Type type in this.Types)
      {
        hash ^= type.GetHashCode();
      }
      return hash;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      foreach (Type type in this.Types)
      {
        if (sb.Length != 0)
        {
          sb.Append(", ");
        }
        sb.Append(type);
      }
      string args = sb.ToString();
      sb = new StringBuilder();
      sb.Append(this.Type.FullName).Append(this.Name).Append("(").Append(args).Append(")");
      return sb.ToString();
    }
    #endregion
  }
}
