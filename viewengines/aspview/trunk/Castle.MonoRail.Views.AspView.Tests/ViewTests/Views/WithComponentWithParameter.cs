namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class WithComponentWithParameter : AspViewBase
	{
		public override void Render()
		{
			InvokeViewComponent("WithMandatoryParameter", null, null, "tExt", Properties["text"]);
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
