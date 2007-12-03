using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class viewfilters_loweranduppercaseviewfilters : AspViewBase
{
public override void Render()
{
Output(@"Outside the filter
");
StartFiltering(new Castle.MonoRail.Views.AspView.ViewFilters.LowerCaseViewFilter());
Output(@"
Inside the LowerCaseViewFilter - this text should be viewed in lower case
");
EndFiltering();
Output(@"
Outside the filter AGain
");
StartFiltering(new Castle.MonoRail.Views.AspView.ViewFilters.UpperCaseViewFilter());
Output(@"
Inside the UpperCaseViewFilter - this text should be viewed in upper case
");
EndFiltering();
Output(@"
Finally - outside the filter");

}
protected override string ViewName { get { return "\\ViewFilters\\LowerAndUpperCaseViewFilters.aspx"; } }
protected override string ViewDirectory { get { return "\\ViewFilters"; } }
}
}
