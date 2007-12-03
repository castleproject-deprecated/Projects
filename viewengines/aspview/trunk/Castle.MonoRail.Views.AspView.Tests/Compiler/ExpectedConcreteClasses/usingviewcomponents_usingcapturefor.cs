using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class usingviewcomponents_usingcapturefor : AspViewBase
{
public override void Render()
{
Output(@"a. Some text, located before the capturedContent component
");
InvokeViewComponent("CaptureFor", new ViewComponentSectionRendereDelegate(CaptureFor5_body), new KeyValuePair<string, object>[] {  } , "id", "capturedContent");
Output(@"
b. Some text, located after the capturedContent component
This text should be rendered right after text b.
");

}
protected override string ViewName { get { return "\\UsingViewComponents\\UsingCaptureFor.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }

				internal void CaptureFor5_body ()
				{
					Output(@"
This content should be rendered in the captured-for place holder
");

				}
}
}
