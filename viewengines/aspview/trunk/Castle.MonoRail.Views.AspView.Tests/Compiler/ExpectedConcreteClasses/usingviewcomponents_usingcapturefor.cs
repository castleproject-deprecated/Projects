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
ViewComponentContext viewComponentContext5 = new ViewComponentContext(this, new ViewComponentSectionRendereDelegate(CaptureFor5_body), "CaptureFor", this.OutputWriter , "id", "capturedContent");
				this.AddProperties(viewComponentContext5.ContextVars);
				ViewComponent CaptureFor5 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("CaptureFor");

				CaptureFor5.Init(Context, viewComponentContext5);
				CaptureFor5.Render();
				if (viewComponentContext5.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext5.ViewToRender, viewComponentContext5.ContextVars);
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
