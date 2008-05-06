
namespace Castle.MonoRail.Views.AspView.Tests.RenderingTests
{
	using Xunit;

	public class SanityTests : AbstractTests
	{
		[Fact]
		public void WhenRenderingSimpleTextView_ItRendersProperly()
		{
			const string expected = @"I am sane";
			fixture.ProcessView("/sanity/sanity");
			fixture.AssertReplyEqualTo(expected);
			System.Console.WriteLine("A");
		}

	}
}
