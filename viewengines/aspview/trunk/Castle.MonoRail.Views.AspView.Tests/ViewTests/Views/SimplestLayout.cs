using System;

namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class SimplestLayout : AspViewBase
	{
		private void OutputLine(string text)
		{
			Output(text);
			Output(Environment.NewLine);
		}

		public override void Render()
		{
			OutputLine("Layout - before");
			OutputLine(ViewContents);
			OutputLine("Layout - after");
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
