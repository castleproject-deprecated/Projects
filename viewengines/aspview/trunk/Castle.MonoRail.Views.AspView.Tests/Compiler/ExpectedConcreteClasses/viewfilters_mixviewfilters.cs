using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class viewfilters_mixviewfilters : AspViewBase
{
public override void Render()
{
Output(@"outside the filter
");
StartFiltering("SingleLineViewFilter");
Output(@"
first line
");
StartFiltering(new Castle.MonoRail.Views.AspView.ViewFilters.UpperCaseViewFilter());
Output(@"
THIS TEXT SHOULD BE IN UPPER CASE AND IN THE SAME LINE AS THE SURROUNDING TEXT
");
EndFiltering();
Output(@"
second line
");
EndFiltering();
Output(@"
outside the filter again - first line
outside the filter again - second line");

}
protected override string ViewName { get { return "\\ViewFilters\\MixViewFilters.aspx"; } }
protected override string ViewDirectory { get { return "\\ViewFilters"; } }
}
}
