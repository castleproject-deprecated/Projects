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
private string text { get { return (string)GetParameter("text", "some variable text"); } }
public override void Render()
{
Output(@"Some view text
");
InvokeViewComponent("CaptureFor", new ViewComponentSectionRendereDelegate(CaptureFor10_body), new KeyValuePair<string, object>[] {  } , "id", "capturedContent1");
Output(@"
");
InvokeViewComponent("CaptureFor", new ViewComponentSectionRendereDelegate(CaptureFor11_body), new KeyValuePair<string, object>[] {  } , "id", "capturedContent2");
Output(@"
Some view text
The next text should be bolded:
");
InvokeViewComponent("Bold", new ViewComponentSectionRendereDelegate(Bold12_body), new KeyValuePair<string, object>[] {  } );
Output(@"
Some view text - not bolded");

}
protected override string ViewName { get { return "\\UsingViewComponents\\UsingMultipleViewComponents.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }

				internal void CaptureFor10_body ()
				{
					Output(@"
This content should be rendered in the captured-for place holder no. 1
");

				}

				internal void CaptureFor11_body ()
				{
					Output(@"
This content should be rendered in the captured-for place holder no. 2
");

				}

				internal void Bold12_body ()
				{
					Output(@"I should be bold, ");
Output(text);
Output(@"and within a BoldViewComponent");

				}
}
}
