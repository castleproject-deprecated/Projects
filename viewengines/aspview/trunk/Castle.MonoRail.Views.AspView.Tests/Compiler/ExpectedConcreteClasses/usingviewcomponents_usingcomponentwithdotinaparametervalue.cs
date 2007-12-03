using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class usingviewcomponents_usingcomponentwithdotinaparametervalue : AspViewBase
{
public override void Render()
{
Output(@"some text before viewcomponent
");
ViewComponentContext viewComponentContext8 = new ViewComponentContext(this, null, "Echo", this.OutputWriter , "out", "with.dot");
				this.AddProperties(viewComponentContext8.ContextVars);
				ViewComponent Echo8 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("Echo");

				Echo8.Init(Context, viewComponentContext8);
				Echo8.Render();
				if (viewComponentContext8.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext8.ViewToRender, viewComponentContext8.ContextVars);
Output(@"

some text after viewcomponent");

}
protected override string ViewName { get { return "\\UsingViewComponents\\UsingComponentWithDotInAParameterValue.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }
}
}
