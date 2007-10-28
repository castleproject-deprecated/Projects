namespace Castle.Tools.CodeGenerator.Model
{
	public interface IControllerViewReference
	{
		void Render(bool skiplayout);
		void Render();
	}
}
