using System;
using System.Collections.Generic;
using System.Web;

using Castle.MonoRail.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  public class AspDotNetRedirectService : IRedirectService
  {
    #region Public Methods
    public virtual void Redirect(string url)
    {
      HttpContext.Current.Response.Redirect(url);
    }
    #endregion
  }
}
