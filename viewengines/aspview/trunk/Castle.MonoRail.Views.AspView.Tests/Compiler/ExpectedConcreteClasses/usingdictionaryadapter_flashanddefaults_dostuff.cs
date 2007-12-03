using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class usingdictionaryadapter_flashanddefaults_dostuff : AspViewBase<AspViewTestSite.Interfaces.UsingDictionaryAdapter.IStupidView>
{
public override void Render()
{
Output(@"The data was: <br />
Id: ");
Output( view.Id);
Output(@", Name: ");
Output( view.Name);

}
protected override string ViewName { get { return "\\UsingDictionaryAdapter\\FlashAndDefaults\\DoStuff.aspx"; } }
protected override string ViewDirectory { get { return "\\UsingDictionaryAdapter\\FlashAndDefaults"; } }
}
}
