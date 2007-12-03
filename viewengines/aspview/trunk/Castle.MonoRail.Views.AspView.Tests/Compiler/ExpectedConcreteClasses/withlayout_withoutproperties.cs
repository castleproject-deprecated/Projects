using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class withlayout_withoutproperties : AspViewBase
{
public override void Render()
{
Output(@"A View without any properties");

}
protected override string ViewName { get { return "\\WithLayout\\WithoutProperties.aspx"; } }
protected override string ViewDirectory { get { return "\\WithLayout"; } }
}
}
