using System;

namespace Castle.Tools.CodeGenerator.Model
{
  public interface IControllerActionReference : IArgumentlessControllerActionReference
  {
    void Transfer();
    void Redirect(bool useJavascript);
    void Redirect();
  }
}
