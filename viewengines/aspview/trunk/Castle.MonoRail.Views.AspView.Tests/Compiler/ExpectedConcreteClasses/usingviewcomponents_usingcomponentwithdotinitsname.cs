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
InvokeViewComponent("With.Dot.In.Name", null, new KeyValuePair<string, object>[] {  } );
Output(@"
some text after viewcomponent");

}
protected override string ViewName { get { return "\\UsingViewComponents\\UsingComponentWithDotInItsName.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }
}
}
