
namespace Castle.MonoRail.Views.AspView.Tests.RenderingTests
{
	using Xunit;

	public abstract class AbstractTests : IUseFixture<IntegrationViewTestFixture>
	{
		protected IntegrationViewTestFixture fixture;
		public virtual void SetFixture(IntegrationViewTestFixture data)
		{
			fixture = data;
		}
	}
}
