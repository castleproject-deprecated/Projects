namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class UsingBubbledCaptureFromSubView : AspViewBase
	{
		private string CapturedContent { get { return (string)GetParameter("CapturedContent"); } }
		public override void Render()
		{
			Output(@"Parnt View
From subview: ");
			OutputSubView("subview");
			Output(@"
From CaptureFor: ");
			Output(CapturedContent);
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
