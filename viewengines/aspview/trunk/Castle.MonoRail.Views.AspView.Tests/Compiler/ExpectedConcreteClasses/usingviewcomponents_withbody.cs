using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class usingviewcomponents_withbody : AspViewBase
{
public override void Render()
{
Output(@"A simple viewcomponent, without a body and sections
");
ViewComponentContext viewComponentContext13 = new ViewComponentContext(this, new ViewComponentSectionRendereDelegate(Bold13_body), "Bold", this.OutputWriter );
				this.AddProperties(viewComponentContext13.ContextVars);
				ViewComponent Bold13 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("Bold");

				Bold13.Init(Context, viewComponentContext13);
				Bold13.Render();
				if (viewComponentContext13.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext13.ViewToRender, viewComponentContext13.ContextVars);
Output(@"

I was supposed to be rendered after the viewcomponent");

}
protected override string ViewName { get { return "\\UsingViewComponents\\WithBody.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }

				internal void Bold13_body ()
				{
					Output(@"I was supposed to be rendered in the viewcomponent");

				}
}
}
