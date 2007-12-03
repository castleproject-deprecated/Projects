namespace Castle.MonoRail.Views.AspView.Tests.ViewTests
{
	using Views;
	using NUnit.Framework;

	[TestFixture]
	public class SanityTestFixture : AbstractViewTestFixture
	{
		protected override void CreateView()
		{
			view = new SanityView();
		}

		[Test]
		public void Render_WhenRendersSimpleStringAndNoLayout_Works()
		{
			view.Render();

			AssertViewOutputEqualsTo("Sanity");
		}

	}
}
