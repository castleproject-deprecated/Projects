using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class home_index : AspViewBase
{
private string[] strings { get { return (string[])GetParameter("strings"); } }
public override void Render()
{
Output(@"hello from index<br />
This are the strings:<br />
");
foreach (string s in strings) {
Output(@"
    ");
Output(s);
Output(@"<br />
");
}
Output(@"
        
<br />
End of normal view
<br />
");
string message = "Hello";
Output(@"
");
OutputSubView("SubViewSample", "message", message, "number", 1);
Output(@"
<form action=""Print.rails"">
<input type=""text"" name=""theText"" />
<input type=""submit"" value=""send"" />
</form>");

}
protected override string ViewName { get { return "\\Home\\Index.aspx"; } }
protected override string ViewDirectory { get { return "\\Home"; } }
}
}
