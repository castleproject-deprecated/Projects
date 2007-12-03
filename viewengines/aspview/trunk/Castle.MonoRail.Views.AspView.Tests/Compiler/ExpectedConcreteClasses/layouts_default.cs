using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class layouts_default : AspViewBase
{
public override void Render()
{
Output(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">

<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head>
    <title>AspView layout test</title>
</head>
<body>
    <div>
        hello from default layout
    </div>
    <div>
        ");
Output(ViewContents);
Output(@"
    </div>
</body>
</html>
");

}
protected override string ViewName { get { return "\\Layouts\\Default.aspx"; } }
protected override string ViewDirectory { get { return "\\Layouts"; } }
}
}
