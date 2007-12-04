using System;

namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class SimplestLayout : AspViewBase
	{
		public override void Render()
		{
			Output(@"Layout - before
");
			Output(ViewContents);
			Output(@"
Layout - after");
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
