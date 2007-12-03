namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class SanityView : AspViewBase
	{
		public override void Render()
		{
			Output("Sanity");
		}

		protected override string ViewDirectory
		{
			get { throw new System.Exception("The method or operation is not implemented."); }
		}

		protected override string ViewName
		{
			get { throw new System.Exception("The method or operation is not implemented."); }
		}
	}
}
