namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class WithPropertyAndWithSubViewWithParameters : AspViewBase
	{
		private string prop { get { return (string)GetParameter("prop"); } }

		public override void Render()
		{
			Output(@"Parent
");
			OutputSubView("SubView", "prop", "property");
			Output(@"
");
			Output(prop);
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
