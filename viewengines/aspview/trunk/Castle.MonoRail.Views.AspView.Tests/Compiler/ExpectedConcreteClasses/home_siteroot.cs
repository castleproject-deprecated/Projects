using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class home_siteroot : AspViewBase
{
public override void Render()
{
Output(@"<a href=""");
Output(siteRoot);
Output(@"/home"">Home</a>
<a href=""");
Output(fullSiteRoot);
Output(@"/away"">Away</a>");

}
protected override string ViewName { get { return "\\Home\\SiteRoot.aspx"; } }
protected override string ViewDirectory { get { return "\\Home"; } }
}
}
