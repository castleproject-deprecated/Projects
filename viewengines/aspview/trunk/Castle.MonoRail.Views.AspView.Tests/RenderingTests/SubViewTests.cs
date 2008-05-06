
namespace Castle.MonoRail.Views.AspView.Tests.RenderingTests
{
	using Xunit;

	public class SubViewTests : AbstractTests
	{
		[Fact]
		public void WhenCallingASubviewOnTheSameDirectoryWithoutArguments_ItRendersProperly()
		{
			const string expected = @"I am view1
I am view2
I am view1 again";
			fixture.ProcessView("/subviews/view1");
			fixture.AssertReplyEqualTo(expected);
		}

	}
}
