using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class usingviewcomponents_usingcomponentwithdotinaliteralparametervalue : AspViewBase
{
public override void Render()
{
Output(@"some text before viewcomponent
");
ViewComponentContext viewComponentContext7 = new ViewComponentContext(this, null, "Echo", this.OutputWriter , "out", "with.dot");
				this.AddProperties(viewComponentContext7.ContextVars);
				ViewComponent Echo7 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("Echo");

				Echo7.Init(Context, viewComponentContext7);
				Echo7.Render();
				if (viewComponentContext7.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext7.ViewToRender, viewComponentContext7.ContextVars);
Output(@"

some text after viewcomponent");

}
protected override string ViewName { get { return "\\UsingViewComponents\\UsingComponentWithDotInALiteralParameterValue.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }
}
}
