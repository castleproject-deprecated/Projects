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
InvokeViewComponent("Bold", new ViewComponentSectionRendereDelegate(Bold0_body), new KeyValuePair<string, object>[] {  } );
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
InvokeViewComponent("Bold", new ViewComponentSectionRendereDelegate(Bold1_body), new KeyValuePair<string, object>[] {  } );
Output(@"
	back in outer bolded
");

				}
}
}
