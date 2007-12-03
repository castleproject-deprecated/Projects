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
InvokeViewComponent("Repeater", new ViewComponentSectionRendereDelegate(Repeater14_body), new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("header", new ViewComponentSectionRendereDelegate(Repeater14_header1)) , new KeyValuePair<string, object>("item", new ViewComponentSectionRendereDelegate(Repeater14_item2)) , new KeyValuePair<string, object>("footer", new ViewComponentSectionRendereDelegate(Repeater14_footer3))  } , "source", items);
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
