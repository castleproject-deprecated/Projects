using System;

namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class OuterLayout : AspViewBase
	{
		public override void Render()
		{
			Output(@"Outer Layout - before
");
			Output(ViewContents);
			Output(@"
Outer Layout - after");
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
