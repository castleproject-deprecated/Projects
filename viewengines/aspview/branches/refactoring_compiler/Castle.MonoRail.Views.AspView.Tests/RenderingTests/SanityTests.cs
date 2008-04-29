
namespace Castle.MonoRail.Views.AspView.Tests.RenderingTests
{
	using Xunit;

	public class SanityTests : IUseFixture<IntegrationViewTestFixture>
	{
		IntegrationViewTestFixture fixture;
		///<summary>
		///
		///            Called on the test class just before each test method is run,
		///            passing the fixture data so that it can be used for the test.
		///            All test runs share the same instance of fixture data.
		///            
		///</summary>
		///
		///<param name="data">The fixture data</param>
		public void SetFixture(IntegrationViewTestFixture data)
		{
			fixture = data;
		}

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
