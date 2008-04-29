
namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.ViewComponents
{
	using Framework;

	public class TextRendererViewComponent : ViewComponent
	{
		public override void Render()
		{
			RenderText("Default text from component's RenderText()");
		}
	}
}
