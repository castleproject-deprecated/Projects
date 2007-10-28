namespace Castle.Tools.CodeGenerator.Services
{
	public interface IRedirectService
	{
		void Redirect(string url);
		void Transfer(string url);
	}
}
