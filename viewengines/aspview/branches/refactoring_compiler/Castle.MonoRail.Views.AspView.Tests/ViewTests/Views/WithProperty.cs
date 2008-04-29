namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class WithProperty : AspViewBase
	{
		private string prop { get { return (string)GetParameter("prop"); } }

		public override void Render()
		{
			Output(@"ViewWithProperty
");
			Output(prop);
			Output(@"
ViewWithProperty");
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
