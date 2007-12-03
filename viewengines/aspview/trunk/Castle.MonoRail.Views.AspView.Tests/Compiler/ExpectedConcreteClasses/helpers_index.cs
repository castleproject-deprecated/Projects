using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class helpers_index : AspViewBase
{
private string s { get { return (string)GetParameter("s"); } }
public override void Render()
{
Output(this.Helpers.Form.TextField("object.field"));

}
protected override string ViewName { get { return "\\Helpers\\Index.aspx"; } }
protected override string ViewDirectory { get { return "\\Helpers"; } }
}
}
