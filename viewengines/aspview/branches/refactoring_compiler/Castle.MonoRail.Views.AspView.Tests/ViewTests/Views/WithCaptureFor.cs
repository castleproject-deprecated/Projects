namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class WithCaptureFor : AspViewBase
	{
		public override void Render()
		{
			Output(@"Parent
");
			InvokeViewComponent("CaptureFor", CaptureFor1_body, null, "id", "CapturedContent");
			Output(@"
Parent");
		}


		internal void CaptureFor1_body()
		{
			Output(@"The captured content");
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
