using System;

namespace Castle.MonoRail.Views.AspView.Tests.Stubs
{
	public class StubView : AspViewBase
	{
		public override void Render()
		{
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
