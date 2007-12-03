using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class usingdictionaryadapter_flashanddefaults_index : AspViewBase<AspViewTestSite.Interfaces.UsingDictionaryAdapter.IStupidView>
{
public override void Render()
{
Output(@"<p>");
Output(view.Message);
Output(@"</p>
<form action=""DoStuff.rails"">
	Name: <input type=""text"" name=""name"" value=""");
Output( view.Name);
Output(@""" /> <br />
	password: <input type=""password"" name=""password"" /> <br />
	<input type=""submit"" />
</form>");

}
protected override string ViewName { get { return "\\UsingDictionaryAdapter\\FlashAndDefaults\\Index.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingDictionaryAdapter\\FlashAndDefaults"; } }
}
}
