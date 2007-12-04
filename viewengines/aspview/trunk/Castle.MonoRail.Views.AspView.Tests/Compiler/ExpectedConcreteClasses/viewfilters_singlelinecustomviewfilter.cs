using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class viewfilters_singlelinecustomviewfilter : AspViewBase
	{
		protected override string ViewName { get { return "\\ViewFilters\\SingleLineCustomViewFilter.aspx"; } }
		protected override string ViewDirectory { get { return "\\ViewFilters"; } }


		public override void Render()
		{
Output(@"outside the filter
");
StartFiltering("SingleLineViewFilter");
Output(@"
first line
second line
");
EndFiltering();
Output(@"
outside the filter again - first line
outside the filter again - second line");

		}

	}
}
