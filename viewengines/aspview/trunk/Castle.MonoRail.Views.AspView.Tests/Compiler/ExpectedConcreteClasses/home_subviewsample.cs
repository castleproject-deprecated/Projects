using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class home_subviewsample : AspViewBase
{
private string message { get { return (string)GetParameter("message"); } }
private int number { get { return (int)GetParameter("number"); } }
public override void Render()
{
Output(@"<div>");
Output(message);
Output(@"</div>
<div>");
Output(number);
Output(@"</div>");

}
protected override string ViewName { get { return "\\Home\\SubViewSample.aspx"; } }
protected override string ViewDirectory { get { return "\\Home"; } }
}
}
