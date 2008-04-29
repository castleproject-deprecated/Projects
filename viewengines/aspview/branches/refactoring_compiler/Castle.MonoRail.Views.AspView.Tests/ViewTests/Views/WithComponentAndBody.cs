namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class WithComponentAndBody : AspViewBase
	{
		public override void Render()
		{
			Output(@"Parent
");
			InvokeViewComponent("MyComponent", MyComponent1_body,  null);
			Output(@"
Parent");
		}


		internal void MyComponent1_body()
		{
			Output(@"Component's Body");
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
