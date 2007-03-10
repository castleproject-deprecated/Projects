using System;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface INamingService
  {
    string ToVariableName(string name);
    string ToPropertyName(string name);
    string ToMemberVariableName(string name);
    string ToClassName(string name);
    string ToControllerName(string name);
    string ToAreaWrapperName(string name);
    string ToControllerWrapperName(string name);
    string ToActionWrapperName(string name);
    string ToViewWrapperName(string name);
    string ToMethodSignatureName(string name, string[] types);
    string ToMethodSignatureName(string name, Type[] types);
  }
}
