using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class usingviewcomponents_usingcomponentwithasinglelettername : AspViewBase
{
public override void Render()
{
Output(@"some text before viewcomponent
");
ViewComponentContext viewComponentContext6 = new ViewComponentContext(this, null, "A", this.OutputWriter );
				this.AddProperties(viewComponentContext6.ContextVars);
				ViewComponent A6 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("A");

				A6.Init(Context, viewComponentContext6);
				A6.Render();
				if (viewComponentContext6.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext6.ViewToRender, viewComponentContext6.ContextVars);
Output(@"

some text after viewcomponent");

}
protected override string ViewName { get { return "\\UsingViewComponents\\UsingComponentWithASingleLetterName.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }
}
}
