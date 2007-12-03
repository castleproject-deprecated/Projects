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
InvokeViewComponent("Echo", null, new KeyValuePair<string, object>[] {  } , "out", "with.dot");
Output(@"
some text after viewcomponent");

}
protected override string ViewName { get { return "\\UsingViewComponents\\UsingComponentWithDotInAParameterValue.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }
}
}
