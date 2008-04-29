namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class SimpleView : AspViewBase
	{
		public override void Render()
		{
			Output(@"Simple");
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
