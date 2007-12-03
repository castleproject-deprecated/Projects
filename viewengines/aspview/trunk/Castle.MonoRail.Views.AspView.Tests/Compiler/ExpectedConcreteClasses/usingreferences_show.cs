using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
using TestModel;
namespace CompiledViews
{
public class usingreferences_show : AspViewBase
{
private Post post { get { return (Post)GetParameter("post"); } }
public override void Render()
{
Output(@"<form action=""Save.rails"">
    <input type=""text"" name=""post.PublishDate"" value='");
Output(post.PublishDate);
Output(@"' />
    <input type=""text"" name=""post.Content"" value='");
Output(post.Content);
Output(@"' />
    <input type=""submit"" value=""Save"" />
</form>
");

}
protected override string ViewName { get { return "\\UsingReferences\\Show.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingReferences"; } }
}
}
