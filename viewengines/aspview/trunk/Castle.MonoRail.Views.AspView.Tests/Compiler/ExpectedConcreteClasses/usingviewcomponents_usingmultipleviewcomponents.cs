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
ViewComponentContext viewComponentContext10 = new ViewComponentContext(this, new ViewComponentSectionRendereDelegate(CaptureFor10_body), "CaptureFor", this.OutputWriter , "id", "capturedContent1");
				this.AddProperties(viewComponentContext10.ContextVars);
				ViewComponent CaptureFor10 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("CaptureFor");

				CaptureFor10.Init(Context, viewComponentContext10);
				CaptureFor10.Render();
				if (viewComponentContext10.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext10.ViewToRender, viewComponentContext10.ContextVars);
Output(@"

");
ViewComponentContext viewComponentContext11 = new ViewComponentContext(this, new ViewComponentSectionRendereDelegate(CaptureFor11_body), "CaptureFor", this.OutputWriter , "id", "capturedContent2");
				this.AddProperties(viewComponentContext11.ContextVars);
				ViewComponent CaptureFor11 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("CaptureFor");

				CaptureFor11.Init(Context, viewComponentContext11);
				CaptureFor11.Render();
				if (viewComponentContext11.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext11.ViewToRender, viewComponentContext11.ContextVars);
Output(@"

Some view text
The next text should be bolded:
");
ViewComponentContext viewComponentContext12 = new ViewComponentContext(this, new ViewComponentSectionRendereDelegate(Bold12_body), "Bold", this.OutputWriter );
				this.AddProperties(viewComponentContext12.ContextVars);
				ViewComponent Bold12 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("Bold");

				Bold12.Init(Context, viewComponentContext12);
				Bold12.Render();
				if (viewComponentContext12.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext12.ViewToRender, viewComponentContext12.ContextVars);
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
