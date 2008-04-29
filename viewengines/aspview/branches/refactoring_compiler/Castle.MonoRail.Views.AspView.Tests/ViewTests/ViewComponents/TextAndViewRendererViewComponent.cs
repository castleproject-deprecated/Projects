namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.ViewComponents
{
	using Framework;

	public class TextAndViewRendererViewComponent : ViewComponent
	{
		public override void Render()
		{
			RenderView("SimpleView");
			RenderText("Default text from component's RenderText()");
		}
	}
}
