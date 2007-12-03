using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class usingviewcomponents_simple : AspViewBase
{
public override void Render()
{
Output(@"A simple viewcomponent, without a body and sections
");
ViewComponentContext viewComponentContext4 = new ViewComponentContext(this, null, "Simple", this.OutputWriter );
				this.AddProperties(viewComponentContext4.ContextVars);
				ViewComponent Simple4 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("Simple");

				Simple4.Init(Context, viewComponentContext4);
				Simple4.Render();
				if (viewComponentContext4.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext4.ViewToRender, viewComponentContext4.ContextVars);
Output(@"

I was supposed to be rendered after the viewcomponent");

}
protected override string ViewName { get { return "\\UsingViewComponents\\Simple.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }
}
}
