namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	using Framework.Helpers;

	public class WithSubViewWithParameters : AspViewBase
	{
		public override void Render()
		{
			Output(@"Parent
");
			OutputSubView("SubView", DictHelper.CreateN("prop", "property"));
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
