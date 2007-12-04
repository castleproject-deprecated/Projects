namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.ViewComponents
{
	using Framework;

	public class ViewRendererViewComponent : ViewComponent
	{
		public override void Render()
		{
			RenderView("SimpleView");
		}
	}
}
