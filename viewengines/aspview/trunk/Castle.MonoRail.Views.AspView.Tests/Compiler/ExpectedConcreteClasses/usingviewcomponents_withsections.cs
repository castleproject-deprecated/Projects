using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
using Castle.MonoRail.Framework.Helpers;
namespace CompiledViews
{
public class usingviewcomponents_withsections : AspViewBase
{
private string[] items { get { return (string[])GetParameter("items"); } }
private string item { get { return (string)GetParameter("item"); } }
public override void Render()
{
Output(@"A simple viewcomponent, without a body and sections
");
ViewComponentContext viewComponentContext14 = new ViewComponentContext(this, new ViewComponentSectionRendereDelegate(Repeater14_body), "Repeater", this.OutputWriter , "source", items);
				this.AddProperties(viewComponentContext14.ContextVars);
				ViewComponent Repeater14 = ((IViewComponentFactory)Context.GetService(typeof(IViewComponentFactory))).Create("Repeater");

				viewComponentContext14.RegisterSection("header", new ViewComponentSectionRendereDelegate(Repeater14_header1));


				viewComponentContext14.RegisterSection("item", new ViewComponentSectionRendereDelegate(Repeater14_item2));


				viewComponentContext14.RegisterSection("footer", new ViewComponentSectionRendereDelegate(Repeater14_footer3));


				Repeater14.Init(Context, viewComponentContext14);
				Repeater14.Render();
				if (viewComponentContext14.ViewToRender != null)
					OutputSubView("\\" + viewComponentContext14.ViewToRender, viewComponentContext14.ContextVars);
Output(@"

I was supposed to be rendered after the viewcomponent");

}
protected override string ViewName { get { return "\\UsingViewComponents\\WithSections.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }

				internal void Repeater14_body ()
				{
					Output(@"
	<section:header>
	<table>
		<thead>
			<th>Id</th>
			<th>Word</th>
		</thead>
	</section:header>
	<section:item>
		<tr>
			<td>1</td>
			<td>");
Output(item);
Output(@"</td>
		</tr>
	</section:item>
	<section:footer>
	</table>
	</section:footer>
");

				}

				internal void Repeater14_header1 ()
				{
					Output(@"
	<table>
		<thead>
			<th>Id</th>
			<th>Word</th>
		</thead>
	");

				}

				internal void Repeater14_item2 ()
				{
					Output(@"
		<tr>
			<td>1</td>
			<td>");
Output(item);
Output(@"</td>
		</tr>
	");

				}

				internal void Repeater14_footer3 ()
				{
					Output(@"
	</table>
	");

				}
}
}
