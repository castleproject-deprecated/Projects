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
		protected override string ViewName { get { return "\\UsingViewComponents\\WithSections.aspx"; } }
		protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }

		private string[] items { get { return (string[])GetParameter("items"); } }
		private string item { get { return (string)GetParameter("item"); } }

		public override void Render()
		{
Output(@"A simple viewcomponent, without a body and sections
");
InvokeViewComponent("Repeater", null, new KeyValuePair<string, ViewComponentSectionRendereDelegate>[] { new KeyValuePair<string, ViewComponentSectionRendereDelegate>("header", Repeater0_header) , new KeyValuePair<string, ViewComponentSectionRendereDelegate>("item", Repeater0_item) , new KeyValuePair<string, ViewComponentSectionRendereDelegate>("footer", Repeater0_footer)  }, "source", items);
Output(@"
I was supposed to be rendered after the viewcomponent");

		}

		internal void Repeater0_header ()
		{
			Output(@"
				<table>
					<thead>
						<th>Id</th>
						<th>Word</th>
					</thead>
				");

		}

		internal void Repeater0_item ()
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

		internal void Repeater0_footer ()
		{
			Output(@"
				</table>
				");

		}

	}
}
