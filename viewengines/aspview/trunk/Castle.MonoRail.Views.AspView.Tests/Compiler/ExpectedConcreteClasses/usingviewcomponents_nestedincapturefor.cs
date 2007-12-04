using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class usingviewcomponents_nestedincapturefor : AspViewBase
	{
		protected override string ViewName { get { return "\\UsingViewComponents\\NestedInCaptureFor.aspx"; } }
		protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }


		public override void Render()
		{
Output(@"a. Some text, located before the capturedContent component
");
InvokeViewComponent("CaptureFor", new ViewComponentSectionRendereDelegate(CaptureFor2_body), new KeyValuePair<string, object>[] {  } , "id", "capturedContent");
Output(@"
b. Some text, located after the capturedContent component
This text should be rendered right after text b.
");

		}

		internal void Bold3_body ()
		{
			Output(@"Bolded, yet still in the captured-for place holder");

		}

		internal void CaptureFor2_body ()
		{
			Output(@"
			This content should be rendered in the captured-for place holder
			");
			InvokeViewComponent("Bold", new ViewComponentSectionRendereDelegate(Bold3_body), new KeyValuePair<string, object>[] {  } );
			Output(@"
			Not bolded anymore, yet still in the captured-for place holder
			");

		}

	}
}
