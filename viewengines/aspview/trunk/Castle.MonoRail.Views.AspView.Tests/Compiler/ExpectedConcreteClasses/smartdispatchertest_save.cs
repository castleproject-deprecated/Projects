using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class smartdispatchertest_save : AspViewBase
{
private string message { get { return (string)GetParameter("message"); } }
public override void Render()
{
Output(@" Saved OK. <br />
 message: ");
Output(message);

}
protected override string ViewName { get { return "\\SmartDispatcherTest\\Save.aspx"; } }
protected override string ViewDirectory { get { return "\\SmartDispatcherTest"; } }
}
}
