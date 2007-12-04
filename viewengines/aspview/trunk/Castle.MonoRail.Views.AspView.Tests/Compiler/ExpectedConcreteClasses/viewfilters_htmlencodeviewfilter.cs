using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class viewfilters_htmlencodeviewfilter : AspViewBase
	{
		protected override string ViewName { get { return "\\ViewFilters\\HtmlEncodeViewFilter.aspx"; } }
		protected override string ViewDirectory { get { return "\\ViewFilters"; } }


		public override void Render()
		{
StartFiltering(new Castle.MonoRail.Views.AspView.ViewFilters.HtmlEncodeViewFilter());
Output(@"<html>");
EndFiltering();

		}

	}
}
