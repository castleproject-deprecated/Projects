using System.Web;

namespace Castle.Tools.CodeGenerator.Services
{
	public class AspDotNetRedirectService : IRedirectService
	{
		public virtual void Redirect(string url)
		{
			HttpContext.Current.Response.Redirect(url);
		}

		public virtual void Transfer(string url)
		{
			HttpContext.Current.Server.Transfer(url);
		}
	}
}
