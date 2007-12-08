using System;

namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class LayoutUsesCaptureFor : AspViewBase
	{
		private string CapturedContent { get { return (string)GetParameter("CapturedContent"); } }
		public override void Render()
		{
			Output(@"View: ");
			Output(ViewContents);
			Output(@"
From CaptureFor: ");
			Output(CapturedContent);
		}
		protected override string ViewDirectory
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}
		protected override string ViewName
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}
	}
}
