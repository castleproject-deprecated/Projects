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
public override void Render()
{
Output(@"a. Some text, located before the capturedContent component
");
ViewComponentContext viewComponentContext2 = new ViewComponentContext(this, new ViewComponentSectionRendereDelegate(CaptureFor2_body), "CaptureFor", this.OutputWriter , "id", "capturedContent");
				this.AddProperties(viewComponentContext2.ContextVars);
				ViewComponent CaptureFor2 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("CaptureFor");

				CaptureFor2.Init(Context, viewComponentContext2);
				CaptureFor2.Render();
				if (viewComponentContext2.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext2.ViewToRender, viewComponentContext2.ContextVars);
Output(@"

b. Some text, located after the capturedContent component
This text should be rendered right after text b.
");

}
protected override string ViewName { get { return "\\UsingViewComponents\\NestedInCaptureFor.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }

				internal void Bold3_body ()
				{
					Output(@"Bolded, yet still in the captured-for place holder");

				}

				internal void CaptureFor2_body ()
				{
					Output(@"
This content should be rendered in the captured-for place holder
");
ViewComponentContext viewComponentContext3 = new ViewComponentContext(this, new ViewComponentSectionRendereDelegate(Bold3_body), "Bold", this.OutputWriter );
				this.AddProperties(viewComponentContext3.ContextVars);
				ViewComponent Bold3 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("Bold");

				Bold3.Init(Context, viewComponentContext3);
				Bold3.Render();
				if (viewComponentContext3.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext3.ViewToRender, viewComponentContext3.ContextVars);
Output(@"

Not bolded anymore, yet still in the captured-for place holder
");

				}
}
}
