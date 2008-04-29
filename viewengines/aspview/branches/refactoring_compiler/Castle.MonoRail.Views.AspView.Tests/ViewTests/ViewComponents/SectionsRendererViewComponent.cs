
namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.ViewComponents
{
	using Framework;

	public class SectionsRendererViewComponent : ViewComponent
	{
		public override void Render()
		{
			RenderSection("section1");
			RenderText("Text");
			RenderSection("section2");
		}
	}
}
