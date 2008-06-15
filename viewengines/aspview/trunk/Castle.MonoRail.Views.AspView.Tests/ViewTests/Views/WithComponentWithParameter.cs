namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	using Framework.Helpers;

	public class WithComponentWithParameter : AspViewBase
	{
		public override void Render()
		{
			InvokeViewComponent("WithMandatoryParameter", DictHelper.CreateN("tExt", Properties["text"]));
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
