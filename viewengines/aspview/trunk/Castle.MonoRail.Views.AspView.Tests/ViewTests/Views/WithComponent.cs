namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class WithComponent : AspViewBase
	{
		public override void Render()
		{
			Output(@"Parent
");
			InvokeViewComponent("MyComponent", null, null);
			Output(@"
Parent");
		}

		protected override string ViewDirectory
		{
			get { return ""; }
		}

		protected override string ViewName
		{
			get { throw new System.Exception("The method or operation is not implemented."); }
		}
	}
}
