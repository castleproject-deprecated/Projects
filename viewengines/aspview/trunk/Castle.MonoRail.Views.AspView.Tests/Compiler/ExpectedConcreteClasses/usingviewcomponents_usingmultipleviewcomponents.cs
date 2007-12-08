using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class usingviewcomponents_usingmultipleviewcomponents : AspViewBase
	{
		protected override string ViewName { get { return "\\UsingViewComponents\\UsingMultipleViewComponents.aspx"; } }
		protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }

		private string text { get { return (string)GetParameter("text", "some variable text"); } }

		public override void Render()
		{
Output(@"Some view text
");
InvokeViewComponent("CaptureFor2", CaptureFor20_body, null, "id", "capturedContent1");
Output(@"
");
InvokeViewComponent("CaptureFor2", CaptureFor21_body, null, "id", "capturedContent2");
Output(@"
Some view text
The next text should be bolded:
");
InvokeViewComponent("Bold", Bold2_body, null);
Output(@"
Some view text - not bolded");

		}

		internal void CaptureFor20_body ()
		{
			Output(@"
			This content should be rendered in the captured-for place holder no. 1
			");

		}

		internal void CaptureFor21_body ()
		{
			Output(@"
			This content should be rendered in the captured-for place holder no. 2
			");

		}

		internal void Bold2_body ()
		{
			Output(@"I should be bold, ");
			Output(text);
			Output(@"and within a BoldViewComponent");

		}

	}
}
