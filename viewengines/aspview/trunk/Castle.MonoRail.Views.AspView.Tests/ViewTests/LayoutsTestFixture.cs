namespace Castle.MonoRail.Views.AspView.Tests.ViewTests
{
	using Views;
	using NUnit.Framework;

	[TestFixture]
	public class LayoutsTestFixture : AbstractViewTestFixture
	{
		protected override void CreateView()
		{
			IViewBaseInternal theView = new SanityView();
			IViewBaseInternal layout = new SimplestLayout();
			layout.ContentView = theView;

			view = layout;
		}

		[Test]
		public void Render_WhenRendersSimpleStringAndNoLayout_Works()
		{
			view.Process();

			string expected = @"Layout - before
Sanity
Layout - after
";

			AssertViewOutputEqualsTo(expected);
		}

	}
}
