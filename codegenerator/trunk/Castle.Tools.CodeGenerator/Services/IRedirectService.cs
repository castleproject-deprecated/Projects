using System;
using System.Collections.Generic;
using System.Web;
using Castle.MonoRail.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface IRedirectService
  {
    void Redirect(string url);
  }
}
