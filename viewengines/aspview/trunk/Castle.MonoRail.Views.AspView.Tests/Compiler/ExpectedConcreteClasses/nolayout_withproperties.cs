using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class nolayout_withproperties : AspViewBase
{
private string data { get { return (string)GetParameter("data"); } }
public override void Render()
{
Output(@"A View with properties: ");
Output(data);

}
protected override string ViewName { get { return "\\NoLayout\\WithProperties.aspx"; } }
protected override string ViewDirectory { get { return "\\NoLayout"; } }
}
}
