using System;

namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class LayoutWithSubView : AspViewBase
	{
		public override void Render()
		{
			Output(@"Layout - before
");
			Output(ViewContents);
			Output(@"
Layout - after
");
			OutputSubView("Subview");
			Output(@"
Layout - after SubView");
		}
		protected override string ViewDirectory
		{
			get { return ""; }
		}
		protected override string ViewName
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}
	}
}
