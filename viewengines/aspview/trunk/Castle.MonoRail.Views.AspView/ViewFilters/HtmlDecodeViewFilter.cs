using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Castle.MonoRail.Views.AspView.ViewFilters
{
	public class HtmlDecodeViewFilter : IViewFilter
    {
        #region IViewFilter Members
        public string ApplyOn(string input)
        {
			return HttpUtility.HtmlDecode(input);
        }
        #endregion
    }
}