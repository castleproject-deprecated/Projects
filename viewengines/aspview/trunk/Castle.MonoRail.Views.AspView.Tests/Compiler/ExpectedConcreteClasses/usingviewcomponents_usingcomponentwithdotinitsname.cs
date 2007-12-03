using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class usingviewcomponents_usingcomponentwithdotinitsname : AspViewBase
{
public override void Render()
{
Output(@"some text before viewcomponent
");
ViewComponentContext viewComponentContext9 = new ViewComponentContext(this, null, "With.Dot.In.Name", this.OutputWriter );
				this.AddProperties(viewComponentContext9.ContextVars);
				ViewComponent With_Dot_In_Name9 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("With.Dot.In.Name");

				With_Dot_In_Name9.Init(Context, viewComponentContext9);
				With_Dot_In_Name9.Render();
				if (viewComponentContext9.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext9.ViewToRender, viewComponentContext9.ContextVars);
Output(@"

some text after viewcomponent");

}
protected override string ViewName { get { return "\\UsingViewComponents\\UsingComponentWithDotInItsName.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }
}
}
