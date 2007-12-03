using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class usingviewcomponents_nested : AspViewBase
{
public override void Render()
{
Output(@"before the components
");
ViewComponentContext viewComponentContext0 = new ViewComponentContext(this, new ViewComponentSectionRendereDelegate(Bold0_body), "Bold", this.OutputWriter );
				this.AddProperties(viewComponentContext0.ContextVars);
				ViewComponent Bold0 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("Bold");

				Bold0.Init(Context, viewComponentContext0);
				Bold0.Render();
				if (viewComponentContext0.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext0.ViewToRender, viewComponentContext0.ContextVars);
Output(@"

after all components");

}
protected override string ViewName { get { return "\\UsingViewComponents\\Nested.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }

				internal void Bold1_body ()
				{
					Output(@"
		in secondary bolded
	");

				}

				internal void Bold0_body ()
				{
					Output(@"
	in inner bolded
	");
ViewComponentContext viewComponentContext1 = new ViewComponentContext(this, new ViewComponentSectionRendereDelegate(Bold1_body), "Bold", this.OutputWriter );
				this.AddProperties(viewComponentContext1.ContextVars);
				ViewComponent Bold1 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("Bold");

				Bold1.Init(Context, viewComponentContext1);
				Bold1.Render();
				if (viewComponentContext1.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext1.ViewToRender, viewComponentContext1.ContextVars);
Output(@"

	back in outer bolded
");

				}
}
}
