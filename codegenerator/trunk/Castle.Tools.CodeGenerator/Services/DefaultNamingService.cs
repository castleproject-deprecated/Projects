using System;

namespace Castle.Tools.CodeGenerator.Services
{
  public class DefaultNamingService : INamingService
  {
    public string ToVariableName(string name)
    {
      return name[0].ToString().ToLower() + name.Substring(1);
    }

    public string ToPropertyName(string name)
    {
      return name[0].ToString().ToUpper() + name.Substring(1);
    }

    public string ToMemberVariableName(string name)
    {
      return "_" + ToVariableName(name);
    }

    public string ToClassName(string name)
    {
      return ToPropertyName(name);
    }

    public string ToControllerName(string name)
    {
      if (name.EndsWith("Controller"))
      {
        return name.Replace("Controller", "");
      }
      return name;
    }

    public string ToAreaWrapperName(string name)
    {
      return name + "AreaNode";
    }

    public string ToControllerWrapperName(string name)
    {
      return name + "Node";
    }

    public string ToActionWrapperName(string name)
    {
      return name + "ActionNode";
    }

    public string ToViewWrapperName(string name)
    {
      return name + "ViewNode";
    }
  }
}