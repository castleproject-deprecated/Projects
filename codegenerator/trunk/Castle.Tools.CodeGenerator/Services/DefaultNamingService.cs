using System;
using System.Text;

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

    public string ToMethodSignatureName(string name, Type[] types)
    {
      string[] names = new string[types.Length];
      for (int i = 0; i < types.Length; i++)
      {
        names[i] = types[i].Name;
      }
      return ToMethodSignatureName(name, names);
    }

  	public string ToWizardStepWrapperName(string name)
  	{
  		return name + "WizardStepNode";
  	}

  	public string ToMethodSignatureName(string name, string[] types)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(name);
      foreach (string type in types)
      {
        sb.Append("_").Append(type.Replace(".", "_").Replace("[]", "BB"));
      }
      return sb.ToString();
    }
  }
}
