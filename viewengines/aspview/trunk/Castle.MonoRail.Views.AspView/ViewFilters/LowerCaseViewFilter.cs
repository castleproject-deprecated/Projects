using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.MonoRail.Views.AspView.ViewFilters
{
    public class LowerCaseViewFilter : IViewFilter
    {
        #region IViewFilter Members
        public string ApplyOn(string input)
        {
            return input.ToLower();
        }
        #endregion
    }
}
