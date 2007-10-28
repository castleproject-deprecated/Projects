namespace Castle.Tools.CodeGenerator.Model
{
	public interface IControllerActionReference : IArgumentlessControllerActionReference
	{
		void Redirect(bool useJavascript);
		void Redirect();
		void Transfer();
	}
}
